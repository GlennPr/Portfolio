using System;

namespace References
{
	public class ReferenceObject<T> : ReferenceValueBase where T : class
	{
		private T _value;

		public T Value
		{
			get
			{
				if (_value == null)
				{
					throw new Exception($"Value has not been set: {name}");
				}

				return _value;
			}
		}

		public bool HasValue => _value != null;

		public void SetValue(T value)
		{
			if (_value != null)
			{
				throw new Exception($"Value has already been set: {name}");
			}

			_value = value;
		}

		public void ClearValue()
		{
			_value = null;
		}
	}

	public static class ReferenceObjectExtensions
	{
		public static bool IsSet<T>(this ReferenceObject<T> reference) where T : class
		{
			return reference != null && reference.HasValue;
		}
	}
}