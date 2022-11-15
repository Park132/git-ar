using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBaseSC : Slime_Stat
{
    public PLATESTATE clickState;
	public GameObject[] bases;
	public float rechargeDelay; // �ӵ�
	public int rechargeHP; // ��
	private TEAM finalAttack;
	private bool canChanged = false;
	private IEnumerator recharging;
	public MeshRenderer plate;
	public GameObject area;
	private Color32[] arrColor;
	[SerializeField]private int nearCount, currentNearCount;
	public List<GameObject> atkObj;	// �� ���̽����� ����ϴ� ��� ���ݸ�� ����
	private float multipleNum;
	public float[] arrSAD;
	

	protected override void Start()
	{
		arrSAD = new float[] { 1,1,1};
		base.Start();
		if (this.state == TEAM.NONE) canChanged = true;
		recharging = ReChargeSlime();
		StartCoroutine(recharging);

		multipleNum = 1;
		rechargeHP = 1;
		rechargeDelay = StructorCollector.BASERECHARGEDELAY;
		atkObj = new List<GameObject>();
		arrColor = new Color32[] { new Color32(43, 97, 19, 255),
		new Color32(102, 188, 66, 255),new Color32(50, 152, 186, 255)
		,new Color32(210, 198, 54, 255), new Color32(174, 34, 41, 255)};
		this.ChangeState(PLATESTATE.UNCLICKED);
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
			// �ʹ� �߸������� �ƴ� �� �� ���� ���̽� ķ���� ���
			if (!canChanged)
			{
				nearCount = 0;
				this.area.transform.localScale = Vector3.one * (GameManager.Instance.distanceEP / 5) / 1.8f;
				for (int i = 0; i < GameManager.Instance.arrNone.Count; i++)
				{
					float dummy_dis = Vector3.Distance(GameManager.Instance.arrNone[i].transform.position, this.transform.position);
					// �߸� �������� �Ÿ��� �� ���ο� �ִٸ� nearCount �߰�
					if (GameManager.Instance.distanceEP / 2 + 0.5f >= dummy_dis)
						nearCount++;
					// �߸� ������ �÷��̾�/���� ������ �Ÿ����� �� �ָ� �������ִٸ� ����
					else if (dummy_dis >= GameManager.Instance.distanceEP)
						nearCount--;
				}
			}
			// �ʹݿ� �߸������̾�����, ���� �߸��� �ƴ� �� ���� �������� ���.
			else if (this.state != TEAM.NONE)
			{
				switch (this.state)
				{
					case TEAM.PLAYER:
						nearCount = GameManager.Instance.stdPointSB.currentNearCount;
						break;
					case TEAM.ENEMY:
						nearCount = GameManager.Instance.enePointSB.currentNearCount;
						break;
				}
			}

			if (currentNearCount != nearCount)
			{
				multipleNum = GameManager.Instance.marker.markerLen - nearCount;
				for (int i = 0; i < atkObj.Count; i++)
				{ ChangeSoldierPower(atkObj[i]); }
			}
			currentNearCount = nearCount;
		}
	}

	private IEnumerator ReChargeSlime()
	{
		while (true)
		{
			yield return new WaitForSeconds(rechargeDelay * ((canChanged) ? 1.5f:1f));
			if (this.state != TEAM.NONE && GameManager.Instance.gameState == GAMESTATE.START)
			{
				this.Health+=rechargeHP;
				this.SlimeScaleChange();
			}
		}
	}

	public void ChangeState(PLATESTATE s)
	{
		this.clickState = s; 
		plate.material.color = arrColor[(int)clickState];
	}

	// ������ ���ݷ� ����
	public void ChangeSoldierPower(GameObject obj)
	{
		this.Attack = Mathf.RoundToInt(Mathf.Max(1, Mathf.CeilToInt(multipleNum /2)) *arrSAD[1]);
		float speed = Mathf.Round(StructorCollector.BASESPEED * Mathf.Max(0.5f, multipleNum / 3)*1000)/1000 *arrSAD[0];
		float del = Mathf.Round(StructorCollector.BASEDELAYATTACK * (1.5f - 0.2f*multipleNum) * 1000) / 1000 *arrSAD[2];
		obj.GetComponent<SlimeBridge>().SettingAtkSpeedDelay(this.Attack, speed, del);
	}

	// ���������� ������ �� �������� ������ ����. �ʱ� ü�� 10 ����.
	private void OnTriggerEnter(Collider other)
	{
		if (other.transform.CompareTag("Slime"))
		{
			finalAttack=other.GetComponent<SlimeSoldierSC>().state;
			if (Health <= 1 && canChanged)
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
					for (int i = 0; i < 3; i++)
					{
						if (i == visibleNum) bases[i].SetActive(true);
						else bases[i].SetActive(false);
					}
					StopOrderPreviousTeam();
					this.Health = 10;
				}
			}
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

	public void settingSkillSAD(float speed, float attack, float delay)
	{
		arrSAD[0] = speed; arrSAD[1] = attack; arrSAD[2] = delay;
	}
}
