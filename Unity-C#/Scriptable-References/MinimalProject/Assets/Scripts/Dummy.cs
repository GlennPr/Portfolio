using References;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour
{
    [SerializeField] private MainReference _reference;

    private void Start()
    {
        if (_reference.HasValue)
        {
            Debug.Log("A reference to Main " + _reference.Value, _reference.Value.gameObject);
        }
    }


}
