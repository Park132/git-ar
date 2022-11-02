using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
	public GameObject stdPoint;
	public GAMESTATE gameState;
	
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
		marker.markerObj = new List<GameObject>();
		marker.markerExist = new List<bool>();
		marker.markerTeam = new List<TEAM>();
		marker.markerLen = 0;
	}

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
