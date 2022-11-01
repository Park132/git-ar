using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
	public GameObject stdPoint;

	
	public StructorCollector.Markers marker;


	// �̱���
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

		// ������ �Ʊ��� ���̽�ķ���� Ȯ��
		for (int i = 0; i < marker.markerLen; i++)
		{
			if (marker.markerTeam[i] == TEAM.PLAYER) playerP = i;
			else if (marker.markerTeam[i] == TEAM.ENEMY) enemyP = i;
		}
		// �Ʊ� ���� ���̽�ķ�� Ȯ�� �Ϸ��
		if (playerP != -1 && enemyP != -1)
		{
			bridgelen = Math.Max(marker.markerLen - 2, 1);
			int[] arrBridge = new int[bridgelen];
			float[] arrBridgeDist = new float[bridgelen];

			// �÷��̾� ������ �߸� ������ �ٸ� ����

			if (true)
			{
				DistanceCalc(marker.markerObj[playerP], playerP, ref arrBridge, ref arrBridgeDist, bridgelen);
				DistanceSorting(ref arrBridge, ref arrBridgeDist, bridgelen);
				for (int i = 0; i < bridgelen - 1; i++)
					CreateBridge(marker.markerObj[playerP], marker.markerObj[arrBridge[i]]);
			}

			// �� ������ �߸� ������ �ٸ� ����
			if (true)
			{
				DistanceCalc(marker.markerObj[enemyP], enemyP, ref arrBridge, ref arrBridgeDist, bridgelen);
				DistanceSorting(ref arrBridge, ref arrBridgeDist, bridgelen);
				for (int i = 0; i < bridgelen - 1; i++)
					CreateBridge(marker.markerObj[enemyP], marker.markerObj[arrBridge[i]]);
			}


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

	// ������ �ٸ� ����
	private void CreateBridge(GameObject SetP1, GameObject SetP2)
	{
		GameObject dummy = GameObject.Instantiate(PrefabManager.Instance.BridgeCube);
		dummy.GetComponent<BridgeLook>().SettingP(SetP1,SetP2);
		dummy.transform.parent = SetP1.transform;		
	}

	// ���� ���. �߸� �����̶� ��,�Ʊ��� �������� ���� ���
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

	// sorting�� ����ϱ� �ָ���. �׷��Ƿ� ���� �Լ� ����
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
