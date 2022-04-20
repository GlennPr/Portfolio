using Data;
using System;
using UnityEngine;
using UnityEngine.Video;
using Random = UnityEngine.Random;

namespace ParticleSystem
{
	public class ParticlePositionUpdater : MonoBehaviour
	{
		//keep count matched with shader
		private const int _threadCount = 64;
		//amount of floats in struct * bytes per float
		private const int _particleStride = 16 * sizeof(float);
		private const int _particleCoreStride = 8 * sizeof(float);

		private bool _needsReset = true;

		private Vector2 _blurredSourceEpsilon;
		private int _particleCount;

		private ComputeBuffer _particleBuffer;
		private ComputeBuffer _randomBuffer;
		private ComputeBuffer _retrieveBuffer;

		[SerializeField] private ComputeShader _compute = default;

		
		private int ThreadGroupCount
		{
			get { return _particleCount / _threadCount; }
		}

		public void Init(int particleCount)
		{
			_particleCount = particleCount;

			_particleBuffer = new ComputeBuffer(_particleCount, _particleStride);
			_randomBuffer = CreateRandomBuffer(_particleCount);
			_retrieveBuffer = new ComputeBuffer(_particleCount, _particleCoreStride);
		}

		private ComputeBuffer CreateRandomBuffer(int instanceCount)
		{
			int stride = 16;
			ComputeBuffer buffer = new ComputeBuffer(instanceCount, stride * sizeof(float));

			float[] data = new float[instanceCount * stride];
			int index = 0;
			var v = Vector3.zero;
			Random.InitState(1337);
			for (int i = 0; i < instanceCount; i++)
			{
				for (int j = 0; j < (stride - 3); j++)
				{
					data[index] = Random.value;
					index++;
				}

				v = Random.insideUnitSphere;
				
				data[index] = v.x;
				index++;
				
				data[index] = v.y;
				index++;

				data[index] = v.z;
				index++;
			}

			buffer.SetData(data);

			return buffer;
		}

		private void SetParameters(Data_Parameter parameters)
		{
			_compute.SetFloat("_Density", parameters.Density);
			_compute.SetFloat("_ParticleFill", parameters.ParticleFill);
			_compute.SetFloat("_BrightnessRandomness", parameters.BrightnessRandomness);

			_compute.SetFloat("_BrightnessLag", Mathf.Pow(parameters.BrightnessLag, 0.2f));
			_compute.SetFloat("_ColorLag", Mathf.Pow(parameters.ColorLag, 0.2f));
			_compute.SetFloat("_ColorOverLife", parameters.ColorOverLife);
			_compute.SetFloat("_SourceModulateColor", parameters.SourceModulateColor);
			_compute.SetFloat("_DepthModulateBrightness", parameters.DepthModulateBrightness);
			_compute.SetFloat("_UseSpawnColor", parameters.UseSpawnColor ? 1 : 0);

			_compute.SetFloat("_RandomAmbientColor", parameters.RandomAmbientColor);

			_compute.SetFloat("LifeTime", parameters.LifeTime);
			_compute.SetFloat("AmbientAmount", parameters.AmbientAmount);
			_compute.SetFloat("_SourceGlow", parameters.SourceGlow);


			_compute.SetFloat("_SourceModulateFill", parameters.SourceModulateFill);
			_compute.SetFloat("_SourceModulateBrightness", parameters.SourceModulateBrightness);
			_compute.SetFloat("_SourceModulateSize", parameters.SourceModulateSize);
			_compute.SetFloat("_DepthModulateSize", parameters.DepthModulateSize);

			_compute.SetFloat("_SourceAttraction", parameters.SourceAttraction);

			_compute.SetFloat("Inertia", Mathf.Pow(parameters.Inertia, 0.1f));
			//using a 0.01 sized mesh to prevent shader errors to cause fillrate crashes*/
			_compute.SetFloat("_Size", parameters.Size);
			_compute.SetFloat("_SizeLag", Mathf.Pow(parameters.SizeLag, 0.2f));
			_compute.SetFloat("_SizeRandomness", parameters.SizeRandomness);
			_compute.SetFloat("_SizeRandomness2", parameters.SizeRandomness2);

			_compute.SetFloat("_ScaleInLifetimePercentage", parameters.ScaleInLifetimePercentage);
			_compute.SetFloat("_ScaleOutLifetimePercentage", parameters.ScaleOutLifetimePercentage);
			_compute.SetFloat("_UseMotionVectors", parameters.UseMotionVectors ? 1 : 0);
			_compute.SetFloat("_MotionVectorRandomness", parameters.MotionVectorRandomness);

		}

		public void UpdateParticles(
			Texture colors, Data_Parameter parameters, 
			float time, float deltaTime, 
			CameraInfo cameraInfo, 
			Texture source, Texture blurredSource)
		{

			SetParameters(parameters);
			_compute.SetFloat("DeltaTime", deltaTime);
			_compute.SetFloat("Time", time);
			_compute.SetInt("InstanceCount", _particleCount);

			_blurredSourceEpsilon.x = 1f / blurredSource.width;
			_blurredSourceEpsilon.y = 1f / blurredSource.height;
			_compute.SetVector("_BlurredSourceEpsilon", _blurredSourceEpsilon);
			_compute.SetVector("_BlurredSourceEpsilonLarge", _blurredSourceEpsilon * 3);

			_compute.SetVector("_Region", cameraInfo.GetScreenBounds());
			//more padding in single screen rendering mode
			_compute.SetFloat("_RegionPadding", 0.02f);

			_compute.SetFloat("_Density", parameters.Density);

			_compute.SetVector("_SourceTextureRes", new Vector4(source.width, source.height));

			//reset
			if (_needsReset)
			{
				int k = _compute.FindKernel("Reset");
				_compute.SetBuffer(k, "ParticleBuffer", _particleBuffer);
				_compute.Dispatch(k, ThreadGroupCount, 1, 1);
				_needsReset = false;
			}

			//update
			int kernel = _compute.FindKernel("Update");

			_compute.SetBuffer(kernel, "ParticleBuffer", _particleBuffer);
			_compute.SetBuffer(kernel, "RandomBuffer", _randomBuffer);
			_compute.SetTexture(kernel, "Source", source);
			_compute.SetTexture(kernel, "BlurredSource", blurredSource);
			_compute.SetTexture(kernel, "Colors", colors);
			_compute.Dispatch(kernel, ThreadGroupCount, 1, 1);

			//copy
			//copy to a smaller buffer for download or drawing
			kernel = _compute.FindKernel("Copy");
			_compute.SetBuffer(kernel, "ParticleBuffer", _particleBuffer);
			_compute.SetBuffer(kernel, "RetrieveBuffer", _retrieveBuffer);
			_compute.Dispatch(kernel, ThreadGroupCount, 1, 1);
		}

		public ComputeBuffer ParticleBufferSparse
		{
			get { return _retrieveBuffer; }
		}

		public void Reset()
		{
			_needsReset = true;
		}

		public void OnDestroy()
		{
			_particleBuffer?.Release();
			_randomBuffer?.Release();
			_retrieveBuffer?.Release();
		}
	}
}