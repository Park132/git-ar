using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
	public GameObject[] Prefabs;
	public GameObject BridgeCube;
	public GameObject imgHP;
	public GameObject canvas;

	private static PrefabManager instance;
	private void Awake()
	{
		if (null == instance)
		{
			instance = this;
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
