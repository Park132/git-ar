using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SlimeSoldierSC : Slime_Stat
{
	//private NavMeshAgent nav;
	protected Animator anim;
	private StructorCollector.SoldierSetting order;
	private GameObject destination = null;
	private Vector3 moveVec;

	// public ParticleSystem hit_effect; // PS_ 베이스 또는 병사와 부딛혔을 때, 발생하는 이펙트

	protected override void Awake()
	{
		base.Awake();
		//nav = this.GetComponent<NavMeshAgent>();
		anim = this.GetComponent<Animator>();
		Destroy(this.gameObject, 10f);
	}

	protected void Update()
	{
		//nav.destination = order.Destination_Point.transform.position;
		if (!ReferenceEquals(destination, null))
		{
			moveVec = (destination.transform.position - this.transform.position).normalized;
			this.transform.position += moveVec * order.Speed;
		}
	}

	public override void SlimeScaleChange()
	{
		base.SlimeScaleChange();
		box.size = Vector3.one *0.5f;
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
		this.transform.LookAt(order.Destination_Point.transform.position);
		Debug.Log(order.Speed);
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
				Destroy(this.gameObject);
			}
			else
			{
				desSB.Health += order.AttackDamage;
				desSB.SlimeScaleChange();
				Destroy(this.gameObject);
			}
		}
	}

	void Effect_hit() // PS_부딛혔을 때, 이펙트를 발생시킬 함수
    {
		// Instantiate(hit_effect, this.transform.position, this.transform.rotation);
    }
}
