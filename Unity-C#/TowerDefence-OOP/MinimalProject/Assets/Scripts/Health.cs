using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float _healthPointsMax = 100;

    private float _healthPoints;

    private void Awake()
    {
        _healthPoints = _healthPointsMax;
    }

    public void TakeDamage(ref IDamageable damageable, float value)
    {
        _healthPoints -= value;

        if (_healthPoints <= 0)
        {
            damageable.Kill();
        }
    }

    public void Restore(float value)
    {
        _healthPoints = Mathf.Max(_healthPointsMax, _healthPoints + value);
    }
}
