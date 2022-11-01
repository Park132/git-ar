using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Slime_Stat : MonoBehaviour
{
    public int Health;
	public int Attack;

	protected SkinnedMeshRenderer[] rds;
	protected Color[] colors;

	public TEAM state;


	protected BoxCollider box;
	

	protected virtual void Awake()
	{
		box = this.GetComponent<BoxCollider>();
		
		// ������ �ǰ� �� ���� ���ϰ� �ϱ� ����.
		rds = this.GetComponentsInChildren<SkinnedMeshRenderer>();
		colors = new Color[rds.Length];
		for (int i = 0; i < rds.Length; i++)
		{ colors[i] = rds[i].material.color; }
	}

	protected void Start()
	{
		SlimeScaleChange();
	}

	protected virtual void Update()
	{
		// ������
		if (Input.GetKeyDown("a"))
		{
			Damage_Start(1);

		}
	}

	// �������� �ִ� �ּ� ũ�� ����
	protected virtual void SlimeScaleChange()
	{
		this.transform.localScale = Vector3.one * (Mathf.Max(0.5f, Mathf.Min(Health * 0.05f, 3)));		
	}

	// �������� ���� �� �ణ�� ������ ���� ���ƿ�
	public void Damage_Start(int damage) { StartCoroutine(Damaged(damage)); }
	public virtual IEnumerator Damaged(int damage)
    {
		float delayChangeColor = 0.4f;
		Health -= damage;
		SlimeScaleChange();
		foreach (SkinnedMeshRenderer rd in rds)
			rd.material.color = Color.red;
		yield return new WaitForSeconds(delayChangeColor);
		for (int i = 0; i < rds.Length; i++)
		{rds[i].material.color = colors[i];}
    }
}
