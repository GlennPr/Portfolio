using Data;
using ParticleSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
	[SerializeField] private Data_Parameter _parameters;
	[SerializeField, Range(64, 6400)] private int _particleCount = 100;
	[Space]
	[Space]
	[Space]
	[SerializeField] private Shader _particleMeshShader;
	[SerializeField] private CameraInfo _cameraInfo;
	[Space]
	[SerializeField] private ParticlePositionUpdater _particleUpdater;
	[SerializeField] private DownUpScaleBlur _downUpScaleBlur;
	[SerializeField] private DiskRenderer _diskRenderer;

	private Material _particleMeshMaterial;

	private ParticleMesh[] _meshes = new ParticleMesh[0];
	[Header("Exposed for debug")]
	[SerializeField] private RenderTexture _outputTexture = null;

	private void Start()
    {
		_particleMeshMaterial = new Material(_particleMeshShader);

		_meshes = FindObjectsOfType<ParticleMesh>();


		var resolution = new Vector2Int(1920, 1080);	
		Init(resolution);

		_cameraInfo.Init(resolution);

		_particleUpdater.Init(_particleCount);
		_downUpScaleBlur.Init(resolution);

		_diskRenderer.Init(_particleCount);

	}

    private void LateUpdate()
    {
		#region Setup Targets and Render

		_cameraInfo.Camera.targetTexture = _outputTexture;

		for (int i = 0; i < _meshes.Length; i++)
		{
			_meshes[i].Show();
			_meshes[i].SetMaterial(_particleMeshMaterial, _cameraInfo.Camera);
        }


		_cameraInfo.Camera.Render();
		#endregion

		#region Update particles

		UpdateParticles(_outputTexture, Time.time, Time.deltaTime, _cameraInfo, _parameters.ColorTexture);

		#endregion


		for (int i = 0; i < _meshes.Length; i++)
		{
			_meshes[i].Hide();
		}


		#region Draw Particles

		_cameraInfo.Camera.targetTexture = null;
		_diskRenderer.Render(_particleUpdater.ParticleBufferSparse, _cameraInfo.Camera, _parameters);

		#endregion
	}

	private void UpdateParticles(Texture source, float time, float deltaTime, CameraInfo cameraInfo, Texture colors)
	{
		if (deltaTime > 0f)
		{
			_downUpScaleBlur.Draw((RenderTexture)source, _parameters);

			_particleUpdater.UpdateParticles(colors, _parameters, time, deltaTime, cameraInfo, source, _downUpScaleBlur.Output);
		}
	}


	private void Init(Vector2Int resolution)
	{
		_outputTexture = new RenderTexture(resolution.x, resolution.y, 24, RenderTextureFormat.ARGBHalf);
		//setting this higher than 1 drops performance significantly
		_outputTexture.antiAliasing = 1;
		_outputTexture.filterMode = FilterMode.Point;

		Texture2D black = new Texture2D(1, 1);
		black.SetPixel(0, 0, Color.black);
		black.Apply(false);
		Graphics.Blit(black, _outputTexture);
	}
}
