using UnityEngine;

namespace Data
{
	[CreateAssetMenu(fileName = "Type", menuName = "ScriptableObjects/Type", order = 1)]
	public class Data_Type : ScriptableObject
	{
		[SerializeField] private Sprite _sprite;
		[SerializeField] private Color _color = Color.white;

		public Sprite Sprite { get => _sprite; }
		public Color Color { get => _color; set => _color = value; }
	}
}