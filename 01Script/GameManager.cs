using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
	public GameObject stdPoint, enePoint;
	public SlimeBaseSC stdPointSB, enePointSB;
	public GameObject bridgeObjs;
	public GameObject attackObjs;
	public List<GameObject> arrNone, arrPlayer, arrEnemy;
	public GAMESTATE gameState;
	public float distanceEP = 0;
	
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
		gameState = GAMESTATE.MAIN;
	}
	public static GameManager Instance { 
		get { return instance; }
	}
	//////
	
	private void Start()
	{
		stdPoint = GameObject.FindGameObjectWithTag("StandardPoint");
		enePoint = GameObject.FindGameObjectWithTag("EnemyPoint");
		stdPointSB = stdPoint.GetComponentInChildren<SlimeBaseSC>();
		enePointSB = enePoint.GetComponentInChildren<SlimeBaseSC>();
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

	// Start BUtton으로 버튼 클릭 시 다리 생성 및 게임 조작이 가능.
	public void BridgeButton()
	{
		if (gameState != GAMESTATE.MAIN && gameState != GAMESTATE.READY)
			return;

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
			arrNone.Clear(); arrPlayer.Clear(); arrEnemy.Clear();
			for (int i = 0; i < marker.markerLen; i++)
			{
				if (marker.markerTeam[i] == TEAM.NONE && marker.markerExist[i])
				{ arrNone.Add(marker.markerObj[i]); }
				else if (marker.markerTeam[i] == TEAM.PLAYER && marker.markerExist[i])
				{ arrPlayer.Add(marker.markerObj[i]); }
				else if (marker.markerTeam[i] == TEAM.ENEMY && marker.markerExist[i])
				{ arrEnemy.Add(marker.markerObj[i]); }
			}

			BridgeManager.Instance.BridgeCalc(playerP,enemyP);

			gameState = GAMESTATE.READY;
			
		}
		else
		{ Debug.Log("마커를 더 찍어주셈"); }
	}
	public void StartButton()
	{
		if (gameState == GAMESTATE.READY)
		{
			gameState = GAMESTATE.START;
			LS_EnemyBaseSC.Instance.StartAI();
		}
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
