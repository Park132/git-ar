using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBaseSC : Slime_Stat
{
    public PLATESTATE clickState;
	public GameObject[] bases;
	private TEAM finalAttack, currentTeam;
	private bool canChanged = false;
	private IEnumerator recharging;
	public MeshRenderer plate;
	public GameObject area;
	private Color32[] arrColor;
	[SerializeField]private int nearCount, currentNearCount;
	public List<GameObject> atkObj;
	private float multipleNum;
	

	protected override void Start()
	{
		base.Start();
		if (this.state == TEAM.NONE) canChanged = true;
		recharging = ReChargeSlime();
		StartCoroutine(recharging);

		currentTeam = TEAM.NONE;
		multipleNum = 1;
		atkObj = new List<GameObject>();
		arrColor = new Color32[] { new Color32(43, 97, 19, 255),
		new Color32(102, 188, 66, 255),new Color32(50, 152, 186, 255)
		,new Color32(210, 198, 54, 255), new Color32(174, 34, 41, 255)};
		this.ChangeState(PLATESTATE.UNCLICKED);
	}

	public override void SlimeScaleChange()
	{
		base.SlimeScaleChange();
		float sizeDummy = (Mathf.Max(3f, Mathf.Min(Health * 0.15f, 7.5f)));
		box.size = Vector3.one * (5 / sizeDummy);
		//nav.radius = this.transform.localScale.x * 0.5f;
	}

	protected void Update()
	{
		if (!canChanged && GameManager.Instance.gameState == GAMESTATE.START)
		{
			this.area.transform.localScale = Vector3.one * (GameManager.Instance.distanceEP/5)/1.8f;
			nearCount = 0;
			for (int i = 0; i < GameManager.Instance.arrNone.Count; i++)
			{
				float dummy_dis = Vector3.Distance(GameManager.Instance.arrNone[i].transform.position, this.transform.position);
				// 중립 진영과의 거리가 원 내부에 있다면 nearCount 추가
				if (GameManager.Instance.distanceEP / 2 +0.5f >= dummy_dis)
					nearCount++;
				// 중립 진영이 플레이어/적군 진영의 거리보다 더 멀리 떨어져있다면 빼기
				else if (dummy_dis >= GameManager.Instance.distanceEP)
					nearCount--;
			}
			if (currentNearCount != nearCount)
			{
				multipleNum = GameManager.Instance.marker.markerLen - nearCount;
				for (int i = 0; i < atkObj.Count; i++)
				{ChangeSoldierPower(atkObj[i]);}

			}
			currentNearCount = nearCount;
		}
	}

	private IEnumerator ReChargeSlime()
	{
		while (true)
		{
			yield return new WaitForSeconds(3f);
			if (this.state != TEAM.NONE && GameManager.Instance.gameState == GAMESTATE.START)
			{
				this.Health++;
				this.SlimeScaleChange();
			}
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
		this.Attack = Mathf.Max(1, Mathf.CeilToInt(multipleNum - 2 /2));
		float speed = Mathf.Round(StructorCollector.BASESPEED * Mathf.Max(0.5f, multipleNum / 3)*1000)/1000;
		float del = Mathf.Round(StructorCollector.BASEDELAYATTACK * (1.5f - 0.2f*multipleNum) * 1000) / 1000;
		obj.GetComponent<SlimeBridge>().SettingAtkSpeedDelay(this.Attack, speed, del);
	}

	// 마지막으로 공격을 한 슬라임의 팀으로 변경. 초기 체력 10 지급.
	private void OnTriggerEnter(Collider other)
	{
		if (other.transform.CompareTag("Slime"))
		{
			finalAttack=other.GetComponent<SlimeSoldierSC>().state;
			if (Health <= 1 && canChanged)
			{
				int visibleNum = 0;
				bool changeT = (currentTeam == TEAM.NONE) ? false : true;
				switch (finalAttack)
				{
					case TEAM.PLAYER:
						this.state = TEAM.PLAYER;
						visibleNum = 1;
						if (changeT)
						{GameManager.Instance.arrEnemy.Remove(this.gameObject);}
						GameManager.Instance.arrPlayer.Add(this.gameObject);
						break;
					case TEAM.ENEMY:
						this.state = TEAM.ENEMY;
						visibleNum = 2;
						if (changeT)
						{ GameManager.Instance.arrPlayer.Remove(this.gameObject); }
						GameManager.Instance.arrEnemy.Add(this.gameObject);
						break;
				}
				for (int i = 0; i < 3; i++)
				{
					if (i == visibleNum) bases[i].SetActive(true);
					else bases[i].SetActive(false);
				}
				
				this.Health = 10;
			}
		}
	}
}
