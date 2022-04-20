using System;
using System.Globalization;
using TMPro;
using UnityEngine;

namespace References.Components
{
	[RequireComponent(typeof(TMP_Text))]
	public abstract class ReferenceValueLabel<T> : MonoBehaviour
	{
		[SerializeField] private ReferenceValue<T> _valueReference;
		[SerializeField] private string _format = "{0}";
		[SerializeField] private TextFormatting _formatStyle = TextFormatting.StringFormat;

		private TMP_Text _label;

		public enum TextFormatting
		{
			TMPro,
			StringFormat,
		}

		public string Format
		{
			get => _format;
			set
			{
				_format = value;
				UpdateLabel(_valueReference.Value);
			}
		}

		public TMP_Text Label => _label ? _label : _label = GetComponent<TMP_Text>();
		public TextFormatting FormatStyle => _formatStyle;

		private void Start()
		{
			if (_valueReference == null) return;

			SetValueReference(_valueReference);
		}

		public void SetValueReference(ReferenceValue<T> value)
		{
			if (_valueReference != null)
			{
				_valueReference.OnValueChange -= OnValueChanged;
			}

			_valueReference = value;
			UpdateLabel(_valueReference.Value);
			_valueReference.OnValueChange += OnValueChanged;
		}

		private void OnDestroy()
		{
			_valueReference.OnValueChange -= OnValueChanged;
		}

		private void OnValueChanged(T newValue, T oldValue)
		{
			UpdateLabel(newValue);
		}

		public virtual void UpdateLabel(T value)
		{
			switch (_formatStyle)
			{
				case TextFormatting.TMPro:
					switch (value)
					{
						case float val:
							Label.SetText(_format, val);
							break;
						case int val:
							Label.SetText(_format, val);
							break;
						default:
							Label.text = string.Format(CultureInfo.InvariantCulture, _format, value);
							break;
					}

					break;
				case TextFormatting.StringFormat:
					Label.text = string.Format(CultureInfo.InvariantCulture, _format, value);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}