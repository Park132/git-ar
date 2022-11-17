using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class LS_EnemyBaseSC : MonoBehaviour
{
	// 개같은 깃허브 망할!
	// 싱글톤 /////
	private static LS_EnemyBaseSC instance;

	private void Awake()
	{
		if (instance == null) { instance = this; }
		else{ Destroy(this.gameObject);}
	}
	public static LS_EnemyBaseSC Instance {
		get {
			if (null == instance) { return null; }
			return instance;
		}
	}
	/////

	//public ENEMYTYPE e_type;
	//[SerializeField]private ENEMYCHAR e_char;
	//[SerializeField] private int minEmergencyBase, maxAttackCount, maxRechargeCount, maxSupportHP;
	//private int skills;
	//[SerializeField] private int emergencyHP, stopAttackHP;
	//[SerializeField] private float delayThink;
	public ENEMYTYPE e_type;
	private SlimeBaseSC baseSC;
	[SerializeField] private List<List<GameObject>> attackList;
	[SerializeField] private List<ENEMYATTACKTYPE> attackTypeList;
	[SerializeField] private int attackListCount;
	[SerializeField] private StructorCollector.DestinationSet des;
	[SerializeField]List<StructorCollector.AI_CampCheck> allDummy = new List<StructorCollector.AI_CampCheck>();
	[SerializeField]List<StructorCollector.Bridge_Info> baseConnectedArr = new List<StructorCollector.Bridge_Info>();
	bool attack_once;
	private float prevCheckTime;

	[SerializeField]public StructorCollector.AI_Setting ai;


	private void Start()
	{
		baseSC = this.GetComponent<SlimeBaseSC>();
		attackList = new List<List<GameObject>>();
		attackList.Add(new List<GameObject>());
		attackList.Add(new List<GameObject>());
		attackTypeList = new List<ENEMYATTACKTYPE>();
		attackListCount = 0;
		attack_once = false;
		//ai = new StructorCollector.AI_Setting();
	}

	public void SettingEnemyTypeHard()
	{
		e_type = ENEMYTYPE.HARD;
	}
	public void SettingEnemyTypeNormal()
	{
		e_type = ENEMYTYPE.NORMAL;
	}
	public void SettingEnemyTypeTutorial()
	{
		e_type = ENEMYTYPE.TUTORIAL;
	}

	public void StartAI()
	{
		// 모든 진영을 저장.
		if (true)
		{
			List<StructorCollector.Bridge_Info> dummy = new List<StructorCollector.Bridge_Info>();
			List<GameObject> arrDummy = GameManager.Instance.arrNone.ToList();
			arrDummy.Add(GameManager.Instance.arrPlayer[0]);

			foreach (GameObject obj in arrDummy)
			{
				TargetEnable TE_dummy = obj.GetComponent<TargetEnable>();
				dummy.Clear();
				for (int i = 0; i < BridgeManager.Instance.bridgeCount; i++)
				{
					GameObject dummy_obj = BridgeManager.Instance.CheckConnect(obj, i);
					if (!ReferenceEquals(dummy_obj, null))
						dummy.Add(new StructorCollector.Bridge_Info(dummy_obj, dummy_obj.GetComponentInChildren<SlimeBaseSC>()));
				}
				allDummy.Add(new StructorCollector.AI_CampCheck(obj, TE_dummy.slSC, dummy));
			}
		}
		//////////
		TargetEnable TE_base = this.GetComponentInParent<TargetEnable>();
		foreach (GameObject obj in GameManager.Instance.arrNone)
		{
			if(BridgeManager.Instance.CheckBridge(obj, TE_base.gameObject)){
				this.baseConnectedArr.Add(new StructorCollector.Bridge_Info(obj, obj.GetComponentInChildren<SlimeBaseSC>()));
			}
		}
		///
		switch (e_type)
		{	//AI_Setting(enemyType, minEmergency, maxAttack, delayThink, skills, delayCheck, character);
			case ENEMYTYPE.TUTORIAL:
				ai = new StructorCollector.AI_Setting(e_type, 1, 2, 5f, 1, 10f ,ENEMYCHAR.DEFENSIVE);
				//ai.maxRechargeCount = 2;
				//ai.e_char = ENEMYCHAR.DEFENSIVE;
				//ai.emergencyHP = 8; ////
				//ai.stopAttackHP = 20;
				//ai.maxSupportHP = 30;
				break;
			case ENEMYTYPE.NORMAL:
				ai = new StructorCollector.AI_Setting(e_type, 1, 5, 3f, 2, 8f, ENEMYCHAR.DEFENSIVE);
				//ai.maxRechargeCount = 2;
				//ai.e_char = ENEMYCHAR.DEFENSIVE;
				//ai.emergencyHP = 10;
				//ai.stopAttackHP = 12;
				//ai.maxSupportHP = 40;
				break;

			case ENEMYTYPE.HARD:
				ai = new StructorCollector.AI_Setting(e_type, 1, 7, 0.5f, 2, 3f, ENEMYCHAR.AGRESSIVE);
				//ai.maxRechargeCount = 3;
				//ai.e_char = ENEMYCHAR.AGRESSIVE;
				//ai.emergencyHP = 13;
				//ai.stopAttackHP = 10;
				//ai.maxSupportHP = 30;
				break;
		}
		prevCheckTime = 0;
		StartCoroutine(UpdateAI());
	}

	private IEnumerator UpdateAI()
	{
		// 한번에 공격, 공격 취소가 가능.
		while (true)
		{
			// 현재 갖고 있는 진영의 수가 일정 수 이하일 경우 응급상황
			// 전부 취소하고 하나만 먹으려고 노력
			if (GameManager.Instance.arrEnemy.Count - 1 < ai.minEmergencyBase)
			{
				bool dummy_emergency_act = false;
				for (int i = attackListCount - 1; i >= 0; i--)
				{
					if (attackTypeList[i] != ENEMYATTACKTYPE.EMERGENCY)
						this.OrderStopAttack(attackList[0][i], attackList[1][i]);
					else
						dummy_emergency_act = true;
				}
				if (!dummy_emergency_act)
					this.emergencyAttack();
			}

			else if (!this.attack_once)
			{
				if (ai.e_char == ENEMYCHAR.AGRESSIVE)
					this.AttackAI2();
				else if (ai.e_char == ENEMYCHAR.DEFENSIVE)
					this.AttackAI();
			}
			if (ai.maxAttackCount > attackListCount)
			{
				SuddenAttackAI();
			}
			DefenseAI();
			EmergencyBase();
			CheckAttackAI();

			// 만약 공격을 한번도 안한다면 이유 찾기
			if (attackListCount == 0)
			{CheckState();}

			// 공격 정지
			StopAttack();
			
			yield return new WaitForSeconds(ai.delayThink);
		}
	}

	// 현재 적팀의 기지가 지정된 수보다 적을 경우 응급상황
	// 가깝고 체력이 적은 곳을 공격함.
	private void emergencyAttack()
	{
		GameObject startP = null, destP = null;
		// total = (dist / 2) + health
		float dest_total = float.MaxValue - 1;

		// 현재 팀이 Enemy인 모든 오브젝트를 불러온다.
		for (int i = 0; i < GameManager.Instance.arrEnemy.Count; i++)
		{
			if (GameManager.Instance.arrEnemy[i].GetComponentInChildren<SlimeBaseSC>().Health >= ai.stopAttackHP)
			{
				// 모든 다리에 대한 정보를 불러온다.
				for (int j = 0; j < BridgeManager.Instance.bridgeCount; j++)
				{
					GameObject dummy_connect_obj = BridgeManager.Instance.CheckConnect(GameManager.Instance.arrEnemy[i], j);
					// 만약 현재 Enemy 진영과 연결된 진영이라면
					if (!ReferenceEquals(dummy_connect_obj, null) &&
						!AttackCheck(GameManager.Instance.arrEnemy[i], dummy_connect_obj))
					{
						float dummy_dist = Vector3.Distance(dummy_connect_obj.transform.position, GameManager.Instance.arrEnemy[i].transform.position);
						int dummy_health = dummy_connect_obj.GetComponentInChildren<SlimeBaseSC>().Health;
						float dummy_total = (dummy_dist / 2) + dummy_health;
						if (dest_total > dummy_total)
						{
							startP = GameManager.Instance.arrEnemy[i]; destP = dummy_connect_obj;
							dest_total = dummy_total;
						}
					}
				}
			}
		}
		CheckBeforeOrder(destP, startP, ENEMYATTACKTYPE.EMERGENCY);
	}

	// 가장 가까이에 있는 진영을 공격하는 구문
	private void AttackAI()
	{
		GameObject startP = null, destP = null;

		// 거리, 남은 체력, 팀에 따라 우선 순위가 결정된다.
		// 공식 = (dist/2) + hp + 2*(team)
		float dest_total = float.MaxValue-1;

		// 현재 팀이 Enemy인 모든 오브젝트를 불러온다.
		for (int i = 0; i < GameManager.Instance.arrEnemy.Count; i++)
		{
			if (GameManager.Instance.arrEnemy[i].GetComponentInChildren<SlimeBaseSC>().Health >= ai.stopAttackHP)
			{
				// 모든 다리에 대한 정보를 불러온다.
				for (int j = 0; j < BridgeManager.Instance.bridgeCount; j++)
				{
					GameObject dummy_connect_obj = BridgeManager.Instance.CheckConnect(GameManager.Instance.arrEnemy[i], j);
					// 만약 현재 Enemy 진영과 연결된 진영이라면
					// 공격 중인가 확인.
					if (!ReferenceEquals(dummy_connect_obj, null) &&
						!AttackCheck(GameManager.Instance.arrEnemy[i], dummy_connect_obj))
					{
						SlimeBaseSC dummySSC = dummy_connect_obj.GetComponentInChildren<SlimeBaseSC>();
						int dummy_hp = dummySSC.Health;
						TEAM dummy_team = dummySSC.state;
						float dummy_dist = Vector3.Distance(dummy_connect_obj.transform.position, GameManager.Instance.arrEnemy[i].transform.position);

						if (dummy_team != TEAM.ENEMY)
						{
							float dummy_total = (dummy_dist / 2) + dummy_hp + 2 * CharacterMultTeam(dummy_team);

							if (dest_total > dummy_total)
							{
								startP = GameManager.Instance.arrEnemy[i]; destP = dummy_connect_obj;
								dest_total = dummy_total;
							}
						}

					}
				}
			}

		}
		CheckBeforeOrder(destP, startP,ENEMYATTACKTYPE.ATTACK);
	}

	// 현재 모든 적근 팀에서 일정피 이하인 곳 찾기.
	// 찾았다면 먼저 maxAttackCount를 확인.
	// maxAttackCount보다 크다면 마지막 명령을 종료한 후에 회복용 공격 경로 설정.
	private void DefenseAI()
	{
		int index = -1;
		int minHP = int.MaxValue;
		// 체력이 일정량 이하 혹은 체력 재생이 -1.2 이하라면 응급상황 디펜스 필요
		for (int i = 0; i < allDummy.Count; i++)
		{
			if (allDummy[i].obj_sc.state == TEAM.ENEMY)
			{
				if (allDummy[i].obj_sc.Health <= ai.emergencyHP || allDummy[i].obj_sc.regenePerSec <= -1.2f || allDummy[i].obj_sc.woundRechargeDelay != 0)
				{
					if (minHP > allDummy[i].obj_sc.Health)
					{ index = i; minHP = allDummy[i].obj_sc.Health; }
				}
			}
		}
		if (index != -1)
		{
			Debug.Log("emergency emergency!!" + index + "\nemergency destp = " + allDummy[index].obj);
			StructorCollector.AI_CampCheck dummy_campcheck = allDummy[index];
			for (int i = 0; i < dummy_campcheck.connectedObj.Count; i++)
			{
				// 같은 팀이라면 모두 달려가서 체력회복 시키기
				if (dummy_campcheck.connectedObj[i].obj_sc.state == TEAM.ENEMY)
				{
					if (dummy_campcheck.connectedObj[i].obj_sc.regenePerSec >= -0.6f && dummy_campcheck.connectedObj[i].obj_sc.Health >= ai.stopAttackHP)
					{
						Debug.Log("Lets Recharging! start = " +dummy_campcheck.connectedObj[i].obj);
						CheckBeforeOrder(dummy_campcheck.obj,dummy_campcheck.connectedObj[i].obj, ENEMYATTACKTYPE.RECHARGING);
					}
					else
						Debug.Log("Dont Recharging because non equals team or low hp = " + dummy_campcheck.connectedObj[i].obj);
				}
			}
		}
	}

	private void EmergencyBase()
	{
		if (baseSC.Health <= ai.emergencyHP * 1.5f && baseSC.regenePerSec <= -1.0f)
		{
			Debug.Log("BaseCamp Defense Start!");
			for (int i = 0; i < baseConnectedArr.Count; i++)
			{
				if (baseConnectedArr[i].obj_sc.state == TEAM.ENEMY)
				{
					CheckBeforeOrder(this.gameObject, baseConnectedArr[i].obj, ENEMYATTACKTYPE.RECHARGING);
				}
			}
		}
	}

	private void AttackAI2()
	{
		this.attack_once = true;
		int dummy_health;
		List<int> indexs = new List<int>();
		List<StructorCollector.AI_StateofAttack> canAttackIndex = new List<StructorCollector.AI_StateofAttack>();
		
		// 모든 지점에 대하여 계산.
		// 공격 지점의 피를 확인. 그 진영에 연결된 진영 중 적팀(우리팀)이 존재하는지 확인.
		// 존재한다면 그 진영을 공격할 때 소모되는 피, 체력 등을 확인.
		for (int i = 0; i < allDummy.Count; i++)
		{
			if (allDummy[i].obj_sc.state == TEAM.ENEMY)
				continue;
			indexs.Clear();
			dummy_health = allDummy[i].obj_sc.Health;
			for (int j = 0; j < allDummy[i].connectedObj.Count; j++)
			{
				if (allDummy[i].connectedObj[j].obj_sc.state == TEAM.ENEMY)
					indexs.Add(j);
			}

			if (indexs.Count > 0)
			{
				List<float> timeForBreaksArr = new List<float>();
				float totalDamageTIme = 0;
				foreach (int dummy_index in indexs)
				{
					float[] dummyarr = allDummy[i].connectedObj[dummy_index].obj_sc.GetSAD();
					// 1초에 공격 가능한 데미지와 공격 가능한 시간.
					float distance_dummy = Vector3.Distance(allDummy[i].obj.transform.position, allDummy[i].connectedObj[dummy_index].obj.transform.position);
					float speed_dummy = dummyarr[0] * (1 / Time.deltaTime);
					float firstAttackTime = distance_dummy / speed_dummy;   // 맨 처음 한마리가 닿는데 걸리는 시간
					float DamagePerSec = ((firstAttackTime - dummyarr[2]) + dummyarr[2] * 100) / 100;   // 100초를 기준으로 1초에 몇 데미지를 주는지 확인
					// 모든 피를 깍는데 걸리는 시간
					timeForBreaksArr.Add(allDummy[i].obj_sc.Health / DamagePerSec);
					totalDamageTIme += allDummy[i].obj_sc.Health / DamagePerSec;
				}
				int enoughDamage = 0;
				float eachAttackTime = (allDummy[i].obj_sc.Health / totalDamageTIme);
				for(int j = 0; j < indexs.Count; j++)
				{enoughDamage += (allDummy[i].connectedObj[indexs[j]].obj_sc.Health - ai.stopAttackHP) 
						- Mathf.CeilToInt(eachAttackTime * timeForBreaksArr[j]);}

				if (enoughDamage >= 0)
				{
					if (allDummy[i].obj_sc.state == TEAM.PLAYER)
						if (enoughDamage < totalDamageTIme / 2)
							continue;
					canAttackIndex.Add(new StructorCollector.AI_StateofAttack(i, enoughDamage, dummy_health));
				}
			}
			else
				continue;
		}

		float maxPhEh = float.MinValue;
		int dummy_index_pheh = -1;
		// 가장 공격하기 적합한 곳을 고르는 알고리즘
		// - desthealth + enoughHP * 2
		for (int i = canAttackIndex.Count-1; i >= 0; i--)
		{
			float dummy_max = (canAttackIndex[i].enoughHealth * 2)-allDummy[canAttackIndex[i].index].obj_sc.Health;
			if (maxPhEh < dummy_max)
			{ maxPhEh = dummy_max; dummy_index_pheh = i; }
		}

		if (dummy_index_pheh != -1)
		{
			StructorCollector.AI_CampCheck dummy_check = allDummy[canAttackIndex[dummy_index_pheh].index];
			Debug.Log("Attack pheh = " + maxPhEh + "\nattack dummy team = " +dummy_check.obj.name);
			for (int i = 0; i < dummy_check.connectedObj.Count; i++)
			{
				if (dummy_check.connectedObj[i].obj_sc.state == TEAM.ENEMY)
				{
					Debug.Log(dummy_check.obj.name + "       start = " +dummy_check.connectedObj[i].obj.name);
					CheckBeforeOrder(dummy_check.obj, dummy_check.connectedObj[i].obj, ENEMYATTACKTYPE.ATTACK);
				}
			}
		}
	}

	// 먹을 수 있다면 바로 공격 시도
	private void SuddenAttackAI()
	{
		for (int i = 0; i < allDummy.Count; i++)
		{
			if (attackListCount < ai.maxAttackCount)
			{
				// 만약 해당 진영의 피가 7 이하라면
				if (allDummy[i].obj_sc.Health <= 8 && allDummy[i].obj_sc.state != TEAM.ENEMY)
				{
					for (int j = 0; j < allDummy[i].connectedObj.Count; j++)
					{
						SlimeBaseSC dummy_slsc = allDummy[i].connectedObj[j].obj_sc;
						if (dummy_slsc.state == TEAM.ENEMY && (dummy_slsc.Health) >= 7)
						{ CheckBeforeOrder(allDummy[i].obj, allDummy[i].connectedObj[j].obj, ENEMYATTACKTYPE.SUDDENLY); }
					}
				}
			}
		}
	}

	// 견제 공격 -> 견제공격을 한다면 공격 받은 기지의 체력 재생 속도가 느려짐.
	// 연결된 자신의 팀 중 가장 체력이 높은 기지가 한대 톡 치기.
	private void CheckAttackAI()
	{
		if (LS_TimerSC.Instance.timer - prevCheckTime >= ai.delayCheckAttack)
		{
			int maxHP = int.MinValue, index = -1;
			for (int i = 0; i < allDummy.Count; i++)
			{
				maxHP = int.MinValue;
				index = -1;
				if (allDummy[i].obj_sc.state == TEAM.ENEMY)
					continue;
				for (int j = 0; j < allDummy[i].connectedObj.Count; j++) {
					if (allDummy[i].connectedObj[j].obj_sc.state == TEAM.ENEMY)
					{
						if (allDummy[i].connectedObj[j].obj_sc.Health > maxHP)
						{ index = j; maxHP = allDummy[i].connectedObj[j].obj_sc.Health; }
					}
				}
				if (index != -1)
				{
					CheckBeforeOrder(allDummy[i].obj, allDummy[i].connectedObj[index].obj, ENEMYATTACKTYPE.CHECKATTACK);
				}
			}
		}
	}

	private IEnumerator CheckDestroy(GameObject P1, GameObject P2)
	{
		yield return new WaitForSeconds(2f);
		OrderStopAttack(P1, P2);
	}

	// 체크체크 왜 공격을 안 하나! 빠져가지곤
	private void CheckState()
	{
		// 홀로 남은 기지
		if (GameManager.Instance.arrEnemy.Count == 1)
		{
			// 발악 패턴을 추가할 것인지?
		}
		// 기지는 여러개임 근데 공격같은걸 안해오
		else
		{
			int hp = int.MaxValue;
			int index = -1;

			// 현재 가장 피가 적은 기지를 찾음.
			for (int i = 0; i < GameManager.Instance.arrEnemy.Count; i++)
			{
				// 원래 현재 체력이 maxSupportHP /2 보다 작은 경우에 찾게 하려고 하였음.
				// 허나 지금 공격이 아무것도 없는 상태. 결국 전부 공격 가능 체력보다 낮은 상태이므로 체력 확인 X
				SlimeBaseSC dummy_sc = GameManager.Instance.arrEnemy[i].GetComponentInChildren<SlimeBaseSC>();
				if (dummy_sc.Health < hp)
				{ index = i; hp = dummy_sc.Health; }
			}

			if (index != -1)
			{
				GameObject destP = GameManager.Instance.arrEnemy[index];
				float dest_total = float.MinValue;
				GameObject startP = null;

				// 지원 명령
				for (int i = 0; i < GameManager.Instance.arrEnemy.Count; i++)
				{
					if (i != index)
					{
						GameObject dummy_obj = GameManager.Instance.arrEnemy[i];
						// 만약 현재 Enemy 진영과 연결된 진영이라면
						// 공격 중인가 확인.
						if (BridgeManager.Instance.CheckBridge(GameManager.Instance.arrEnemy[i], GameManager.Instance.arrEnemy[index]) &&
							!AttackCheck(dummy_obj, destP))
						{
							
							float dummy_dist = (GameManager.Instance.distanceEP * 2) -
								Vector3.Distance(dummy_obj.transform.position, destP.transform.position);
							int dummy_hp = dummy_obj.GetComponentInChildren<SlimeBaseSC>().Health;
							if (dummy_hp >= ai.maxSupportHP / 2)
							{
								float dummy_total = dummy_dist + (dummy_hp * 2);
								if (dest_total < dummy_total)
								{
									dest_total = dummy_total;
									startP = GameManager.Instance.arrEnemy[i];
								}
							}
						}
					}
				}
				CheckBeforeOrder(destP, startP, ENEMYATTACKTYPE.RECHARGING);
			}
		}
	}

	private void StopAttack()
	{
		// 공격 정지. -> 일정 피 이하일 경우, 일정 피 이상 회복 시켜주었다면 
		int attackCount = 0; // attack2를 다시 사용할 수 있게 하기 위한 변수
		for (int i = this.attackListCount - 1; i >= 0; i--)
		{
			bool dummy_stop = false;
			SlimeBaseSC dummy_sl_s = attackList[0][i].GetComponentInChildren<SlimeBaseSC>();
			SlimeBaseSC dummy_sl_d = attackList[1][i].GetComponentInChildren<SlimeBaseSC>();
			if (attackTypeList[i] == ENEMYATTACKTYPE.ATTACK)
				attackCount++;

			// 공격하는 진영의 피가 일정 피 이하일 경우 종료
			if (dummy_sl_s.Health < ai.stopAttackHP && attackTypeList[i] != ENEMYATTACKTYPE.RECHARGING && attackTypeList[i] != ENEMYATTACKTYPE.EMERGENCY)
				dummy_stop = true;
			else if (dummy_sl_s.Health <= ai.maxSupportHP / 2 && attackTypeList[i] == ENEMYATTACKTYPE.RECHARGING)
				dummy_stop = true;

			// 현재 자기 자신의 팀을 공격하고, 일정 피 이상이면 종료
			if (dummy_sl_d.state == TEAM.ENEMY)
			{
				if (attackTypeList[i] == ENEMYATTACKTYPE.EMERGENCY)	// 급한불을 껏으면 종료
					dummy_stop = true;
				if (dummy_sl_d.Health > ai.emergencyHP && attackTypeList[i] != ENEMYATTACKTYPE.RECHARGING)  // 공격 당하는 진영 체력이 일정 이상이면 종료
					dummy_stop = true;
				// 회복 당하는 진영의 피가 서포팅 가능 피보다 크거나, 회복 당하는 진영 피가 하는 진영보다 클떄 종료
				else if ((dummy_sl_d.Health > ai.maxSupportHP || dummy_sl_d.Health >= dummy_sl_s.Health) && attackTypeList[i] == ENEMYATTACKTYPE.RECHARGING && dummy_sl_s.canChanged)
					dummy_stop = true;
				else if (!dummy_sl_s.canChanged && dummy_sl_s.Health <= ai.emergencyHP)
					dummy_stop = true;
			}
			// 만약 공격 당하는 진영이 적(플레이어)이다.
			else if (dummy_sl_d.state == TEAM.PLAYER)
			{
				if (dummy_sl_d.Health >= dummy_sl_s.Health * 1.5f || dummy_sl_d.regenePerSec >= 0.2f)
					dummy_stop = true;
				if (attackTypeList[i] == ENEMYATTACKTYPE.SUDDENLY)
					dummy_stop = true;
			}

			if (dummy_stop)
			{ this.OrderStopAttack(attackList[0][i], attackList[1][i]); }
		}
		if (attackCount <= 1)
			attack_once = false;
	}

	// 특정 조건 때 두개의 지점을 잇는 다리가 존재하며, 공격을 하지 않음을 전제로 실행.
	private void OrderAttack(GameObject P1, GameObject P2, ENEMYATTACKTYPE t)
	{
		des.SetP1 = P1.GetComponentInChildren<SlimeBaseSC>().gameObject;
		des.SetP2 = P2.GetComponentInChildren<SlimeBaseSC>().gameObject;


		string dummy_name = des.SetP1.name + "_attack_" + des.SetP2.name;

		GameObject dummy = GameObject.Instantiate(BridgeManager.Instance.bridge_obj);
		dummy.name = dummy_name;

		dummy.transform.parent = GameManager.Instance.attackObjs.transform;
		dummy.GetComponent<SlimeBridge>().SetSD(des, TEAM.ENEMY);
		des.SetP1.GetComponent<SlimeBaseSC>().atkObj.Add(dummy);
		des.SetP1.GetComponent<SlimeBaseSC>().ChangeSoldierPower(dummy);

		if (t == ENEMYATTACKTYPE.CHECKATTACK)
		{
			StartCoroutine(CheckDestroy(des.SetP1, des.SetP2));
		}
	}

	// 특정 조건 때 두개의 지점을 잇는 다리가 존재한다는 전제로 실행.
	private void OrderStopAttack(GameObject P1, GameObject P2)
	{
		des.SetP1 = P1.GetComponentInChildren<SlimeBaseSC>().gameObject;
		des.SetP2 = P2.GetComponentInChildren<SlimeBaseSC>().gameObject;

		GameObject dummy = GameManager.Instance.attackObjs.transform.Find(des.SetP1.name + "_attack_" + des.SetP2.name).gameObject;
		des.SetP1.GetComponent<SlimeBaseSC>().atkObj.Remove(dummy);
		dummy.GetComponent<SlimeBridge>().CancleAttack();
		int dummy_index = FindAttackIndex(P1, P2);
		if (dummy_index != -1)
		{
			attackList[0].RemoveAt(dummy_index); attackList[1].RemoveAt(dummy_index); 
			attackTypeList.RemoveAt(dummy_index); attackListCount--; 
		}
	}

	// 공격 명령 내리기 전 확인
	private void CheckBeforeOrder(GameObject destP, GameObject startP, ENEMYATTACKTYPE t)
	{
		if (!ReferenceEquals(destP, null) && !ReferenceEquals(startP, null) && !ReferenceEquals(destP, startP))
		{

			GameObject dSetP1 = startP.GetComponentInChildren<SlimeBaseSC>().gameObject;
			GameObject dSetP2 = destP.GetComponentInChildren<SlimeBaseSC>().gameObject;

			SlimeBaseSC p1_sc = dSetP1.GetComponent<SlimeBaseSC>();
			bool exist = false;
			for (int i = 0; i < p1_sc.atkObj.Count; i++)
			{
				if (p1_sc.atkObj[i].name.Equals(dSetP1.name + "_attack_" + dSetP2.name))
				{ exist = true; break; }
			}

			if (!exist)
			{
				attackList[0].Add(startP); attackList[1].Add(destP);
				attackTypeList.Add(t);
				attackListCount++;
				OrderAttack(startP, destP, t);
			}
			else
				Debug.Log("Error: Error in OrderAttack P1 = " + startP + "  P2 = " + destP);
		}
		else
			Debug.Log("Error: Error in CheckBeforeOrder start = " + startP + "  dest = " + destP);
	}

	// 현재 적의 성격에 따라 피공격자의 팀에 따라서 반환 값이 달라짐.
	private int CharacterMultTeam(TEAM t)
	{
		int mult = 0;
		switch (ai.e_char)
		{
			case ENEMYCHAR.DEFENSIVE:
				switch (t)
				{
					case TEAM.PLAYER:
						mult = 0; break;
					case TEAM.NONE:
						mult = -2; break;
				}
				break;
			case ENEMYCHAR.AGRESSIVE:
				switch (t)
				{
					case TEAM.PLAYER:
						mult = -2; break;
					case TEAM.NONE:
						mult = -1; break;
				}
				break;
		}
		return mult;
	}

	// P1이 P2를 공격하는 리스트가 있는지 확인.
	private bool AttackCheck(GameObject P1, GameObject P2)
	{
		for (int i = 0; i < attackListCount; i++)
		{
			if (P1.Equals(attackList[0][i]) && P2.Equals(attackList[1][i]))
				return true;
		}
		return false;
	}

	// P1은 무조건 출발, P2는 도착 지점. attackList의 인덱스 두개를 변경하기 위해 필요한 함수
	private int FindAttackIndex(GameObject P1, GameObject P2)
	{
		for (int i = 0; i < this.attackListCount; i++)
		{
			if (attackList[0][i].Equals(P1) && attackList[1][i].Equals(P2))
				return i;
		}
		return -1;
	}

	// 갑작스럽게 공격경로가 있는 상태로 팀이 변경되었을 경우를 대비해서 만든 함수
	public void RemovePreviousTeamOrder(GameObject P1)
	{
		for (int i = attackListCount-1; i >= 0; i--)
		{
			if (attackList[0][i].Equals(P1))
			{
				attackList[0].RemoveAt(i);attackList[1].RemoveAt(i);
				attackTypeList.RemoveAt(i);
			}
		}
	}
}
