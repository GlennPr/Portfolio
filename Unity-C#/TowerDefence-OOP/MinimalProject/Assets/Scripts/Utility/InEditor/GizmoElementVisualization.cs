using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility.InEditor
{
	public class GizmoElementVisualization : MonoBehaviour
	{
		[SerializeField] private bool _showAlways = true;

		[Header("Visual Settings")]
		[SerializeField] private Color _color = Color.green;
		[SerializeField] private Shape _shape = Shape.Sphere;
		[SerializeField] private bool _wireframe = true;

		private enum Shape
		{
			Cube,
			Sphere
		}


#if UNITY_EDITOR
		private void OnDrawGizmosSelected()
		{
			Gizmos.color = _color;
			var originalMatrix = Gizmos.matrix;

			Gizmos.matrix = transform.localToWorldMatrix;
			var pos = Vector3.zero;


			switch (_shape)
			{
				case Shape.Sphere:

					var radius = 0.5f;

					if (_wireframe) Gizmos.DrawWireSphere(pos, radius);
					else Gizmos.DrawSphere(pos, radius);

					break;

				case Shape.Cube:

					var size = Vector3.one;

					if (_wireframe) Gizmos.DrawWireCube(pos, size);
					else Gizmos.DrawCube(pos, size);

					break;
			}

			Gizmos.matrix = originalMatrix;
		}

		private void OnDrawGizmos()
		{
			if (_showAlways)
			{
				OnDrawGizmosSelected();
			}
		}
#endif

	}
}