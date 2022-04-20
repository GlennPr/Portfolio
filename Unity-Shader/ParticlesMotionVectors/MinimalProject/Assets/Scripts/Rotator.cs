using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] private float _speed = 90;
    [SerializeField] private Vector3 _axis = Vector3.up;

    private void Update()
    {
        transform.Rotate(_axis.normalized, _speed * Time.deltaTime);
    }
}
