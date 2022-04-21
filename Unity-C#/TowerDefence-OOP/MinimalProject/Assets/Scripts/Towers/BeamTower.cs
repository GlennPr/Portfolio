using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BeamTower : Tower
{
    [Header("BeamTower")]
    [SerializeField] private float _dmgPerSecond;

    private LineRenderer _line;

    protected override void Initialize() 
    {
        _line = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        var target = GetTarget();
        if (target != null)
        {
            target.TakeDamage(_dmgPerSecond * Time.deltaTime);

            _line.enabled = true;
            _line.SetPosition(0, _head.position);
            _line.SetPosition(1, target.transform.position);
        }
        else
		{
            _line.enabled = false;
        }
    }
}
