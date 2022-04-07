using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformLayoutGroup : MonoBehaviour
{
    private enum LayoutType
	{
        Horizontal,
        Vertical
    }

    private enum Alignment
    {
        Centered,
        Left,
        Right
    }

    [SerializeField] private Vector2 _areaDimensions = Vector2.one;
    [SerializeField] private float _spacingPercentage = 0;
    [SerializeField] private LayoutType _layout = LayoutType.Horizontal;
    [SerializeField] private Alignment _allingment = Alignment.Centered;

	public Vector2 AreaDimensions { get => _areaDimensions; set { _areaDimensions = value; UpdateLayout(); } }
	public float SpacingPercentage { get => _spacingPercentage; set { _spacingPercentage = value; UpdateLayout(); } }

	[ContextMenu("UpdateLayout")]
    public void UpdateLayout()
	{
        if (transform.childCount == 0) return;

        var spacingCount = transform.childCount - 1;
        var availableSpace = 0f;
        var totalElementsSize = 0f;
        var scale = 1f;
        var allingmentOffset = 0f;
        var direction = Vector3.zero;

        switch (_layout)
		{
            case LayoutType.Horizontal:

                direction = Vector3.right;

                for (int i = 0; i < transform.childCount; i++)
                {
                    var size = transform.GetChild(i).GetComponent<ISize>().Size();

                    totalElementsSize += size.x;
                }

                availableSpace = Mathf.Max(AreaDimensions.x - spacingCount * _spacingPercentage, 0);

                break;

            case LayoutType.Vertical:

                direction = Vector3.up;

                for (int i = 0; i < transform.childCount; i++)
                {
                    var size = transform.GetChild(i).GetComponent<ISize>().Size();

                    totalElementsSize += size.y;
                }

                availableSpace = Mathf.Max(AreaDimensions.y - spacingCount * _spacingPercentage, 0);

                break;
        }


        if (totalElementsSize > availableSpace)
        {
            scale = availableSpace / totalElementsSize;
        }
        else
        {
            switch (_allingment)
            {
                case Alignment.Right:
                    allingmentOffset = (availableSpace - totalElementsSize);
                    break;
                case Alignment.Centered:
                    allingmentOffset = (availableSpace - totalElementsSize) * 0.5f;
                    break;
            }
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            child.localScale = Vector3.one * scale;
            var size = child.GetComponent<ISize>().Size();

            var areaSize = _layout == LayoutType.Horizontal ? AreaDimensions.x : AreaDimensions.y;
            var childSize = _layout == LayoutType.Horizontal ? size.x : size.y;

            var v = -areaSize * 0.5f + childSize * scale * 0.5f + allingmentOffset;
            v += i * _spacingPercentage;
            v += i * childSize * scale;
            child.localPosition = direction * v;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
	{
        Gizmos.color = Color.blue;

        var topLeft =       Vector3.left * AreaDimensions.x * 0.5f     + Vector3.up * AreaDimensions.y * 0.5f;
        var topRight =      Vector3.right * AreaDimensions.x * 0.5f    + Vector3.up * AreaDimensions.y * 0.5f;
        var bottomRight =   Vector3.right * AreaDimensions.x * 0.5f    + Vector3.down * AreaDimensions.y * 0.5f;
        var bottomLeft =    Vector3.left * AreaDimensions.x * 0.5f     + Vector3.down * AreaDimensions.y * 0.5f;

        topLeft += transform.position;
        topRight += transform.position;
        bottomRight += transform.position;
        bottomLeft += transform.position;

        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }
#endif
}
