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
		
		// 슬라임 피격 시 색을 변하게 하기 위함.
		rds = this.GetComponentsInChildren<SkinnedMeshRenderer>();
		colors = new Color[rds.Length];
		for (int i = 0; i < rds.Length; i++)
		{ colors[i] = rds[i].material.color; }
	}

	protected virtual void Start()
	{
		SlimeScaleChange();
	}


	// 슬라임의 최대 최소 크기 제한
	public virtual void SlimeScaleChange()
	{
		float sizeDummy = (Mathf.Max(3f, Mathf.Min(Health * 0.15f, 7.5f)));
		this.transform.localScale = Vector3.one * sizeDummy;

	}

	// 데미지를 받을 시 약간의 붉은색 이후 돌아옴
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
