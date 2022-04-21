using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class Mob : MonoBehaviour, IDamageable
{
    [Header("Settings", order = 0)]
    [Header("Movement", order = 1)]
    [SerializeField] private float _stepDelay = 0.1f;
    [SerializeField] private float _stepDuration = 1;
    [SerializeField] private float _stepSize = 0.25f;
    [SerializeField] private float _stepHeight = 0.25f;

    [Header("Stats")]
    [SerializeField] private Health _health;
    [SerializeField] private float _dmg = 10;

    [Header("References")]
    [SerializeField] private ParticleSystem _onDeathParticles;

    private Transform _target;

    private float _nextStepTime;
    private Vector3 _stepStartPos;
    private Vector3 _stepEndPos;

    private void Start()
    {
        SetTarget();

        if (_target != null)
        {
            SetupStep();
        }
    }

    private void Update()
    {
        if (Time.time > _nextStepTime)
        {
            var progress = (Time.time - _nextStepTime) / _stepDuration;
            progress = Mathf.Clamp01(progress);

            transform.position = Vector3.Lerp(_stepStartPos, _stepEndPos, progress);
            transform.position += Vector3.up * _stepHeight * Ease.EaseOutQuad(1 - Mathf.Abs(progress * 2 - 1));

            if (Mathf.Approximately(progress, 1))
			{
                var isAtTarget = _target == null ? false : Vector3.Distance(_target.position, transform.position) <= Mathf.Epsilon;
                if (isAtTarget)
                {
                    var damageable = _target.GetComponent<IDamageable>();
                    if(damageable != null)
					{
                        damageable.TakeDamage(_dmg);
                    }

                    Kill();
                }
                else
                {
                    SetupStep();
                }
            }
        }
    }

	public void TakeDamage(float value)
	{
        var damageable = this as IDamageable;
        _health.TakeDamage(ref damageable, value);
    }

    public void Kill()
	{
        if (_onDeathParticles != null)
        {
            _onDeathParticles.transform.SetParent(MobManager.Transform());
            _onDeathParticles.gameObject.SetActive(true);
            _onDeathParticles.Play();

            _onDeathParticles.gameObject.AddComponent<AutoDestroy>().Initialize(_onDeathParticles.main.duration + 1);
        }

        Destroy(gameObject);
    }

    private void SetTarget()
    {
        _target = MobManager.GetTarget();
        if (_target == null)
        {
            Kill();
        }
    }


    private void SetupStep()
	{
        if(_target == null)
		{
            SetTarget();
        }

        if (_target == null)
        {
            return;
        }

        _stepStartPos = transform.position;
        _stepStartPos.y = 0;

        var delta = _target.transform.position - transform.position;
        var dir = delta.normalized;
        dir.y = 0;
        dir = dir.normalized;

        _stepEndPos = _stepStartPos + dir * Mathf.Min(_stepSize, delta.magnitude);

        transform.LookAt(transform.position + dir);

        DetermineStepTime();
    }


    private void DetermineStepTime()
    {
        var isFirstStep = Mathf.Approximately(_nextStepTime, 0);

        var overshootCorrection = isFirstStep ? 0 :  Time.time - (_nextStepTime + _stepDuration);
        _nextStepTime = Time.time + _stepDelay - overshootCorrection;
    }
}
