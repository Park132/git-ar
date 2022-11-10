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
	
	public ENEMYTYPE e_type;
	private ENEMYCHAR e_char;
	private int minAttackCount;
	private int skills;
	private int emergencyHP;
	private float delayThink;
	private List<GameObject> arrObj;
	private List<List<GameObject>> attackList;
	private int attackListCount;
	private StructorCollector.DestinationSet des;

	private void Start()
	{
		arrObj = new List<GameObject>();
		attackList = new List<List<GameObject>>();
		attackList.Add(new List<GameObject>());
		attackList.Add(new List<GameObject>());
		attackListCount = 0;
	}

	public void SettingEnemyType(ENEMYTYPE t)
	{
		this.e_type = t;
	}

	public void StartAI()
	{
		switch (e_type)
		{
			case ENEMYTYPE.TUTORIAL:
				this.minAttackCount = 1;
				this.e_char = ENEMYCHAR.DEFENSIVE;
				delayThink = 2f;
				skills = 1;
				emergencyHP = 5;
				break;
			case ENEMYTYPE.NORMAL:
				this.minAttackCount = 1;
				this.e_char = ENEMYCHAR.DEFENSIVE;
				delayThink = 1f;
				skills = 2;
				emergencyHP = 10;
				break;
		}
		
		//arrObj = GameManager.Instance.arrNone.ConvertAll<GameObject>;
	}

	private IEnumerator UpdateAI()
	{
		// ���� ������ minAttackCount���� ���� �ϴ��� Ȯ��.
		if (this.attackListCount < minAttackCount)
		{
			this.AttackAI();
		}
		else
		{
			for (int i = 0; i < this.attackListCount; i++)
			{
				SlimeBaseSC dummy_sl = attackList[1][i].GetComponentInChildren<SlimeBaseSC>();
				if (dummy_sl.state == TEAM.ENEMY)
				{
					if (dummy_sl.Health > this.emergencyHP)
					{
						this.OrderStopAttack(attackList[0][i], attackList[1][i]);
					}
				}
			}
		}

		yield return new WaitForSeconds(delayThink);
	}

	// ���� �����̿� �ִ� ������ �����ϴ� ����
	private void AttackAI()
	{
		GameObject startP = null, destP = null;
		float dist = float.MaxValue-1;
		// ���� ���� Enemy�� ��� ������Ʈ�� �ҷ��´�.
		for (int i = 0; i < GameManager.Instance.arrEnemy.Count; i++)
		{
			// ��� �ٸ��� ���� ������ �ҷ��´�.
			for (int j = 0; j < BridgeManager.Instance.bridgeCount; j++)
			{
				GameObject dummy_connect_obj = BridgeManager.Instance.CheckConnect(GameManager.Instance.arrEnemy[i], j);
				// ���� ���� Enemy ������ ����� �����̶��
				if (!ReferenceEquals(dummy_connect_obj,null))
				{
					// ���� ����� ������ �߸� �����̶��
					if (dummy_connect_obj.GetComponent<SlimeBaseSC>().state == TEAM.NONE)
					{
						if (!CheckAttack(dummy_connect_obj, GameManager.Instance.arrEnemy[i]))
						{

							float dummy_dist = Vector3.Distance(dummy_connect_obj.transform.position, GameManager.Instance.arrEnemy[i].transform.position);
							if (dist > dummy_dist)
							{
								startP = GameManager.Instance.arrEnemy[i]; destP = dummy_connect_obj;
								dist = dummy_dist;
							}
						}
					}
				}
			}

		}
		if (!ReferenceEquals(destP, null))
		{
			attackList[0].Add(startP); attackList[1].Add(destP);
			attackListCount++;
			OrderAttack(startP, destP);
		}
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

	
	// Ư�� ���� �� �ΰ��� ������ �մ� �ٸ��� �����ϸ�, ������ ���� ������ ������ ����.
	private void OrderAttack(GameObject P1, GameObject P2)
	{
		des.SetP1 = P1.GetComponentInChildren<SlimeBaseSC>().gameObject;
		des.SetP2 = P2.GetComponentInChildren<SlimeBaseSC>().gameObject;


		GameObject dummy = GameObject.Instantiate(BridgeManager.Instance.bridge_obj);
		dummy.name = des.SetP1.name + "_attack_" + des.SetP2.name;

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
		{ attackList[0].RemoveAt(dummy_index); attackList[1].RemoveAt(dummy_index); }
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
}
