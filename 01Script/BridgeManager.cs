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
	public GameObject bridge_obj;
	public int bridgeCount;
	public bool creatingBridge;
	
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

	public IEnumerator BridgeCalc(int playerP, int enemyP)
	{
		creatingBridge = true;
		bridgeArr[0].Clear(); bridgeArr[1].Clear();
		bridgeCount = 0;
		//for (int i = GameManager.Instance.bridgeObjs.transform.childCount-1; i >= 0 ; i--)
		//Destroy(GameManager.Instance.bridgeObjs.transform.GetChild(i).gameObject);
		BridgeLook[] arr_child = GameManager.Instance.bridgeObjs.GetComponentsInChildren<BridgeLook>();
		foreach (BridgeLook tr in arr_child)
		{Destroy(tr.gameObject,0); }

		yield return new WaitForSeconds(0.5f);

		int PEbridgelen = 0;
		for (int i = 0; i < GameManager.Instance.marker.markerLen; i++)
		{ if (GameManager.Instance.marker.markerExist[i] && GameManager.Instance.marker.markerTeam[i] == TEAM.NONE) PEbridgelen++; }
		// 
		int[] arrBridge = new int[PEbridgelen];
		float[] arrBridgeDist = new float[PEbridgelen];

		// 플레이어 진영과 중립 사이의 다리 생성
		CreateBridgeCode_for_PE(playerP, ref arrBridge, ref arrBridgeDist, PEbridgelen);

		// 적 진영과 중립 사이의 다리 생성
		arrBridge = new int[PEbridgelen];
		arrBridgeDist = new float[PEbridgelen];
		CreateBridgeCode_for_PE(enemyP, ref arrBridge, ref arrBridgeDist, PEbridgelen);

		arrBridge = new int[PEbridgelen];
		arrBridgeDist = new float[PEbridgelen];
		CreateBridge_For_MidlePoints(PEbridgelen);
		creatingBridge = false;
	}


	// 플레이어, 적의 구문 반복을 줄이기 위한 코드
	private void CreateBridgeCode_for_PE(int P, ref int[] arrBridge, ref float[] arrBridgeDist, int bridgelen)
	{
		DistanceCalc(GameManager.Instance.marker.markerObj[P], P, ref arrBridge, ref arrBridgeDist);
		DistanceSorting(ref arrBridge, ref arrBridgeDist, bridgelen);
		int j = 0, max_create = Mathf.CeilToInt(bridgelen);
		for (int i = 0; i < bridgelen; i++)
		{
			if (j < max_create)
			{
				if (CheckRay(GameManager.Instance.marker.markerObj[P], GameManager.Instance.marker.markerObj[arrBridge[i]]))
				{
					CreateBridge(GameManager.Instance.marker.markerObj[P], GameManager.Instance.marker.markerObj[arrBridge[i]]);
					j++;
				}
			}
			else break;
		}
	}

	// 다리 체크 -> 레이를 발사해서 그 레이 안에 다른 기지가 존재한다면 다리는 생성 X
	private bool CheckRay(GameObject SetP1, GameObject SetP2)
	{
		if (ReferenceEquals(SetP1, null) || ReferenceEquals(SetP2, null)) return false;

		RaycastHit[] hits;
		float dummy_dist = Vector3.Distance(SetP1.transform.position, SetP2.transform.position);
		//bool hit_check= Physics.BoxCast(SetP1.transform.position, new Vector3(3f, 3f, 3f),
		//(SetP2.transform.position - SetP1.transform.position).normalized, out hit, Quaternion.identity, dummy_dist,3);
		hits = Physics.BoxCastAll(SetP1.transform.position, new Vector3(0.2f, 0.2f, 0.2f),
			(SetP2.transform.position - SetP1.transform.position).normalized, Quaternion.identity, dummy_dist * 0.8f);
		Debug.DrawRay(SetP1.transform.position, (SetP2.transform.position - SetP1.transform.position).normalized * dummy_dist * 0.9f, Color.green, 3, false);
		foreach (RaycastHit hit in hits)
		{
			GameObject dummy = hit.transform.GetComponentInParent<TargetEnable>().gameObject;

			if (!dummy.Equals(SetP1) && !dummy.Equals(SetP2) && !ReferenceEquals(dummy, null))
			{Debug.Log("Hit! hitobj = " +dummy.gameObject +"\nStart = " +SetP1 +"  dest = " +SetP2); return false; }
		}
		
		
		return true;
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

		for (int i = 1; i < bridgeLen; i++) { maxCreate += i; }

		GameObject[,] bridgeCreateCases = new GameObject[maxCreate, 2];

		// 모든 다리 생성의 경우의 수를 각각 저장.
		int k = 0;
		for (int i = 0; i < bridgeLen ; i++)
		{
			for (int j = i + 1; j < bridgeLen; j++)
			{
				bridgeCreateCases[k, 0] = GameManager.Instance.arrNone[i];
				bridgeCreateCases[k, 1] = GameManager.Instance.arrNone[j];
				k++;
			}
		}
		int fixedCreate = UnityEngine.Random.Range(0, maxCreate);

		for (int i = 0; i < maxCreate; i++)
		{
			if (CheckRay(bridgeCreateCases[i,0], bridgeCreateCases[i,1]))
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

	// 클릭 지점과 연결된 지점들의 색을 변경하는 구문
	public void ConnectedPanChange(GameObject P1, PLATESTATE s)
	{
		for (int i = 0; i < bridgeCount; i++)
		{
			GameObject dummy = CheckConnect(P1, i);
			if (!ReferenceEquals(dummy,null))
				dummy.GetComponentInChildren<SlimeBaseSC>().ChangeState(s);
		}
	}

	public void ConnectedPanChange(GameObject P1, PLATESTATE s, bool a)
	{
		ConnectedPanChange(P1, s);
		if (a)
		{
			for (int i = 0; i < bridgeCount; i++)
			{
				GameObject dummy = CheckConnect(P1, i);
				if (!ReferenceEquals(dummy, null))
				{
					string name1 = P1.GetComponentInChildren<SlimeBaseSC>().gameObject.name;
					string name2 = dummy.GetComponentInChildren<SlimeBaseSC>().gameObject.name;
					if (GameManager.Instance.attackObjs.transform.Find(name1 + "_attack_" + name2))
						dummy.GetComponentInChildren<SlimeBaseSC>().ChangeState(PLATESTATE.CANCLE);
					else
						dummy.GetComponentInChildren<SlimeBaseSC>().ChangeState(s);
				}
			}
		}
	}

	// 매개변수로 받아온 P1이 i번째 다리와 연관이 있는지 확인.
	public GameObject CheckConnect(GameObject P1, int i)
	{
		if (bridgeArr[0][i].Equals(P1))
			return bridgeArr[1][i];
		else if (bridgeArr[1][i].Equals(P1))
			return bridgeArr[0][i];
		return null;
	}
}
