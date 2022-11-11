using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
	[SerializeField] private List<List<GameObject>> attackList;
	[SerializeField] private List<ENEMYATTACKTYPE> attackTypeList;
	[SerializeField] private int attackListCount;
	[SerializeField] private StructorCollector.DestinationSet des;

	public StructorCollector.AI_Setting ai;


	private void Start()
	{
		attackList = new List<List<GameObject>>();
		attackList.Add(new List<GameObject>());
		attackList.Add(new List<GameObject>());
		attackTypeList = new List<ENEMYATTACKTYPE>();
		attackListCount = 0;
		ai = new StructorCollector.AI_Setting();
	}

	public void SettingEnemyType(ENEMYTYPE t)
	{
		ai.e_type = t;
	}

	public void StartAI()
	{
		switch (ai.e_type)
		{
			case ENEMYTYPE.TUTORIAL:
				ai.minEmergencyBase = 1;
				ai.maxAttackCount = 2;
				ai.maxRechargeCount = 2;
				ai.e_char = ENEMYCHAR.DEFENSIVE;
				ai.delayThink = 4f;
				ai.skills = 1;
				ai.emergencyHP = 8; ////
				ai.stopAttackHP = 20;
				ai.maxSupportHP = 30;
				break;
			case ENEMYTYPE.NORMAL:
				ai.minEmergencyBase = 1;
				ai.maxAttackCount = 3;
				ai.maxRechargeCount = 2;
				ai.e_char = ENEMYCHAR.DEFENSIVE;
				ai.delayThink = 3f;
				ai.skills = 2;
				ai.emergencyHP = 10;
				ai.stopAttackHP = 12;
				ai.maxSupportHP = 40;
				break;
		}
		StartCoroutine(UpdateAI());
	}

	private IEnumerator UpdateAI()
	{
		// 한번에 공격, 공격 취소가 가능.
		while (true)
		{
			// 현재 갖고 있는 진영의 수가 일정 수 이하일 경우 응급상황
			// 전부 취소하고 하나만 먹으려고 노력
			if (GameManager.Instance.arrEnemy.Count-1 < ai.minEmergencyBase)
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
			else if (this.attackListCount < ai.maxAttackCount)
				this.AttackAI();

			emergencyCheck();

			// 공격 정지. -> 일정 피 이하일 경우, 일정 피 이상 회복 시켜주었다면 
			for (int i = this.attackListCount-1; i >= 0 ; i--)
			{
				bool dummy_stop = false;
				SlimeBaseSC dummy_sl_s = attackList[0][i].GetComponentInChildren<SlimeBaseSC>();
				SlimeBaseSC dummy_sl_d = attackList[1][i].GetComponentInChildren<SlimeBaseSC>();
				// 공격하는 진영의 피가 일정 피 이하일 경우 종료
				if (dummy_sl_s.Health < ai.stopAttackHP && attackTypeList[i] != ENEMYATTACKTYPE.RECHARGING)
					dummy_stop = true;
				else if (dummy_sl_s.Health <= ai.maxSupportHP / 2 && attackTypeList[i] == ENEMYATTACKTYPE.RECHARGING)
					dummy_stop = true;
				// 현재 자기 자신의 팀을 공격하고, 일정 피 이상이면 종료
				if (dummy_sl_d.state == TEAM.ENEMY)
				{
					if (dummy_sl_d.Health > ai.emergencyHP && attackTypeList[i] != ENEMYATTACKTYPE.RECHARGING)
						dummy_stop = true;
					else if (dummy_sl_d.Health > ai.maxSupportHP && attackTypeList[i] == ENEMYATTACKTYPE.RECHARGING)
						dummy_stop = true;
				}
				if (dummy_stop)
				{ this.OrderStopAttack(attackList[0][i], attackList[1][i]); }
			}

			// 만약 공격을 한번도 안한다면 이유 찾기
			if (attackListCount == 0)
			{
				CheckState();
			}
			// 상시로 하나 서포터 하게 작성해야겠음.
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
						!CheckAttack(GameManager.Instance.arrEnemy[i], dummy_connect_obj))
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
		CheckBeforeOrder(destP, startP, ENEMYATTACKTYPE.ATTACK);
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
						!CheckAttack(GameManager.Instance.arrEnemy[i], dummy_connect_obj))
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
	private void emergencyCheck()
	{
		int index = -1;
		int minHP = int.MaxValue;

		for (int i = 0; i < GameManager.Instance.arrEnemy.Count; i++)
		{
			SlimeBaseSC dummy_sl = GameManager.Instance.arrEnemy[i].GetComponentInChildren<SlimeBaseSC>();
			if (dummy_sl.Health <= ai.emergencyHP)
			{
				if (dummy_sl.Health < minHP)
				{ index = i; minHP = dummy_sl.Health; }
			}
		}
		// 일정 피 이하인 진영 발견 회복 시작
		if (index != -1)
		{
			Debug.Log("emergency emergency!!\n" + index);

			GameObject destP = GameManager.Instance.arrEnemy[index];
			Debug.Log("emergency destp = " +destP);

			int dummy_count = 0;
			for (int i = 0; i < attackListCount; i++)
			{ if (attackTypeList[i] == ENEMYATTACKTYPE.SUPPORT) dummy_count++; }
			// 설정된 값보다 회복하는 명령이 적을경우
			if (dummy_count < ai.maxRechargeCount)
			{
				float dest_total = float.MinValue;
				GameObject startP = null;
				
				// 지원 명령
				for (int i = 0; i < GameManager.Instance.arrEnemy.Count; i++)
				{
					if (i != index)
					{
						// 만약 현재 Enemy 진영과 연결된 진영이라면
						// 공격 중인가 확인.
						if (BridgeManager.Instance.CheckBridge(GameManager.Instance.arrEnemy[i], GameManager.Instance.arrEnemy[index]) &&
							!CheckAttack(GameManager.Instance.arrEnemy[i], destP))
						{
							float dummy_dist = (GameManager.Instance.distanceEP * 2) -
						Vector3.Distance(GameManager.Instance.arrEnemy[i].transform.position, destP.transform.position);
							int dummy_hp = GameManager.Instance.arrEnemy[i].GetComponentInChildren<SlimeBaseSC>().Health;
							float dummy_total = dummy_dist + (dummy_hp * 2);
							if (dest_total < dummy_total)
							{
								dest_total = dummy_total;
								startP = GameManager.Instance.arrEnemy[i];
							}
						}
					}
				}
				// 현재 공격하는 명령의 개수가 많을 경우 가장 마지막 명령을 취소
				if (!ReferenceEquals(destP, null) && !ReferenceEquals(startP, null))
				{
					if (attackListCount >= ai.maxAttackCount)
					{
						for (int i = attackListCount - 1; i >= 0; i--)
						{
							if (attackTypeList[i] != ENEMYATTACKTYPE.SUPPORT)
								this.OrderStopAttack(attackList[0][i], attackList[1][i]);
						}
					}
				}
				CheckBeforeOrder(destP, startP, ENEMYATTACKTYPE.SUPPORT);
			}
			else Debug.Log("error dummy_count = " +dummy_count +"\nand maxCount = " +ai.maxRechargeCount);
		}

	}

	// 공격 명령 내리기 전 확인
	private void CheckBeforeOrder(GameObject destP, GameObject startP, ENEMYATTACKTYPE t)
	{
		if (!ReferenceEquals(destP, null) && !ReferenceEquals(startP, null) && !ReferenceEquals(destP,startP))
		{
			attackList[0].Add(startP); attackList[1].Add(destP);
			attackTypeList.Add(t);
			attackListCount++;
			OrderAttack(startP, destP);
		}
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
						mult = -2;break;
					case TEAM.NONE:
						mult = -1;break;
				}
				break;
		}
		return mult;
	}

	// P1이 P2를 공격하는 리스트가 있는지 확인.
	private bool CheckAttack(GameObject P1, GameObject P2)
	{
		for (int i = 0; i < attackListCount; i++)
		{
			if (P1.Equals(attackList[0][i]) && P2.Equals(attackList[1][i]))
				return true;
		}
		return false;
	}

	// 체크체크
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
							!CheckAttack(dummy_obj, destP))
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


	
	// 특정 조건 때 두개의 지점을 잇는 다리가 존재하며, 공격을 하지 않음을 전제로 실행.
	private void OrderAttack(GameObject P1, GameObject P2)
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
