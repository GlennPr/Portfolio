using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    public class PrefabSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _prefab;
        [SerializeField] private Vector2 _spawnDelayRange;
        [Header("Optional")]
        [SerializeField] private Bounds2D _bounds;

        [SerializeField] private float _nextSpawnTime;

        private void Start()
        {
            DetermineNextSpawnTime();
        }

        private void Update()
        {
            if (Time.time > _nextSpawnTime)
            {
                var pos = transform.position;
                if (_bounds != null)
                {
                    pos = _bounds.GetRandomPoint();
                }

                Instantiate(_prefab, pos, Quaternion.identity);
                DetermineNextSpawnTime();
            }
        }

        private void DetermineNextSpawnTime()
        {
            var overshootCorrection = Time.time - _nextSpawnTime;
            _nextSpawnTime = Time.time + Mathf.Lerp(_spawnDelayRange.x, _spawnDelayRange.y, Random.value) - overshootCorrection;
        }
    }
}
