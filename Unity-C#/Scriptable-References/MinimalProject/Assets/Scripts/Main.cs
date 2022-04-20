using References;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    [SerializeField] private MainReference _reference;
    [Space]
    [Space]
    [SerializeField, Range(2, 10)] private int _switchSideGoalsCount = 3;
    [SerializeField] private IntReferenceValue _totalGoalCount;
    [Space]
    [SerializeField] private EventTriggerReference _goalScoredEvent;
    [SerializeField] private EventTriggerReference _switchSidesEvent;
    [Space]
    [SerializeField] private ParticleSystem _confetti;
    

    private void Awake()
    {
        _reference.SetValue(this);

        _totalGoalCount.OnValueChange += HandleGoalCountChange;
        _totalGoalCount.SetOutputFilter((x) => x % _switchSideGoalsCount);

        _goalScoredEvent.OnTrigger += HandleGoalScored;
    }

    private void HandleGoalScored()
    {
        _totalGoalCount.Value = _totalGoalCount.UnfilteredValue + 1;
        _confetti.Play();
    }

    private void HandleGoalCountChange(int newValue, int oldValue)
	{
        if(oldValue > newValue)
		{
            _switchSidesEvent.Trigger();
        }
	}
}
