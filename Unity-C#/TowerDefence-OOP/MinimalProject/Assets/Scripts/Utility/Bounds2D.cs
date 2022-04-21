using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    public class Bounds2D : MonoBehaviour
    {
        private enum Axis
        {
            XY,
            XZ,
            ZY
        }

        [SerializeField] private Axis _axis = Axis.XZ;
        [SerializeField] private Vector2 _bounds = Vector2.one;

        private Vector3 Size()
        {
            var size = Vector3.zero;

            switch (_axis)
            {
                case Axis.XY:

                    size.x = _bounds.x;
                    size.y = _bounds.y;

                    break;
                case Axis.XZ:

                    size.x = _bounds.x;
                    size.z = _bounds.y;

                    break;
                case Axis.ZY:

                    size.z = _bounds.x;
                    size.y = _bounds.y;

                    break;
            }

            return size;
        }

        public Vector3 GetRandomPoint()
        {
            var size = Size() * 0.5f;

            var pos = Vector3.zero;
            pos.x = Mathf.Lerp(-size.x, size.x, Random.value);
            pos.y = Mathf.Lerp(-size.y, size.y, Random.value);
            pos.z = Mathf.Lerp(-size.z, size.z, Random.value);

            return transform.position + pos;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            var originalMatrix = Gizmos.matrix;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.red;

            Gizmos.DrawWireCube(Vector3.zero, Size());

            Gizmos.matrix = originalMatrix;
        }
#endif
    }
}