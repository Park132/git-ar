using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
	public GameObject stdPoint;
	public GAMESTATE gameState;
	
	public StructorCollector.Markers marker;


	// 싱글톤 /////
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

		// 적군과 아군의 베이스캠프를 확인
		for (int i = 0; i < marker.markerLen; i++)
		{
			if (marker.markerExist[i])
			{
				if (marker.markerTeam[i] == TEAM.PLAYER) playerP = i;
				else if (marker.markerTeam[i] == TEAM.ENEMY) enemyP = i;
			}
		}

		// 아군 적군 베이스캠프 확인 완료시
		if (playerP != -1 && enemyP != -1)
		{
			BridgeManager.Instance.BridgeCalc(playerP,enemyP);

			gameState = GAMESTATE.START;
		}
		else
		{ Debug.Log("마커를 더 찍어주셈"); }
	}

	// 마커 인식 시 한번만 실행하게 제작.
	// 구조체에 각 오브젝트, 인식, 팀을 저장
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
