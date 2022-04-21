using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretTower : Tower
{
    [Header("TurretTower")]
    [SerializeField] private float _dmgPerShot;
    [SerializeField] private float _timeBetweenShot;

    private float _nextShootTime;

    protected override void Initialize()
	{
        DetermineNextShootTime();
    }

    private void Update()
    {
        if (Time.time > _nextShootTime)
        {
            var target = GetTarget();
            if (target != null)
            {
                target.TakeDamage(_dmgPerShot);
                PointAt(target.transform);

                DetermineNextShootTime();
            }
        }
    }

    private void DetermineNextShootTime()
    {
        var isFirstStep = Mathf.Approximately(_nextShootTime, 0);
        var overshootCorrection = isFirstStep ? 0 :  Time.time - _nextShootTime;
        _nextShootTime = Time.time + _timeBetweenShot - overshootCorrection;
    }
}
