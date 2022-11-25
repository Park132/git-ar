using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBaseSC : Slime_Stat
{
    public PLATESTATE clickState;
	public GameObject[] bases;
	public float rechargeDelay, woundRechargeDelay, difficultyMultipler; // 속도
	public int rechargeHP; // 양
	private TEAM finalAttack;
	public bool canChanged = false;
	private IEnumerator recharging, recoverRecharge, bloodingCalc;
	public MeshRenderer plate;
	public GameObject area;
	private GameObject sweatyObj;
	private Color32[] arrColor;
	[SerializeField]private int nearCount, currentNearCount;
	public List<GameObject> atkObj;	// 이 베이스에서 출발하는 모든 공격명령 저장
	private float multipleNum;
	public float[] arrSAD;
	float speed, del, setting_prevTimer;
	

	private int previousHP;
	public float regenePerSec = 0f;
	

	protected override void Start()
	{
		arrSAD = new float[] { 1,1,1};
		difficultyMultipler = 1f;
		base.Start();
		if (this.state == TEAM.NONE) canChanged = true;
		recharging = ReChargeSlime(); StartCoroutine(recharging);
		bloodingCalc = BloodingCalc(); StartCoroutine(bloodingCalc);
		recoverRecharge = RecoverDelay();

		multipleNum = 1;
		rechargeHP = 1;
		previousHP = this.Health;
		rechargeDelay = StructorCollector.BASERECHARGEDELAY;
		atkObj = new List<GameObject>();
		arrColor = new Color32[] { new Color32(43, 97, 19, 255),
		new Color32(102, 188, 66, 255),new Color32(50, 152, 186, 255)
		,new Color32(210, 198, 54, 255), new Color32(174, 34, 41, 255)};
		this.ChangeState(PLATESTATE.UNCLICKED);

		sweatyObj = GameObject.Instantiate(PrefabManager.Instance.slowEffectParticle);
		sweatyObj.transform.parent = this.transform;
		sweatyObj.transform.localPosition = new Vector3(0,0.6f,0);
		sweatyObj.SetActive(false);
	}

	public override void SlimeScaleChange()
	{
		float sizeDummy = (Mathf.Max(5f, Mathf.Min(Health * 0.075f+3f, 10f)));
		this.transform.localScale = Vector3.one * sizeDummy;
		box.size = Vector3.one * (5 / sizeDummy);
		//nav.radius = this.transform.localScale.x * 0.5f;
	}

	protected void Update()
	{
		if (GameManager.Instance.gameState == GAMESTATE.START)
		{
			// 초반 중립진영이 아닌 즉 각 팀의 베이스 캠프일 경우
			if (!canChanged)
			{
				nearCount = 0;
				this.area.transform.localScale = Vector3.one * (GameManager.Instance.distanceEP / 5) / 1.8f;
				for (int i = 0; i < GameManager.Instance.arrNone.Count; i++)
				{
					float dummy_dis = Vector3.Distance(GameManager.Instance.arrNone[i].transform.position, this.transform.position);
					// 중립 진영과의 거리가 원 내부에 있다면 nearCount 추가
					if (GameManager.Instance.distanceEP / 2 + 0.5f >= dummy_dis)
						nearCount++;
					// 중립 진영이 플레이어/적군 진영의 거리보다 더 멀리 떨어져있다면 빼기
					else if (dummy_dis >= GameManager.Instance.distanceEP)
						nearCount--;
				}
			}
			// 초반에 중립진영이었으나, 현재 중립이 아닌 각 팀에 속해있을 경우.
			else {
				ChangeBaseObj();
				if (this.state != TEAM.NONE)
				{
					switch (this.state)
					{
						case TEAM.PLAYER:
							nearCount = GameManager.Instance.stdPointSB.currentNearCount;
							difficultyMultipler = GameManager.Instance.stdPointSB.difficultyMultipler;
							break;
						case TEAM.ENEMY:
							nearCount = GameManager.Instance.enePointSB.currentNearCount;
							difficultyMultipler = GameManager.Instance.enePointSB.difficultyMultipler;
							break;
					}
				}
			}

			if (currentNearCount != nearCount || ( LS_TimerSC.Instance.timer-setting_prevTimer >= 10.0f))
			{
				setting_prevTimer = LS_TimerSC.Instance.timer;
				multipleNum = GameManager.Instance.marker.markerLen - nearCount;
				for (int i = 0; i < atkObj.Count; i++)
				{ ChangeSoldierPower(atkObj[i]); }
			}
			currentNearCount = nearCount;
		}
	}

	private IEnumerator ReChargeSlime()
	{
		float recharge_timer = 0;
		while (true)
		{
			if (LS_TimerSC.Instance.timer - recharge_timer > rechargeDelay * ((canChanged) ? 1.5f : 1f) * difficultyMultipler + woundRechargeDelay)
			{
				recharge_timer = LS_TimerSC.Instance.timer;
				if (this.state != TEAM.NONE && GameManager.Instance.gameState == GAMESTATE.START)
				{
					this.Health += rechargeHP;
					this.SlimeScaleChange();
				}
			}
			yield return new WaitForSeconds(0.1f);
		}
	}

	public void ChangeState(PLATESTATE s)
	{
		this.clickState = s; 
		plate.material.color = arrColor[(int)clickState];
	}

	// 병사의 공격력 변경
	public void ChangeSoldierPower(GameObject obj)
	{
		float dummy_timer = LS_TimerSC.Instance.timer; // 50초마다 공격력 1상승, 10초마다 이동속도 0.01f퍼센트 상승,  10초마다 공격딜레이 0.01f퍼센트 하락
		this.Attack = Mathf.RoundToInt(Mathf.Max(1, Mathf.CeilToInt(multipleNum /4) + dummy_timer / 50) *arrSAD[1]);
		speed = Mathf.Round(StructorCollector.BASESPEED * Mathf.Max(0.5f, multipleNum / 4 +dummy_timer/1000)*1000)/1000 *arrSAD[0];
		del = Mathf.Round(StructorCollector.BASEDELAYATTACK * Mathf.Max(0.2f,(1.5f - 0.1f*multipleNum - dummy_timer/1000) *difficultyMultipler) * 1000) / 1000 *arrSAD[2];
		obj.GetComponent<SlimeBridge>().SettingAtkSpeedDelay(this.Attack, speed, del);
	}

	// 마지막으로 공격을 한 슬라임의 팀으로 변경. 초기 체력 5 지급.
	private void OnTriggerEnter(Collider other)
	{
		if (other.transform.CompareTag("Slime"))
		{
			finalAttack=other.GetComponent<SlimeSoldierSC>().state;
			if (Health-other.GetComponent<SlimeSoldierSC>().Attack <= 0 && canChanged)
			{
				int visibleNum = 0;
				bool changeT = (this.state == TEAM.NONE) ? false : true;
				if (finalAttack != this.state)
				{
					switch (finalAttack)
					{
						case TEAM.PLAYER:
							this.state = TEAM.PLAYER;
							visibleNum = 1;
							if (changeT)
							{ GameManager.Instance.arrEnemy.Remove(this.transform.parent.gameObject); LS_EnemyBaseSC.Instance.RemovePreviousTeamOrder(this.transform.parent.gameObject); }
							GameManager.Instance.arrPlayer.Add(this.transform.parent.gameObject);
							break;
						case TEAM.ENEMY:
							this.state = TEAM.ENEMY;
							visibleNum = 2;
							if (changeT)
							{ GameManager.Instance.arrPlayer.Remove(this.transform.parent.gameObject); }
							GameManager.Instance.arrEnemy.Add(this.transform.parent.gameObject);
							break;
					}
					ChangeBaseObj();
					StopOrderPreviousTeam();
					this.Health = 5;
					this.previousHP = this.Health; this.regenePerSec = 0;
				}
			}

			else if (Health-other.GetComponent<SlimeSoldierSC>().Attack <= 0 && !canChanged)
            {
				if (other.GetComponent<SlimeSoldierSC>().state != this.state)
				{
					GameManager.Instance.GameEnding(this.state);
				}
            }
		}
	}

	private void ChangeBaseObj()
	{
		int visibleNum = (int)this.state;
		for (int i = 0; i < 3; i++)
		{
			if (i == visibleNum) bases[i].SetActive(true);
			else bases[i].SetActive(false);
		}
	}

	public void StopOrderPreviousTeam()
	{
		foreach (GameObject obj in atkObj)
		{
			obj.GetComponent<SlimeBridge>().CancleAttack();
		}
		atkObj.Clear();
	}

	public override IEnumerator Damaged(int damage)
	{
		StopCoroutine(recoverRecharge);
		recoverRecharge = RecoverDelay();
		StartCoroutine(recoverRecharge);
		return base.Damaged(damage);
	}

	public void SkillDamaged(int damage)
    {
		int dummy_damage = damage * Mathf.RoundToInt((canChanged) ? 1 : 0.5f);
		if (this.Health - dummy_damage < 5)
			StartCoroutine(Damaged(this.Health - 5));
		else
			StartCoroutine(Damaged(dummy_damage));
	}

	private IEnumerator RecoverDelay()
	{
		woundRechargeDelay = 4f * ((canChanged) ? 1f : 0.5f);
		sweatyObj.SetActive(true);
		yield return new WaitForSeconds(5f);
		woundRechargeDelay = 0;
		sweatyObj.SetActive(false);
	}

	public void settingSkillSAD(float speed, float attack, float delay)
	{
		arrSAD[0] = speed; arrSAD[1] = attack; arrSAD[2] = delay;
		for (int i = 0; i < atkObj.Count; i++)
		{ ChangeSoldierPower(atkObj[i]); }
	}
	public float[] GetSAD()
	{
		return new float[] { this.speed, this.Attack, this.del };
	}
	public IEnumerator BloodingCalc()
	{
		int sumHealth;
		while (true)
		{
			yield return new WaitForSeconds(1f);
			if (GameManager.Instance.gameState == GAMESTATE.START)
			{
				sumHealth = this.Health - previousHP;
				previousHP = this.Health;
				regenePerSec = (sumHealth + regenePerSec) / 2f;
			}
		}
	}
}
