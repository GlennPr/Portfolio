using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobManager : MonoBehaviour
{
    [SerializeField] private List<Transform> _targets;

    private static MobManager _instance;
    private static MobManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<MobManager>();
            }

            return _instance;
        }
    }


    private void Awake()
    {
        _instance = this;    
    }

    public static Transform GetTarget()
	{
        if (Instance == null)
        {
            return null;
        }

        Instance.CheckTargetsValidity();

        if (Instance._targets.Count == 0)
        {
            return null;
        }
        else
        {
            var randomIndex = Mathf.FloorToInt(Random.value * Instance._targets.Count);
            randomIndex = Mathf.Min(Instance._targets.Count, randomIndex);
            return Instance._targets[randomIndex];
        }
    }

    public static Transform Transform()
    {
        return Instance == null ? null : Instance.transform;
    }

    private void CheckTargetsValidity()
	{
        for(int i = _targets.Count - 1; i > -1; i--)
		{
            if(_targets[i] == null)
			{
                _targets.RemoveAt(i);
            }
        }
	}
}
