using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
	public GameObject stdPoint;

	
	public StructorCollector.Markers marker;


	// 싱글톤
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
	}

	public static GameManager Instance { 
		get { return instance; }
	}

	private void Start()
	{
		stdPoint = GameObject.FindGameObjectWithTag("StandardPoint");
		marker.markerObj = new List<GameObject>();
		marker.markerExist = new List<bool>();
		marker.markerTeam = new List<TEAM>();
		marker.markerLen = 0;
	}

	public void BridgeButton()
	{
		int playerP=-1, enemyP=-1, bridgelen = -1;

		// 적군과 아군의 베이스캠프를 확인
		for (int i = 0; i < marker.markerLen; i++)
		{
			if (marker.markerTeam[i] == TEAM.PLAYER) playerP = i;
			else if (marker.markerTeam[i] == TEAM.ENEMY) enemyP = i;
		}
		// 아군 적군 베이스캠프 확인 완료시
		if (playerP != -1 && enemyP != -1)
		{
			bridgelen = Math.Max(marker.markerLen - 2, 1);
			int[] arrBridge = new int[bridgelen];
			float[] arrBridgeDist = new float[bridgelen];

			// 플레이어 진영과 중립 사이의 다리 생성

			if (true)
			{
				DistanceCalc(marker.markerObj[playerP], playerP, ref arrBridge, ref arrBridgeDist, bridgelen);
				DistanceSorting(ref arrBridge, ref arrBridgeDist, bridgelen);
				for (int i = 0; i < bridgelen - 1; i++)
					CreateBridge(marker.markerObj[playerP], marker.markerObj[arrBridge[i]]);
			}

			// 적 진영과 중립 사이의 다리 생성
			if (true)
			{
				DistanceCalc(marker.markerObj[enemyP], enemyP, ref arrBridge, ref arrBridgeDist, bridgelen);
				DistanceSorting(ref arrBridge, ref arrBridgeDist, bridgelen);
				for (int i = 0; i < bridgelen - 1; i++)
					CreateBridge(marker.markerObj[enemyP], marker.markerObj[arrBridge[i]]);
			}


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

	// 물질적 다리 생성
	private void CreateBridge(GameObject SetP1, GameObject SetP2)
	{
		GameObject dummy = GameObject.Instantiate(PrefabManager.Instance.BridgeCube);
		dummy.GetComponent<BridgeLook>().SettingP(SetP1,SetP2);
		dummy.transform.parent = SetP1.transform;		
	}

	// 길이 계산. 중립 지형이랑 적,아군의 지형간의 길이 계산
	private void DistanceCalc(GameObject std, int P ,ref int[] arrBridge, ref float[] arrBridgeDist, int bridgelen)
	{
		int j = 0;
		for (int i = 0; i < marker.markerLen; i++)
		{
			float dummy;
			if (marker.markerTeam[i] == TEAM.NONE)
			{
				dummy = Vector3.Distance(marker.markerObj[P].transform.position
					, marker.markerObj[i].transform.position);
				arrBridge[j] = i;
				arrBridgeDist[j] = dummy;
				j++;
			}
		}
	}

	// sorting을 사용하기 애매함. 그러므로 정렬 함수 제작
	private void DistanceSorting(ref int[] arrBridge, ref float[] arrBridgeDist, int len)
	{
		for (int i = 1; i < len; i++)
		{
			for (int j = 0; j < i; j++)
			{
				if (arrBridgeDist[i] < arrBridgeDist[j])
				{
					float temp_f = arrBridgeDist[i];
					arrBridgeDist[i] = arrBridgeDist[j];
					arrBridgeDist[j] = temp_f;
					int temp_i = arrBridge[i];
					arrBridge[i] = arrBridge[j];
					arrBridge[j] = temp_i;
				}
			}
		}
	}
}
