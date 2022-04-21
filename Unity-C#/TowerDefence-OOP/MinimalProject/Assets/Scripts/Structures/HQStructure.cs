using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HQStructure : Structure
{
	private void OnDestroy()
	{
		Debug.Log("Game Over");
	}
}
