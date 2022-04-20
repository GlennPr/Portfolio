using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamVisuals : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private Data_Team _teamAData;
    [SerializeField] private Data_Team _teamBData;
    [Header("Materials")]
    [SerializeField] private Material _playerUniform;

    private void Start()
    {
        MaterialManager.GetInstance(_playerUniform, _teamAData).color = _teamAData.Color;
        MaterialManager.GetInstance(_playerUniform, _teamBData).color = _teamBData.Color;
    }

}
