using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility.InEditor
{
	public class GizmoGroupVisualization : MonoBehaviour
	{
		[Header("References")]
		[SerializeField] private Transform _targetHolder;

		[Header("Type Settings")]
		[SerializeField] private Connection _connection = Connection.None;
		[SerializeField] private bool _showAlways = true;

		[Header("Visual Settings")]
		[SerializeField] private Color _color = Color.green;
		[SerializeField] private Shape _shape = Shape.Sphere;
		[SerializeField] private bool _wireframe = true;
		[Space]
		[SerializeField] private float _radius = 0.5f;
		[SerializeField] private bool _useChildTransform = false;

		private enum Shape
		{
			Cube,
			Sphere
		}

		private enum Connection 
		{
			None,
			Linked,
			LinkedLoop
		}


#if UNITY_EDITOR
		private void OnDrawGizmosSelected()
		{
			var holder = _targetHolder == null ? transform : _targetHolder;

			if (holder != null)
			{
				Gizmos.color = _color;

				for (int i = 0; i < holder.childCount; i++)
				{
					var currentChild = holder.GetChild(i);
					var nextChild = holder.GetChild((i + 1) % holder.childCount);

					var originalMatrix = Gizmos.matrix;
					var pos = currentChild.transform.position;

					if (_useChildTransform)
					{
						Gizmos.matrix = currentChild.transform.localToWorldMatrix;
						pos = Vector3.zero;
					}

					switch (_shape)
					{
						case Shape.Sphere:

							var radius = _radius;

							if (_wireframe) Gizmos.DrawWireSphere(pos, radius);
							else Gizmos.DrawSphere(pos, radius);

							break;

						case Shape.Cube:

							var size = _radius * 2 * Vector3.one;

							if (_wireframe) Gizmos.DrawWireCube(pos, size);
							else Gizmos.DrawCube(pos, size);

							break;
					}

					Gizmos.matrix = originalMatrix;

					switch (_connection)
					{
						case Connection.Linked:

							if (i < holder.childCount - 1)
							{
								Gizmos.DrawLine(currentChild.transform.position, nextChild.transform.position);
							}
							break;

						case Connection.LinkedLoop:

							Gizmos.DrawLine(currentChild.transform.position, nextChild.transform.position);

							break;
					}

				}
			}
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