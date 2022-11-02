using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BridgeManager : MonoBehaviour
{

	/// <summary>
	/// 2022.11.02
	/// ���� GameManager���� ó���� �ٸ� ���� ������ ���ο� ��ũ��Ʈ�� �����Ͽ� ó��
	/// </summary>

	// �̱��� /////
	private static BridgeManager instance;
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

	public void BridgeCalc(int playerP, int enemyP)
	{
		int PEbridgelen = 0;
		for (int i = 0; i < GameManager.Instance.marker.markerLen; i++)
		{ if (GameManager.Instance.marker.markerExist[i] && GameManager.Instance.marker.markerTeam[i] == TEAM.NONE) PEbridgelen++; }
		int[] arrBridge = new int[PEbridgelen];
		float[] arrBridgeDist = new float[PEbridgelen];

		// �÷��̾� ������ �߸� ������ �ٸ� ����
		CreateBridgeCode_for_PE(playerP, ref arrBridge, ref arrBridgeDist, PEbridgelen);
		// �� ������ �߸� ������ �ٸ� ����
		CreateBridgeCode_for_PE(enemyP, ref arrBridge, ref arrBridgeDist, PEbridgelen);

		CreateBridge_For_MidlePoints(PEbridgelen);
	}

	// �÷��̾�, ���� ���� �ݺ��� ���̱� ���� �ڵ�
	private void CreateBridgeCode_for_PE(int P, ref int[] arrBridge, ref float[] arrBridgeDist, int bridgelen)
	{
		DistanceCalc(GameManager.Instance.marker.markerObj[P], P, ref arrBridge, ref arrBridgeDist);
		DistanceSorting(ref arrBridge, ref arrBridgeDist, bridgelen);
		for (int i = 0; i < bridgelen - 1; i++)
			CreateBridge(GameManager.Instance.marker.markerObj[P], GameManager.Instance.marker.markerObj[arrBridge[i]]);
	}

	// ������ �ٸ� ����
	private void CreateBridge(GameObject SetP1, GameObject SetP2)
	{
		if (!(ReferenceEquals(SetP1, null) || (ReferenceEquals(SetP2, null))))
		{
			GameObject dummy = GameObject.Instantiate(PrefabManager.Instance.BridgeCube);
			dummy.GetComponent<BridgeLook>().SettingP(SetP1, SetP2);
			dummy.transform.parent = GameManager.Instance.stdPoint.transform;
		}
	}

	// ���� ���. �߸� �����̶� ��,�Ʊ��� �������� ���� ���
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

	// sorting�� ����ϱ� �ָ���. �׷��Ƿ� ���� �Լ� ����
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

	// �߸� ������ �ٸ�����
	private void CreateBridge_For_MidlePoints(int bridgeLen)
	{
		int maxCreate = 0;
		GameObject[] arrNone = new GameObject[bridgeLen];

		int k = 0;
		// �߸� ������ �����ϴ� �迭 ����
		for (int i = 0; i < GameManager.Instance.marker.markerLen; i++)
		{
			if (GameManager.Instance.marker.markerTeam[i] == TEAM.NONE && GameManager.Instance.marker.markerExist[i])
			{ arrNone[k] = GameManager.Instance.marker.markerObj[i]; k++; }
		}

		for (int i = 1; i < bridgeLen; i++) { maxCreate += i; }

		GameObject[,] bridgeCreateCases = new GameObject[maxCreate, 2];

		// ��� �ٸ� ������ ����� ���� ���� ����.
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
}
