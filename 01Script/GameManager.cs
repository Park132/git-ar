using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class GameManager : MonoBehaviour
{
	public GameObject stdPoint, enePoint;
	public SlimeBaseSC stdPointSB, enePointSB;
	public GameObject bridgeObjs;
	public GameObject attackObjs;
	public TextMeshProUGUI Counting;
	public List<GameObject> arrNone, arrPlayer, arrEnemy;
	public GAMESTATE gameState;
	public GameObject[] gamePanels;

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
		PS_System.Instance.FadeScene(true);
		Counting.enabled = false;
	}

	private void Update()
	{
		if (gameState == GAMESTATE.START)
		{
			distanceEP = Vector3.Distance(stdPoint.transform.position, enePoint.transform.position);
		}
	}

	// Start BUtton���� ��ư Ŭ�� �� �ٸ� ���� �� ���� ������ ����.
	public void BridgeButton()
	{
		if (BridgeManager.Instance.creatingBridge)
			return;
		if (gameState != GAMESTATE.MAIN && gameState != GAMESTATE.READY)
			return;

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

			StartCoroutine(BridgeManager.Instance.BridgeCalc(playerP,enemyP));

			gameState = GAMESTATE.READY;
			
		}
		else
		{ Debug.Log("��Ŀ�� �� ����ּ�"); }
	}
	public void StartButton()
	{
		if (gameState == GAMESTATE.READY)
		{
			StartCoroutine(StartCoroutine());
		}
	}

	private IEnumerator StartCoroutine()
	{
		yield return StartCoroutine(PS_System.Instance.FadeOutCoroutine());
		distanceEP = Vector3.Distance(stdPoint.transform.position, enePoint.transform.position);
		yield return StartCoroutine(PS_System.Instance.FadeInCoroutine());

		yield return new WaitForSeconds(0.5f);
		Counting.enabled = true;
		for (int i = 3; i >= 1; i--)
		{ Counting.text = i.ToString(); yield return StartCoroutine(CountDownText(Counting.gameObject, 1)); }
		Counting.text = "Start!";
		yield return StartCoroutine(CountDownText(Counting.gameObject, 0.5f));
		Counting.enabled = false;

		gameState = GAMESTATE.START;
		LS_EnemyBaseSC.Instance.StartAI();
	}
	

	public void PausedButton()
	{
		switch (GameManager.instance.gameState)
		{
			case GAMESTATE.START:
				Time.timeScale = 0.0f;
				GameManager.instance.gameState = GAMESTATE.PAUSE;
				break;
			case GAMESTATE.PAUSE:
				Time.timeScale = 1.0f;
				GameManager.instance.gameState = GAMESTATE.START;
				break;
		}
		
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

	private IEnumerator CountDownText(GameObject obj, float time)
	{
		obj.GetComponent<TextMeshProUGUI>().color = new Color(1,1,1,(1-time));
		obj.transform.localScale = Vector3.one * (0.06f * 3);
		for (int i = Mathf.RoundToInt(20/(20*time)); i < 20; i++) {
			yield return new WaitForSeconds(0.05f);
			obj.transform.localScale = Vector3.one * (0.05f *(i+4));
			if (i <= 15)
            {obj.GetComponent<TextMeshProUGUI>().color = new Color(1, 1, 1, (0.5f/15) * i+0.5f);}

		}
	}

	// ���� ����,������ ���� �ǳ� ����
	private IEnumerator GameMenuEnable(GAMESTATE state)
    {
		int index = 0;
        switch (state)
        {
			case GAMESTATE.PAUSE: index = 0;break;
			case GAMESTATE.WIN: index = 1; break;
			case GAMESTATE.OVER: index = 2; break;
		}
		gamePanels[index].SetActive(true);
		yield return new WaitForSeconds(1f);
    }
}
