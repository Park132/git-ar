using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBaseSC : Slime_Stat
{
    public PANSTATE clickState;
	public GameObject[] bases;
	private TEAM finalAttack;
	private bool canChanged = false;
	private IEnumerator recharging;
	public MeshRenderer Pan;
	private Color32[] arrColor;
	

	protected override void Start()
	{
		base.Start();
		if (this.state == TEAM.NONE) canChanged = true;
		recharging = ReChargeSlime();
		StartCoroutine(recharging);
		
		arrColor = new Color32[] { new Color32(43, 97, 19, 255),
		new Color32(102, 188, 66, 255),new Color32(50, 152, 186, 255)
		,new Color(210, 198, 54, 255)};
		this.ChangeState(PANSTATE.UNCLICKED);
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

	public void ChangeState(PANSTATE s)
	{
		if (s != PANSTATE.NONE)
		{ this.clickState = s; }
		Pan.material.color = arrColor[(int)clickState];
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
				switch (finalAttack)
				{
					case TEAM.PLAYER:
						this.state = TEAM.PLAYER;
						visibleNum = 1;
						break;
					case TEAM.ENEMY:
						this.state = TEAM.ENEMY;
						visibleNum = 2;
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
