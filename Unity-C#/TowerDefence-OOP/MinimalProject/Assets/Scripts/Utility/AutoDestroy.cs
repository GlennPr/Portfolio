using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
	public class AutoDestroy : MonoBehaviour
	{
		public void Initialize(float timeTillSelfDestroy)
		{
			Invoke(nameof(SelfDestroy), timeTillSelfDestroy);
		}

		private void SelfDestroy()
		{
			if (this != null && gameObject != null)
			{
				Destroy(gameObject);
			}
		}
	}
}
