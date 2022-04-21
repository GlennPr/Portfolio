using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    [SerializeField] private Health _health;

    public void TakeDamage(float value)
    {
        var damageable = this as IDamageable;
        _health.TakeDamage(ref damageable, value);
    }

    public void Kill()
    {
        Destroy(gameObject);
    }
}
