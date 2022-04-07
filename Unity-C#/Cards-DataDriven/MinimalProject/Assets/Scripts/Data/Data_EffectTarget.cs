using UnityEngine;

namespace Data
{
	[CreateAssetMenu(fileName = "EffectTarget", menuName = "ScriptableObjects/EffectTarget", order = 1)]
	public class Data_EffectTarget : ScriptableObject
	{
		[SerializeField] private string description;

		public string Description { get => description; }
	}
}