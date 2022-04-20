using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownUpScaleBlur : MonoBehaviour
{
	[SerializeField] private Shader _shader = default;

	private RenderTexture[] _buffers;
	private int _maxPasses = 6;
	private Material _scalingMaterial;
	private Vector2 _texelSize = new Vector2();

	public void Init(Vector2 resolution)
	{
		_scalingMaterial = new Material(_shader);

		_buffers = CreateBuffers(_maxPasses, resolution);
	}

	private RenderTexture[] CreateBuffers(int passes, Vector2 resolution)
	{
		RenderTexture[] buffers = new RenderTexture[passes];
		int w = Mathf.FloorToInt(resolution.x);
		int h = Mathf.FloorToInt(resolution.y);
		for (var i = 0; i < passes; i++)
		{
			buffers[i] = new RenderTexture(w, h, 1);
			//Debug.Log("blur buffer: " + w + ", " + h);
			w /= 2;
			h /= 2;
		}
		return buffers;
	}


	public void Draw(RenderTexture source, Data_Parameter parameters)
	{
		RenderTexture destination;

		int n = Mathf.CeilToInt(1 + (_maxPasses - 1) * parameters.SourceBlurRadius);

		//downScale
		for (int i = 1; i < n; i++)
		{
			destination = _buffers[i];
			_texelSize.x = 1f / source.width;
			_texelSize.y = 1f / source.height;
			_scalingMaterial.SetVector("uSourceTexelSize", _texelSize);
			Graphics.Blit(source, destination, _scalingMaterial);
			source = destination;
		}
		for (int i = 1; i < n; i++)
		{
			destination = _buffers[n - i - 1];
			_texelSize.x = 1f / source.width;
			_texelSize.y = 1f / source.height;
			_scalingMaterial.SetVector("uSourceTexelSize", _texelSize);
			Graphics.Blit(source, destination, _scalingMaterial);
			source = destination;
		}
	}

	public RenderTexture Output
	{
		get { return _buffers[0]; }
	}
}



