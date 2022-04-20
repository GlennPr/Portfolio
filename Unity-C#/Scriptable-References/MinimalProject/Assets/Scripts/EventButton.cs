using References;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class EventButton : MonoBehaviour
{
    [SerializeField] private EventTriggerReference _event;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(_event.Trigger);
    }
}
