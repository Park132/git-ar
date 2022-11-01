using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
	public GameObject[] Prefabs;
	public GameObject BridgeCube;

	private static PrefabManager instance;
	private void Awake()
	{
		if (null == instance)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
}
		else
		{ Destroy(this.gameObject); }
	}

	public static PrefabManager Instance { 
		get { 

			return instance; 
		}
	}
	
}
