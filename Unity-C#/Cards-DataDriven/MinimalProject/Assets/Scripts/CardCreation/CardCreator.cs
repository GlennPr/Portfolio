using Data;
using Gameplay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CardCreation
{
	public class CardCreator : MonoBehaviour
	{
		[SerializeField] private CardEditableProperties _cardPrefab;
		[SerializeField] private StatusEditableProperties _cardStatusPrefab;
		[SerializeField] private Image _typePrefab;
		[SerializeField] private Data_Card _data;

#if UNITY_EDITOR
		[ContextMenu("Create")]
		public void Editor_Create()
		{
			Create(_data);
		}
#endif

		public Card Create(Data_Card data)
		{
			var card = Instantiate(_cardPrefab);

			card.ArtworkField.sprite = data.Sprite;
			card.AttackDefenceField.text = string.Format("{0} / {1}", data.Attack, data.Defence);
			card.NameField.text = data.name;
			card.DescriptionField.text = data.Description;

			for (int i = 0; i < data.Types.Length; i++)
			{
				var typeImage = Instantiate(_typePrefab, card.TypeField.transform);
				var typeData = data.Types[i];

				typeImage.sprite = data.Types[i].Sprite;
				typeImage.color = typeData.Color;
			}
			LayoutRebuilder.ForceRebuildLayoutImmediate(card.TypeField.transform as RectTransform);

			if (data.Statuses.Length == 0)
			{
				var status = Instantiate(_cardStatusPrefab, card.StatusField.transform);
				status.EffectDescriptionField.text = "No special effect";
			}
			else
			{
				for (int i = 0; i < data.Statuses.Length; i++)
				{
					var status = Instantiate(_cardStatusPrefab, card.StatusField.transform);
					var statusData = data.Statuses[i];

					status.EffectDescriptionField.text = statusData.Status.Description;

					var targets = "This effect is applied to: ";
					for (int j = 0; j < statusData.Targets.Length; j++)
					{
						var first = j == 0;
						var spacing = first ? "" : ", ";

						targets += string.Format(spacing + "<b>{0}</b>", statusData.Targets[j].Description);
					}
					status.TargetDescriptionField.text = targets;
				}
			}
			LayoutRebuilder.ForceRebuildLayoutImmediate(card.StatusField.transform as RectTransform);

			return card.GetComponent<Card>();
		}
	}
}