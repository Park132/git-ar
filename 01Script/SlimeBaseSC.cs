using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBaseSC : Slime_Stat
{
    public bool Clicked;
	public GameObject[] bases;
	private TEAM finalAttack;
	private bool canChanged = false;
	private IEnumerator recharging;

	protected override void Start()
	{
		base.Start();
		if (this.state == TEAM.NONE) canChanged = true;
		recharging = ReChargeSlime();
		StartCoroutine(recharging);
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
