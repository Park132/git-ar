using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LS_EnemyBaseSC : MonoBehaviour
{
	// ������ ����� ����!
	// �̱��� /////
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
		// �ѹ��� ����, ���� ��Ұ� ����.
		while (true)
		{
			// ���� ���� �ִ� ������ ���� ���� �� ������ ��� ���޻�Ȳ
			// ���� ����ϰ� �ϳ��� �������� ���
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

			// ���� ����. -> ���� �� ������ ���, ���� �� �̻� ȸ�� �����־��ٸ� 
			for (int i = this.attackListCount-1; i >= 0 ; i--)
			{
				bool dummy_stop = false;
				SlimeBaseSC dummy_sl_s = attackList[0][i].GetComponentInChildren<SlimeBaseSC>();
				SlimeBaseSC dummy_sl_d = attackList[1][i].GetComponentInChildren<SlimeBaseSC>();
				// �����ϴ� ������ �ǰ� ���� �� ������ ��� ����
				if (dummy_sl_s.Health < ai.stopAttackHP && attackTypeList[i] != ENEMYATTACKTYPE.RECHARGING)
					dummy_stop = true;
				else if (dummy_sl_s.Health <= ai.maxSupportHP / 2 && attackTypeList[i] == ENEMYATTACKTYPE.RECHARGING)
					dummy_stop = true;
				// ���� �ڱ� �ڽ��� ���� �����ϰ�, ���� �� �̻��̸� ����
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

			// ���� ������ �ѹ��� ���Ѵٸ� ���� ã��
			if (attackListCount == 0)
			{
				CheckState();
			}
			// ��÷� �ϳ� ������ �ϰ� �ۼ��ؾ߰���.
			yield return new WaitForSeconds(ai.delayThink);
		}
	}

	// ���� ������ ������ ������ ������ ���� ��� ���޻�Ȳ
	// ������ ü���� ���� ���� ������.
	private void emergencyAttack()
	{
		GameObject startP = null, destP = null;
		// total = (dist / 2) + health
		float dest_total = float.MaxValue - 1;

		// ���� ���� Enemy�� ��� ������Ʈ�� �ҷ��´�.
		for (int i = 0; i < GameManager.Instance.arrEnemy.Count; i++)
		{
			if (GameManager.Instance.arrEnemy[i].GetComponentInChildren<SlimeBaseSC>().Health >= ai.stopAttackHP)
			{
				// ��� �ٸ��� ���� ������ �ҷ��´�.
				for (int j = 0; j < BridgeManager.Instance.bridgeCount; j++)
				{
					GameObject dummy_connect_obj = BridgeManager.Instance.CheckConnect(GameManager.Instance.arrEnemy[i], j);
					// ���� ���� Enemy ������ ����� �����̶��
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

	// ���� �����̿� �ִ� ������ �����ϴ� ����
	private void AttackAI()
	{
		GameObject startP = null, destP = null;

		// �Ÿ�, ���� ü��, ���� ���� �켱 ������ �����ȴ�.
		// ���� = (dist/2) + hp + 2*(team)
		float dest_total = float.MaxValue-1;

		// ���� ���� Enemy�� ��� ������Ʈ�� �ҷ��´�.
		for (int i = 0; i < GameManager.Instance.arrEnemy.Count; i++)
		{
			if (GameManager.Instance.arrEnemy[i].GetComponentInChildren<SlimeBaseSC>().Health >= ai.stopAttackHP)
			{
				// ��� �ٸ��� ���� ������ �ҷ��´�.
				for (int j = 0; j < BridgeManager.Instance.bridgeCount; j++)
				{
					GameObject dummy_connect_obj = BridgeManager.Instance.CheckConnect(GameManager.Instance.arrEnemy[i], j);
					// ���� ���� Enemy ������ ����� �����̶��
					// ���� ���ΰ� Ȯ��.
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

	// ���� ��� ���� ������ ������ ������ �� ã��.
	// ã�Ҵٸ� ���� maxAttackCount�� Ȯ��.
	// maxAttackCount���� ũ�ٸ� ������ ����� ������ �Ŀ� ȸ���� ���� ��� ����.
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
		// ���� �� ������ ���� �߰� ȸ�� ����
		if (index != -1)
		{
			Debug.Log("emergency emergency!!\n" + index);

			GameObject destP = GameManager.Instance.arrEnemy[index];
			Debug.Log("emergency destp = " +destP);

			int dummy_count = 0;
			for (int i = 0; i < attackListCount; i++)
			{ if (attackTypeList[i] == ENEMYATTACKTYPE.SUPPORT) dummy_count++; }
			// ������ ������ ȸ���ϴ� ����� �������
			if (dummy_count < ai.maxRechargeCount)
			{
				float dest_total = float.MinValue;
				GameObject startP = null;
				
				// ���� ���
				for (int i = 0; i < GameManager.Instance.arrEnemy.Count; i++)
				{
					if (i != index)
					{
						// ���� ���� Enemy ������ ����� �����̶��
						// ���� ���ΰ� Ȯ��.
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
				// ���� �����ϴ� ����� ������ ���� ��� ���� ������ ����� ���
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

	// ���� ��� ������ �� Ȯ��
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

	// ���� ���� ���ݿ� ���� �ǰ������� ���� ���� ��ȯ ���� �޶���.
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

	// P1�� P2�� �����ϴ� ����Ʈ�� �ִ��� Ȯ��.
	private bool CheckAttack(GameObject P1, GameObject P2)
	{
		for (int i = 0; i < attackListCount; i++)
		{
			if (P1.Equals(attackList[0][i]) && P2.Equals(attackList[1][i]))
				return true;
		}
		return false;
	}

	// üũüũ
	private void CheckState()
	{
		// Ȧ�� ���� ����
		if (GameManager.Instance.arrEnemy.Count == 1)
		{
			// �߾� ������ �߰��� ������?
		}
		// ������ �������� �ٵ� ���ݰ����� ���ؿ�
		else
		{
			int hp = int.MaxValue;
			int index = -1;

			// ���� ���� �ǰ� ���� ������ ã��.
			for (int i = 0; i < GameManager.Instance.arrEnemy.Count; i++)
			{
				// ���� ���� ü���� maxSupportHP /2 ���� ���� ��쿡 ã�� �Ϸ��� �Ͽ���.
				// �㳪 ���� ������ �ƹ��͵� ���� ����. �ᱹ ���� ���� ���� ü�º��� ���� �����̹Ƿ� ü�� Ȯ�� X
				SlimeBaseSC dummy_sc = GameManager.Instance.arrEnemy[i].GetComponentInChildren<SlimeBaseSC>();
				if (dummy_sc.Health < hp)
				{ index = i; hp = dummy_sc.Health; }
			}

			if (index != -1)
			{
				GameObject destP = GameManager.Instance.arrEnemy[index];
				float dest_total = float.MinValue;
				GameObject startP = null;

				// ���� ���
				for (int i = 0; i < GameManager.Instance.arrEnemy.Count; i++)
				{
					if (i != index)
					{
						GameObject dummy_obj = GameManager.Instance.arrEnemy[i];
						// ���� ���� Enemy ������ ����� �����̶��
						// ���� ���ΰ� Ȯ��.
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


	
	// Ư�� ���� �� �ΰ��� ������ �մ� �ٸ��� �����ϸ�, ������ ���� ������ ������ ����.
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

	// Ư�� ���� �� �ΰ��� ������ �մ� �ٸ��� �����Ѵٴ� ������ ����.
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

	// P1�� ������ ���, P2�� ���� ����. attackList�� �ε��� �ΰ��� �����ϱ� ���� �ʿ��� �Լ�
	private int FindAttackIndex(GameObject P1, GameObject P2)
	{
		for (int i = 0; i < this.attackListCount; i++)
		{
			if (attackList[0][i].Equals(P1) && attackList[1][i].Equals(P2))
				return i;
		}
		return -1;
	}

	// ���۽����� ���ݰ�ΰ� �ִ� ���·� ���� ����Ǿ��� ��츦 ����ؼ� ���� �Լ�
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
