using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
	public GameObject stdPoint, enePoint;
	public GameObject bridgeObjs;
	public GameObject attackObjs;
	public List<GameObject> arrNone;
	public GAMESTATE gameState;
	public float distanceEP = 0;
	
	public StructorCollector.Markers marker;


	// �̱��� /////
    private static GameManager instance;
	private void Awake()
	{
		if (null == instance)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{ Destroy(this.gameObject); }
		gameState = GAMESTATE.READY;
	}
	public static GameManager Instance { 
		get { return instance; }
	}
	//////
	
	private void Start()
	{
		stdPoint = GameObject.FindGameObjectWithTag("StandardPoint");
		enePoint = GameObject.FindGameObjectWithTag("EnemyPoint");
		marker.markerObj = new List<GameObject>();
		marker.markerExist = new List<bool>();
		marker.markerTeam = new List<TEAM>();
		arrNone = new List<GameObject>();
		marker.markerLen = 0;
	}

	private void Update()
	{
		if (gameState == GAMESTATE.START)
		{
			distanceEP = Vector3.Distance(stdPoint.transform.position, enePoint.transform.position);
		}
	}

	// Start BUtton���� ��ư Ŭ�� �� �ٸ� ���� �� ���� ������ ����.
	public void StartButton()
	{
		int playerP = -1, enemyP = -1;

		// ������ �Ʊ��� ���̽�ķ���� Ȯ��
		for (int i = 0; i < marker.markerLen; i++)
		{
			if (marker.markerExist[i])
			{
				if (marker.markerTeam[i] == TEAM.PLAYER) playerP = i;
				else if (marker.markerTeam[i] == TEAM.ENEMY) enemyP = i;
			}
		}

		// �Ʊ� ���� ���̽�ķ�� Ȯ�� �Ϸ��
		if (playerP != -1 && enemyP != -1)
		{
			arrNone.Clear();
			for (int i = 0; i < marker.markerLen; i++)
			{
				if (marker.markerTeam[i] == TEAM.NONE && marker.markerExist[i])
				{ arrNone.Add(marker.markerObj[i]); }
			}

			BridgeManager.Instance.BridgeCalc(playerP,enemyP);

			gameState = GAMESTATE.START;
		}
		else
		{ Debug.Log("��Ŀ�� �� ����ּ�"); }
	}

	// ��Ŀ �ν� �� �ѹ��� �����ϰ� ����.
	// ����ü�� �� ������Ʈ, �ν�, ���� ����
	public int SettingMarkerList(GameObject obj, TEAM t)
	{
		int markerlen = marker.markerObj.Count;

		marker.markerObj.Add(obj);
		marker.markerExist.Add(true);
		marker.markerTeam.Add(t);
		marker.markerLen = markerlen + 1;
		return markerlen;
	}
}
