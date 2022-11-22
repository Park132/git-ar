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
	public GameObject errorObj;
	public GameObject defaultSkillObj;
	public TextMeshProUGUI Counting;
	public List<GameObject> arrNone, arrPlayer, arrEnemy;
	public GAMESTATE gameState;
	public GameObject[] gamePanels;
	public GameObject beforeGameObjects, endGameObjects;

	public float distanceEP = 0;
	public float delaySkill = 0;
	
	public StructorCollector.Markers marker;


	// 싱글톤 /////
    private static GameManager instance;
	private void Awake()
	{
		if (null == instance)
		{
			instance = this;
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
		gameState = GAMESTATE.MAIN;
		stdPoint = GameObject.FindGameObjectWithTag("StandardPoint");
		enePoint = GameObject.FindGameObjectWithTag("EnemyPoint");
		stdPointSB = stdPoint.GetComponentInChildren<SlimeBaseSC>();
		enePointSB = enePoint.GetComponentInChildren<SlimeBaseSC>();
		marker.markerObj = new List<GameObject>();
		marker.markerExist = new List<bool>();
		marker.markerTeam = new List<TEAM>();
		arrNone = new List<GameObject>();
		marker.markerLen = 0;

		Counting.enabled = false;
		endGameObjects.SetActive(false);
		errorObj.SetActive(false);
		defaultSkillObj.SetActive(false);
	}

	private void Update()
	{
		if (gameState == GAMESTATE.START)
		{
			distanceEP = Vector3.Distance(stdPoint.transform.position, enePoint.transform.position);

			if (LS_TimerSC.Instance.timer - delaySkill > StructorCollector.BASESKILLTIMES && gameState == GAMESTATE.START)
			{
				delaySkill = LS_TimerSC.Instance.timer;
				gameState = GAMESTATE.SKILLTIME;
				Time.timeScale = 0;
				defaultSkillObj.SetActive(true);
				defaultSkillObj.GetComponent<LS_DefaultSkill>().ActiveSkillObj();
			}
		}
	}

	// Start BUtton으로 버튼 클릭 시 다리 생성 및 게임 조작이 가능.
	public void BridgeButton()
	{
		if (BridgeManager.Instance.creatingBridge)
			return;
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
		if (playerP != -1 && enemyP != -1 && marker.markerLen >= 4)
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
		{ StartCoroutine(ErrorToast("마커를 더 등록해주세요!")); }
	}
	public void StartButton()
	{
		if (gameState == GAMESTATE.READY)
		{
			gameState = GAMESTATE.READYFORSTART;
			StartCoroutine(StartCoroutine());
		}
		else
			StartCoroutine(ErrorToast("다리를 먼저 생성해주세요!"));
	}

	private IEnumerator StartCoroutine()
	{
		yield return StartCoroutine(PS_System.Instance.FadeOutCoroutine(1.5f));
		distanceEP = Vector3.Distance(stdPoint.transform.position, enePoint.transform.position);
		beforeGameObjects.SetActive(false);
		endGameObjects.SetActive(true);
		yield return StartCoroutine(PS_System.Instance.FadeInCoroutine(1.5f));

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
		if (GameManager.instance.gameState == GAMESTATE.START || GameManager.instance.gameState == GAMESTATE.PAUSE)
		{
			switch (GameManager.instance.gameState)
			{
				case GAMESTATE.START:
					StartCoroutine(GameMenuEnable(GAMESTATE.PAUSE, true));
					GameManager.instance.gameState = GAMESTATE.PAUSE;
					break;
				case GAMESTATE.PAUSE:
					StartCoroutine(GameMenuEnable(GAMESTATE.PAUSE, false));
					Time.timeScale = 1.0f;
					GameManager.instance.gameState = GAMESTATE.START;
					break;
			}
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

	private IEnumerator CountDownText(GameObject obj, float time)
	{
		obj.GetComponent<TextMeshProUGUI>().color = new Color(1,1,1,(1-time));
		obj.transform.localScale = Vector3.one * (0.06f * 3);
		for (int i = Mathf.RoundToInt(50/(50*time)); i < 50; i++) {
			yield return new WaitForSeconds(0.02f);
			obj.transform.localScale = Vector3.one * (0.02f *(i+10));
			if (i <= 43)
            {obj.GetComponent<TextMeshProUGUI>().color = new Color(1, 1, 1, (0.8f/43) * i+0.2f);}

		}
	}

	public void GameEnding(TEAM t)
    {
        switch (t)
        {
			case TEAM.PLAYER:
				gameState = GAMESTATE.OVER;
				break;
			case TEAM.ENEMY:
				gameState = GAMESTATE.WIN;
				break;
        }
		StartCoroutine(GameMenuEnable(gameState, true));
	}

	// 게임 종료,정지에 대한 판넬 띄우기
	private IEnumerator GameMenuEnable(GAMESTATE state, bool toggle)
    {
		yield return new WaitForSeconds(0.1f);
		int index = 0;
        switch (state)
        {
			case GAMESTATE.PAUSE: index = 0;break;
			case GAMESTATE.WIN: index = 1; break;
			case GAMESTATE.OVER: index = 2; break;
		}
		gamePanels[index].SetActive(toggle);
		yield return new WaitForSeconds(0.2f);
		if (state != GAMESTATE.PAUSE)
        {
			endGameObjects.SetActive(false);
			Time.timeScale = 0;
        }
        else
        {
			if (toggle) Time.timeScale = 0.0f;
			else Time.timeScale = 1.0f;
		}
    }

	private IEnumerator ErrorToast(string msg)
    {
		TextMeshProUGUI t = errorObj.GetComponentInChildren<TextMeshProUGUI>();
		errorObj.SetActive(true);
		t.text = msg;
		yield return new WaitForSeconds(0.8f);
		errorObj.SetActive(false);
    }

	public void SkillSelected()
	{
		gameState = GAMESTATE.START;
		Time.timeScale = 1;
	}
}
