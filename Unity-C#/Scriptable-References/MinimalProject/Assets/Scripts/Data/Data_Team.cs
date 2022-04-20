using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Team", menuName = "ScriptableObjects/Data/Team", order = 1)]
public class Data_Team : ScriptableObject
{
	[SerializeField] private Color _color = Color.white;
	public Color Color { get => _color;}
}
