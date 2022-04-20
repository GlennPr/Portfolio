using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions;

namespace References
{
	public class ReferenceValue<T> : ReferenceValueBase
	{
		[SerializeField] private T _initialValue;

		private T _value;

		public delegate void ValueHasChangedDelegate(T newValue, T oldValue);

		public event ValueHasChangedDelegate OnValueChange;

		public T InitialValue
		{
			get => _initialValue;
			set => _initialValue = value;
		}

		public delegate T FilterDelegate(T value);

		public static readonly FilterDelegate NoFilter = value => value;

		private FilterDelegate _inputFilter = NoFilter;
		private FilterDelegate _outputFilter = NoFilter;

		protected virtual void OnEnable()
		{
			ResetState();
		}

		private void OnDisable()
		{
			ResetState();
		}

		public T Value
		{
			get => _outputFilter(_value);

			set
			{
				if (OnValueChange == null)
				{
					// No listeners, skip equality check
					_value = _inputFilter(value);
					return;
				}

				var oldValue = Value;
				_value = _inputFilter(value);

				var outputFilteredValue = Value;

				if (!EqualityComparer<T>.Default.Equals(outputFilteredValue, oldValue))
				{
#if DEBUG_MODE
			Debug.Log("<i>Reference Value Invoked</i> : "+ name, this);
#endif
					OnValueChange.Invoke(outputFilteredValue, oldValue);
				}
			}
		}

		public T UnfilteredValue
		{
			get => _value;

			set
			{
				if (EqualityComparer<T>.Default.Equals(value, _value)) return;

				var oldValue = _value;
				_value = value;

				OnValueChange?.Invoke(_value, oldValue);
			}
		}

		public virtual void ResetValue()
		{
			Value = _initialValue;
		}

		public void SetInputFilter([NotNull] FilterDelegate filter)
		{
			Assert.IsNotNull(filter);
			_inputFilter = filter;
		}

		public void SetOutputFilter([NotNull] FilterDelegate filter)
		{
			Assert.IsNotNull(filter);
			_outputFilter = filter;
		}

		public override void ResetState()
		{
			_inputFilter = NoFilter;
			_outputFilter = NoFilter;
			OnValueChange = null;
			_value = _initialValue;
		}
	}
}