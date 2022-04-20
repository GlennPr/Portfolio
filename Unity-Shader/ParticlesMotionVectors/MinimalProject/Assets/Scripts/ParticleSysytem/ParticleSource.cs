using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ParticleSource : MonoBehaviour
{
	[SerializeField, Range(0, 1)] private float _strength = 1;
	[SerializeField] private bool _ignoreMotionVectorScaling;
	[SerializeField] private bool _invertMotionVector;

	private Matrix4x4 _viewProjectionPrev;
	private Matrix4x4 _localToWorldMatrixPrev;

	private Material _originalMat;
	private Renderer _renderer;

	private Material _particleMat = null;

	private void Awake()
	{
		_renderer = GetComponent<Renderer>();
		_originalMat = _renderer.sharedMaterial;

		Init();
	}

	public void Show()
	{
		_renderer.enabled = true;
	}

	public void Hide()
	{
		_renderer.enabled = false;
	}

	public void SetMaterial(Material mat, Camera cam)
	{
		if (_particleMat == null)
		{
			_particleMat = new Material(mat);
		}

		_particleMat.mainTexture = _originalMat.mainTexture;

		_particleMat.SetMatrix("_ViewProjectionPrev", _viewProjectionPrev);
		_viewProjectionPrev = GetViewProjection(cam);
		_particleMat.SetMatrix("_ViewProjection", _viewProjectionPrev);

		_particleMat.SetMatrix("_LocalToWorldMatrixPrev", _localToWorldMatrixPrev);
		_localToWorldMatrixPrev = GetLocalToWorldMatrix();
		_particleMat.SetMatrix("_LocalToWorldMatrix", _localToWorldMatrixPrev);


		_particleMat.SetVector("_CameraWorldPos", cam.transform.position);
		_particleMat.SetFloat("_InvertMotionVectorScaling", _invertMotionVector ? 1 : 0);
		_particleMat.SetFloat("_IgnoreMotionVectorScaling", _ignoreMotionVectorScaling ? 1 : 0);

		_particleMat.SetFloat("_Strength", _strength);

		_renderer.sharedMaterial = _particleMat;
	}

	protected abstract void Init();

	protected virtual Matrix4x4 GetLocalToWorldMatrix()
	{
		return transform.localToWorldMatrix;
	}

	private Matrix4x4 GetViewProjection(Camera camera)
	{
		return GL.GetGPUProjectionMatrix(camera.projectionMatrix, true) * camera.worldToCameraMatrix;
	}


}
