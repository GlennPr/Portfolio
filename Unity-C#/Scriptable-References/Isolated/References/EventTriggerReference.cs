using UnityEngine;

namespace References
{
	[CreateAssetMenu(menuName = "References/Trigger")]
	public class EventTriggerReference : ScriptableObject
	{
		public event System.Action OnTrigger;

		protected void OnEnable()
		{
			OnTrigger = null;
		}

		private void OnDisable()
		{
			OnTrigger = null;
		}

		public void Trigger()
		{
#if DEBUG_MODE
			Debug.Log("<i>Reference Event Trigger</i> : "+ name, this);
#endif
			OnTrigger?.Invoke();
		}
	}
}