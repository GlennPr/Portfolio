using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// Get an instanced material back based on its connection to an 'object', acting as the identifier.
/// 
/// when providing no object, it will return a new material each time
/// 
/// </summary>
public static class MaterialManager
{
	private static Dictionary<Material, Dictionary<object, Material>> _materialMapping = new Dictionary<Material, Dictionary<object, Material>>();

    public static Material GetInstance(Material mat, Object key = null)
	{
		if(mat == null)
		{
			Debug.LogError("material provided is null");
			return null;
		}

		if (key == null)
		{
			return MaterialClone(mat);
		}
		else
		{
			if (_materialMapping.ContainsKey(mat) == false)
			{
				_materialMapping.Add(mat, new Dictionary<object, Material>());
			}

			var mappings = _materialMapping[mat];
			if (mappings.ContainsKey(key) == false)
			{
				mappings.Add(key, ManagedMaterialClone(mat));
			}

			return mappings[key];
		}
	}

	private static Material ManagedMaterialClone(Material mat)
	{
		var m = new Material(mat);
		m.name += "-Managed-Clone";
		return m;
	}

	private static Material MaterialClone(Material mat)
	{
		var m = new Material(mat);
		m.name += "-Clone";
		return m;
	}
}
