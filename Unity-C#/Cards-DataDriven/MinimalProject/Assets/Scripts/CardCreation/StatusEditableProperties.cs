using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CardCreation
{
	public class StatusEditableProperties : MonoBehaviour
	{
		[SerializeField] private TMP_Text _effectDescriptionField;
		[SerializeField] private TMP_Text _targetDescriptionField;

		public TMP_Text EffectDescriptionField { get => _effectDescriptionField; }
		public TMP_Text TargetDescriptionField { get => _targetDescriptionField; }
	}
}