using References;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team : MonoBehaviour
{
	[SerializeField] private EventTriggerReference _switchSidesEvent;
	[SerializeField] private int _rotation = 0;

	private void Awake()
	{
		_switchSidesEvent.OnTrigger += HandleSideSwitch;
	}

	private void HandleSideSwitch()
	{
		_rotation += 180;
		_rotation %= 360;
	}

	private void Update()
	{
		transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, _rotation, 0), Time.deltaTime * 160);
	}

}
