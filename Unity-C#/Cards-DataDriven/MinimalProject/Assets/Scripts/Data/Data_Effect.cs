using UnityEngine;

namespace Data
{
	[CreateAssetMenu(fileName = "Effect", menuName = "ScriptableObjects/Effect", order = 1)]
	public class Data_Effect : ScriptableObject
	{
		[SerializeField] private Sprite _sprite;
		[SerializeField] private string _description;

		public Sprite Sprite { get => _sprite; }
		public string Description { get => _description; }
	}
}
