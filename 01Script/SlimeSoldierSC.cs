using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SlimeSoldierSC : Slime_Stat
{
	//private NavMeshAgent nav;
	protected Animator anim;
	private StructorCollector.SoldierSetting order;
	private GameObject destination = null, spawnpoint = null;
	private Vector3 moveVec;
	private Vector3 moveLength = Vector3.zero;

	protected override void Awake()
	{
		base.Awake();
		//nav = this.GetComponent<NavMeshAgent>();
		anim = this.GetComponent<Animator>();
		anim.SetBool("Move", true);
		Destroy(this.gameObject, 20f);

	}

	protected void Update()
	{
		//nav.destination = order.Destination_Point.transform.position;
		if (!ReferenceEquals(destination, null))
		{
			moveVec = (destination.transform.position - spawnpoint.transform.position).normalized;
			this.transform.rotation = Quaternion.LookRotation(moveVec);
			
			if (Vector3.Distance(moveLength + spawnpoint.transform.position , spawnpoint.transform.position)
				> Vector3.Distance(spawnpoint.transform.position, destination.transform.position))
				moveLength -= moveVec * order.Speed * Time.deltaTime;
			else
				moveLength += moveVec * order.Speed * Time.deltaTime;

			this.transform.position = spawnpoint.transform.position + moveLength;
		}
	}

	public override void SlimeScaleChange()
	{
		base.SlimeScaleChange();
		box.size = Vector3.one * 1.5f;
		//nav.radius = this.transform.localScale.x * 0.5f;
	}

	// 슬라임 병사의 목적지를 지정하는 함수
	//
	public void Setting(StructorCollector.SoldierSetting bridge_order)
	{
		// 구조체 깊은 복사
		order = new StructorCollector.SoldierSetting(bridge_order);
		this.destination = order.Destination_Point;
		this.spawnpoint = order.Start_Point;
		this.transform.LookAt(order.Destination_Point.transform.position);
		this.Attack = order.AttackDamage;
	}

	public override IEnumerator Damaged(int damage)
	{
		float delayChangeColor = 0.4f;
		Health -= damage;
		SlimeScaleChange();
		anim.SetTrigger("Damaged");
		foreach (SkinnedMeshRenderer rd in rds)
			rd.material.color = Color.red;
		yield return new WaitForSeconds(delayChangeColor);
		for (int i = 0; i < rds.Length; i++)
		{ rds[i].material.color = colors[i]; }
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.transform.gameObject == order.Destination_Point)
		{
			SlimeBaseSC desSB = order.Destination_Point.GetComponent<SlimeBaseSC>();
			if (desSB.state != order.team)
			{
				desSB.Damage_Start(order.AttackDamage);
				StartCoroutine(AttackAnim());
			}
			else
			{
				desSB.Health += order.AttackDamage;
				desSB.SlimeScaleChange();
				Destroy(this.gameObject);
			}
		}
	}

	private IEnumerator AttackAnim()
	{
		this.box.enabled = false;
		order.Speed = 0;
		anim.SetTrigger("Attack");
		LS_AudioManager.Instance.SFX_spawn(1, this.transform.position);
		yield return new WaitForSeconds(0.5f);
		Destroy(this.gameObject);
	}
}
