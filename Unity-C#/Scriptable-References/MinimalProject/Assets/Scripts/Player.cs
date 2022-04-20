using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Data_Team _team;
    [SerializeField] private MeshRenderer _renderer;

	private void Awake()
	{
		_renderer.material = MaterialManager.GetInstance(_renderer.sharedMaterial, _team);
	}

}
