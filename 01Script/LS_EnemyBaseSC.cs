using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LS_EnemyBaseSC : MonoBehaviour
{

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
	private int minBaseDestination;
	private int skills;
	private int emergencyHP;
	private float delayThink;
	private List<GameObject> arrObj;
	private List<List<GameObject>> attackList;
	private int attackListCount;

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
				this.minBaseDestination = 1;
				this.e_char = ENEMYCHAR.DEFENSIVE;
				delayThink = 2f;
				skills = 1;
				emergencyHP = 5;
				break;
			case ENEMYTYPE.NORMAL:
				this.minBaseDestination = 2;
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
		if (GameManager.Instance.arrEnemy.Count < minBaseDestination)
		{

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

		}
	}

	private bool CheckAttack(GameObject P1, GameObject P2)
	{
		for (int i = 0; i < attackListCount; i++)
		{
			if (P1.Equals(attackList[0][i]) && P2.Equals(attackList[1][i]))
				return true;
		}
		return false;
	}


	private void OrderAttack(GameObject P1, GameObject P2)
	{
		GameObject SetP1 = P1.GetComponentInChildren<SlimeBaseSC>().gameObject;
		GameObject SetP2 = P2.GetComponentInChildren<SlimeBaseSC>().gameObject;

		bool exist = false;
		SlimeBaseSC p1_sc = SetP1.GetComponent<SlimeBaseSC>();
		for (int i = 0; i < p1_sc.atkObj.Count; i++)
		{
			if (p1_sc.atkObj[i].name.Equals(SetP1.name + "_attack_" + SetP2.name))
			{ exist = true; break; }
		}

		if (!exist)
		{
			GameObject dummy = GameObject.Instantiate(bridge);
			dummy.name = des.SetP1.name + "_attack_" + des.SetP2.name;

			dummy.transform.parent = GameManager.Instance.attackObjs.transform;
			dummy.GetComponent<SlimeBridge>().SetSD(des, TEAM.PLAYER);
			des.SetP1.GetComponent<SlimeBaseSC>().atkObj.Add(dummy);
			des.SetP1.GetComponent<SlimeBaseSC>().ChangeSoldierPower(dummy);
		}
		if (!ReferenceEquals(des.SetP1, null))
		{
			des.SetP1.GetComponent<SlimeBaseSC>().ChangeState(PLATESTATE.UNCLICKED);
			BridgeManager.Instance.ConnectedPanChange(des.SetP1.transform.parent.gameObject, PLATESTATE.UNCLICKED);
			des.SetP1 = null;
		}
		if (!ReferenceEquals(des.SetP2, null))
		{
			des.SetP2.GetComponent<SlimeBaseSC>().ChangeState(PLATESTATE.UNCLICKED);
			des.SetP2 = null;
		}
	}
}
