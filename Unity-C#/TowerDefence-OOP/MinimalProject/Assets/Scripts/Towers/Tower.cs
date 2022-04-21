using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class Tower : MonoBehaviour, IDamageable
{
    private enum TargetSelectionMode
    {
        Closest,
        Furthest,
        Random
	}

    [Header("Tower", order = 0)]
    [Header("Stats", order = 1)]
    [SerializeField] private float _rangeRadius;

    [Header("Settings")]
    [SerializeField] private TargetSelectionMode _selectionMode = TargetSelectionMode.Closest;

    [Header("References")]
    [SerializeField] private SphereCollider _rangeTrigger;
    [SerializeField] protected Transform _head;

    private List<Mob> _mobsInRange = new List<Mob>();

    private void Awake()
    {
        _rangeTrigger.radius = _rangeRadius;

        Initialize();
    }

	private void OnTriggerEnter(Collider other)
	{
        var mob = other.GetComponent<Mob>();
        if(mob != null && _mobsInRange.Contains(mob) == false)
		{
            _mobsInRange.Add(mob);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var mob = other.GetComponent<Mob>();
        if (mob != null && _mobsInRange.Contains(mob))
        {
            _mobsInRange.Remove(mob);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
	{
        DrawGizmo();
    }
#endif

    public void TakeDamage(float value) // attackable but indestructible 
    {
        // spawn particle face with a grin ;)
    }

    public void Kill()
    {
        Destroy(gameObject);
    }



    protected abstract void Initialize();
    protected virtual void DrawGizmo()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _rangeRadius);
    }


    protected Mob GetTarget()
    {
        CheckMobsValidity();

        if (_mobsInRange.Count > 0)
        {
            switch(_selectionMode)
			{
                default:
                case TargetSelectionMode.Closest:
                
                    return _mobsInRange.OrderBy(x => Vector3.Distance(x.transform.position, transform.position)).First();
                
                case TargetSelectionMode.Furthest:
                    
                    return _mobsInRange.OrderBy(x => Vector3.Distance(x.transform.position, transform.position)).Last();
                
                case TargetSelectionMode.Random:

                    var randomIndex = Mathf.FloorToInt(Random.value * _mobsInRange.Count);
                    randomIndex = Mathf.Min(_mobsInRange.Count, randomIndex);
                    return _mobsInRange[randomIndex];
            }     
        }
        else
        {
            return null;
        }
    }

    protected void PointAt(Transform t)
	{
        if(_head != null)
		{
            _head.LookAt(t);
		}
	}

    private void CheckMobsValidity()
    {
        for (int i = _mobsInRange.Count - 1; i > -1; i--)
        {
            if (_mobsInRange[i] == null)
            {
                _mobsInRange.RemoveAt(i);
            }
        }
    }
}
