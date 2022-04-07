using UnityEngine;

namespace Data
{
	[CreateAssetMenu(fileName = "Card", menuName = "ScriptableObjects/Card", order = 1)]
	public class Data_Card : ScriptableObject
	{
		[SerializeField] private Sprite _sprite;
		[SerializeField] private string _description;
		[Space]
		[SerializeField] private int _attack;
		[SerializeField] private int _defence;
		[Space]
		[SerializeField] private Data_Type[] _types = new Data_Type[1];
		[SerializeField] private StatusInfo[] _statuses = new StatusInfo[0];

		public Sprite Sprite { get => _sprite; }
		public string Description { get => _description; }
		public int Attack { get => _attack; }
		public int Defence { get => _defence; }
		public Data_Type[] Types { get => _types; }
		public StatusInfo[] Statuses { get => _statuses; }


		[System.Serializable]
		public struct StatusInfo
		{
			[SerializeField] private int _amount;
			[SerializeField] private Data_Effect _status;
			[SerializeField] private Data_EffectTarget[] _targets;

			public int Amount { get => _amount; }
			public Data_Effect Status { get => _status; }
			public Data_EffectTarget[] Targets { get => _targets; }
		}
	}
}
