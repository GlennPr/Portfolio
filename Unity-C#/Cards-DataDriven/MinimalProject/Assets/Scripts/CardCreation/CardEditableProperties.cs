using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CardCreation
{
	public class CardEditableProperties : MonoBehaviour
	{
		[SerializeField] private Image _artworkField;
		[SerializeField] private TMP_Text _nameField;
		[SerializeField] private TMP_Text _descriptionField;
		[SerializeField] private TMP_Text _attackDefenceField;
		[SerializeField] private HorizontalLayoutGroup _typeField;
		[SerializeField] private VerticalLayoutGroup _statusField;

		public Image ArtworkField { get => _artworkField; }
		public TMP_Text NameField { get => _nameField; set => _nameField = value; }
		public TMP_Text DescriptionField { get => _descriptionField; }
		public TMP_Text AttackDefenceField { get => _attackDefenceField; }
		public HorizontalLayoutGroup TypeField { get => _typeField; }
		public VerticalLayoutGroup StatusField { get => _statusField; }
	}
}