using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BridgeManager : MonoBehaviour
{

	/// <summary>
	/// 2022.11.02
	/// 원래 GameManager에서 처리한 다리 생성 구문을 새로운 스크립트를 생성하여 처리
	/// </summary>

	// 싱글톤 /////
	private static BridgeManager instance;
	public List<List<GameObject>> bridgeArr;
	public int bridgeCount;
	private void Awake()
	{
		if (instance == null)
		{ instance = this; }
		else
			Destroy(this.gameObject);
	}
	public static BridgeManager Instance{
		get { return instance; }
	}
	/////

	// 모든 다리를 저장하는 변수 생성
	// 2차원 리스트로 한 다리의 양쪽 지점을 각각 저장할거임.
	

	private void Start()
	{
		bridgeArr = new List<List<GameObject>>();
		bridgeArr.Add(new List<GameObject>());
		bridgeArr.Add(new List<GameObject>());
		bridgeCount = 0;
	}

	public void BridgeCalc(int playerP, int enemyP)
	{
		bridgeArr[0].Clear(); bridgeArr[1].Clear();
		for (int i = 0; i < GameManager.Instance.bridgeObjs.transform.childCount; i++)
			Destroy(GameManager.Instance.bridgeObjs.transform.GetChild(i).gameObject);
		int PEbridgelen = 0;
		for (int i = 0; i < GameManager.Instance.marker.markerLen; i++)
		{ if (GameManager.Instance.marker.markerExist[i] && GameManager.Instance.marker.markerTeam[i] == TEAM.NONE) PEbridgelen++; }
		int[] arrBridge = new int[PEbridgelen];
		float[] arrBridgeDist = new float[PEbridgelen];

		// 플레이어 진영과 중립 사이의 다리 생성
		CreateBridgeCode_for_PE(playerP, ref arrBridge, ref arrBridgeDist, PEbridgelen);
		// 적 진영과 중립 사이의 다리 생성
		CreateBridgeCode_for_PE(enemyP, ref arrBridge, ref arrBridgeDist, PEbridgelen);

		CreateBridge_For_MidlePoints(PEbridgelen);
	}

	// 플레이어, 적의 구문 반복을 줄이기 위한 코드
	private void CreateBridgeCode_for_PE(int P, ref int[] arrBridge, ref float[] arrBridgeDist, int bridgelen)
	{
		DistanceCalc(GameManager.Instance.marker.markerObj[P], P, ref arrBridge, ref arrBridgeDist);
		DistanceSorting(ref arrBridge, ref arrBridgeDist, bridgelen);
		for (int i = 0; i < bridgelen - 1; i++)
			CreateBridge(GameManager.Instance.marker.markerObj[P], GameManager.Instance.marker.markerObj[arrBridge[i]]);
	}

	// 다리 생성
	private void CreateBridge(GameObject SetP1, GameObject SetP2)
	{
		if (!(ReferenceEquals(SetP1, null) || (ReferenceEquals(SetP2, null))))
		{
			GameObject dummy = GameObject.Instantiate(PrefabManager.Instance.BridgeCube);
			dummy.GetComponent<BridgeLook>().SettingP(SetP1, SetP2);
			dummy.transform.parent = GameManager.Instance.bridgeObjs.transform;
			dummy.transform.name = SetP1.name + "_bridge_" + SetP2.name;
			bridgeArr[0].Add(SetP1); bridgeArr[1].Add(SetP2);
			bridgeCount++;
		}
	}

	// 길이 계산. 중립 지형이랑 적,아군의 지형간의 길이 계산
	private void DistanceCalc(GameObject std, int P, ref int[] arrBridge, ref float[] arrBridgeDist)
	{
		int j = 0;
		for (int i = 0; i < GameManager.Instance.marker.markerLen; i++)
		{
			float dummy;
			if (GameManager.Instance.marker.markerTeam[i] == TEAM.NONE && GameManager.Instance.marker.markerExist[i])
			{
				dummy = Vector3.Distance(GameManager.Instance.marker.markerObj[P].transform.position
					, GameManager.Instance.marker.markerObj[i].transform.position);
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
				if (arrBridgeDist[i] < arrBridgeDist[j] && (arrBridgeDist[i] != 0 && arrBridgeDist[j] != 0))
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

	// 중립 지형의 다리생성
	private void CreateBridge_For_MidlePoints(int bridgeLen)
	{
		int maxCreate = 0;
		GameObject[] arrNone = new GameObject[bridgeLen];

		int k = 0;
		// 중립 지형을 저장하는 배열 생성
		for (int i = 0; i < GameManager.Instance.marker.markerLen; i++)
		{
			if (GameManager.Instance.marker.markerTeam[i] == TEAM.NONE && GameManager.Instance.marker.markerExist[i])
			{ arrNone[k] = GameManager.Instance.marker.markerObj[i]; k++; }
		}

		for (int i = 1; i < bridgeLen; i++) { maxCreate += i; }

		GameObject[,] bridgeCreateCases = new GameObject[maxCreate, 2];

		// 모든 다리 생성의 경우의 수를 각각 저장.
		k = 0;
		for (int i = 0; i < bridgeLen - 1; i++)
		{
			for (int j = i + 1; j < bridgeLen; j++)
			{
				bridgeCreateCases[k, 0] = arrNone[i];
				bridgeCreateCases[k, 1] = arrNone[j];
				k++;
			}
		}
		int fixedCreate = UnityEngine.Random.Range(0, maxCreate);

		for (int i = 0; i < maxCreate; i++)
		{
			//if (i == fixedCreate) CreateBridge(bridgeCreateCases[i,0],bridgeCreateCases[i,1]);
			//else if (7 > UnityEngine.Random.Range(0,10))
			CreateBridge(bridgeCreateCases[i, 0], bridgeCreateCases[i, 1]);
		}
	}

	public bool CheckBridge(GameObject P1, GameObject P2)
	{
		for (int i = 0; i < bridgeCount; i++)
		{
			if (bridgeArr[0][i].Equals(P1) || bridgeArr[1][i].Equals(P1))
			{
				if (bridgeArr[0][i].Equals(P2) || bridgeArr[1][i].Equals(P2))
				{ return true; }
			}
		}
		return false;
	}
	public void ConnectedPanChange(GameObject P1, PANSTATE s)
	{
		for (int i = 0; i < bridgeCount; i++)
		{
			if (bridgeArr[0][i].Equals(P1))
				bridgeArr[1][i].GetComponentInChildren<SlimeBaseSC>().ChangeState(s);
			if (bridgeArr[1][i].Equals(P1))
				bridgeArr[0][i].GetComponentInChildren<SlimeBaseSC>().ChangeState(s);
		}
	}
}
