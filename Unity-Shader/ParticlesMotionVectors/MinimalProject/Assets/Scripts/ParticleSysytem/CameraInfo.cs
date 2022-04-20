using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraInfo : MonoBehaviour
{
	private Camera _camera;
	private Vector2 _pixelRes;
	private Vector4 _bounds;

	public void Init(Vector2 resolution)
	{
		_camera = GetComponent<Camera>();
		_camera.orthographic = true;
		_camera.orthographicSize = 1;
		_camera.backgroundColor = new Color(0, 0, 0, 0);
		_camera.depthTextureMode = DepthTextureMode.MotionVectors;

		_pixelRes = resolution;
		float aspectRatio = _pixelRes.x / _pixelRes.y;
		_bounds = new Vector4(-aspectRatio, -1, aspectRatio, 1);
	}

	public Vector4 GetScreenBounds()
	{
		return _bounds;
	}

	public Camera Camera
	{
		get { return _camera; }
	}

	public Vector2 GetResolution()
	{
		return _pixelRes;

	}
}
