using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DiskRenderer : MonoBehaviour
{
	private Mesh _mesh;
	[SerializeField] Shader _shader = default;
	private Material _material;
	private ComputeBuffer _drawArgsBuffer;
	private MaterialPropertyBlock _props;
	private int _instanceCount;
	private uint _indexCount;
	private uint[] _drawArgs;
	private Bounds _bounds;

	public void Init(int instanceCount)
	{
		_material = new Material(_shader);
		_material.enableInstancing = true;

		_mesh = CreateMesh();

		_instanceCount = instanceCount;

		_drawArgsBuffer = new ComputeBuffer(
			1, 5 * sizeof(uint), ComputeBufferType.IndirectArguments
		);

		_indexCount = _mesh.GetIndexCount(0);
		_drawArgs = new uint[5]
		{
			_indexCount, (uint) _instanceCount, 0, 0, 0
		};
		_drawArgsBuffer.SetData(_drawArgs);

		// This property block is used only for avoiding an instancing bug.
		_props = new MaterialPropertyBlock();
		_props.SetFloat("_UniqueID", Random.value);

		_bounds = new Bounds(Vector3.zero, Vector3.one * 1000);
	}

	private Mesh CreateMesh()
	{
		Vector3[] positions = new Vector3[4];
		int[] indices = { 0, 1, 2, 2, 1, 3 };

		positions[0] = new Vector3(-1, -1, 0f);
		positions[1] = new Vector3(1, -1, 0f);
		positions[2] = new Vector3(-1, 1, 0f);
		positions[3] = new Vector3(1, 1, 0f);

		Mesh mesh = new Mesh();
		mesh.vertices = positions;
		mesh.SetIndices(indices, MeshTopology.Triangles, 0);
		return mesh;
	}

	public void Render(ComputeBuffer particleBuffer, Camera camera, Data_Parameter parameters)
	{
		_drawArgs[1] = (uint)Mathf.CeilToInt(_instanceCount * parameters.Density);
		_drawArgsBuffer.SetData(_drawArgs);

		_material.SetMatrix("_LocalToWorld", transform.localToWorldMatrix);
		_material.SetMatrix("_WorldToLocal", transform.worldToLocalMatrix);
		_material.SetBuffer("_ParticleBuffer", particleBuffer);
		_material.SetInt("_InstanceCount", _instanceCount);
		_material.SetFloat("_Opacity", parameters.SourceWeight);
		Graphics.DrawMeshInstancedIndirect(
			_mesh, 0, _material, _bounds,
			_drawArgsBuffer, 0, _props, ShadowCastingMode.Off, true, gameObject.layer, camera
		);
	}

	void OnDestroy()
	{
		if (_drawArgsBuffer != null) _drawArgsBuffer.Release();
	}
}
