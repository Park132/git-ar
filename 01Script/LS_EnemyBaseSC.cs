using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LS_EnemyBaseSC : MonoBehaviour
{

	// ΩÃ±€≈Ê /////
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
	private float delayThink;
	private List<GameObject> arrObj;

	private void Start()
	{
		arrObj = new List<GameObject>();
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
				break;
			case ENEMYTYPE.NORMAL:
				this.minBaseDestination = 1;
				this.e_char = ENEMYCHAR.DEFENSIVE;
				delayThink = 1f;
				skills = 2;
				break;
		}
		
		//arrObj = GameManager.Instance.arrNone.ConvertAll<GameObject>;
	}

	private IEnumerator UpdateAI()
	{
		yield return new WaitForSeconds(delayThink);
	}

}
