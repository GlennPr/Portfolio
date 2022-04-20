namespace References
{
	public class TypedEventTriggerReference<T> : ReferenceValueBase
	{
		public event System.Action<T> WasTriggered;

		protected void OnEnable()
		{
			WasTriggered = null;
		}

		private void OnDisable()
		{
			WasTriggered = null;
		}

		public void Trigger(T value)
		{
			WasTriggered?.Invoke(value);
		}
	}
}