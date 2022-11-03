using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SlimeSoldierSC : Slime_Stat
{
	//private NavMeshAgent nav;
	protected Animator anim;
	private StructorCollector.SoldierSetting order;
	public float speed;
	private GameObject destination = null;
	private Vector3 moveVec;

	protected override void Awake()
	{
		base.Awake();
		//nav = this.GetComponent<NavMeshAgent>();
		anim = this.GetComponent<Animator>();
		Destroy(this.gameObject, 10f);
	}

	protected override void Update()
	{
		//nav.destination = order.Destination_Point.transform.position;
		if (!ReferenceEquals(destination, null))
		{
			moveVec = (destination.transform.position - this.transform.position).normalized;
			this.transform.position += moveVec * speed;
		}
	}

	public override void SlimeScaleChange()
	{
		base.SlimeScaleChange();
		box.size = this.transform.localScale * 0.5f;
		//nav.radius = this.transform.localScale.x * 0.5f;
	}

	// 슬라임 병사의 목적지를 지정하는 함수
	//
	public void Setting(StructorCollector.SoldierSetting bridge_order)
	{
		order = bridge_order;
		//nav.destination = order.Destination_Point.transform.position;
		//nav.speed = order.Speed;
		this.destination = order.Destination_Point;
		this.speed = order.Speed;
		this.Attack = order.AttackDamage;
		this.transform.LookAt(order.Destination_Point.transform.position);
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
				desSB.Damage_Start(this.Attack);
				Destroy(this.gameObject);
			}
			else
			{
				desSB.Health += this.Attack;
				desSB.SlimeScaleChange();
				Destroy(this.gameObject);
			}
		}
	}
}
