using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBridge : MonoBehaviour
{
    
    
    private StructorCollector.SoldierSetting sordierOrder;
    private SlimeBaseSC SPB, DPB;
    private float DelayAttack;
    private bool once;

    public IEnumerator SpawnIE;

	private void Start()
	{
        once = false; DelayAttack = 1.5f;
	}
	// 구조체를 받아온 후 초기값 세팅.
	public void SetSD(StructorCollector.DestinationSet des, TEAM settingT)
    {
        if (!once)
        {
            once = true;
            
            sordierOrder.Start_Point = des.SetP1;
            sordierOrder.Destination_Point = des.SetP2;
            sordierOrder.team = settingT;
            sordierOrder.AttackDamage = 1;
            sordierOrder.Speed = 1f;

            if (ReferenceEquals(des.SetP1, null) || ReferenceEquals(des.SetP2, null))
                Destroy(this.gameObject);
            SPB = sordierOrder.Start_Point.GetComponent<SlimeBaseSC>();
            DPB = sordierOrder.Start_Point.GetComponent<SlimeBaseSC>();
            SpawnIE = SpawnSoldier();
            StartCoroutine(SpawnIE);
        }
    }

    private IEnumerator SpawnSoldier()
    {
        while (true)
        {
            yield return new WaitForSeconds(DelayAttack);
            if (SPB.Health > 5 && GameManager.Instance.gameState == GAMESTATE.START)
            {
                SPB.Health--;
                SPB.SlimeScaleChange();
                GameObject dummy = GameObject.Instantiate(PrefabManager.Instance.Prefabs[0]);
                dummy.transform.parent = this.transform;
                dummy.transform.position = sordierOrder.Start_Point.transform.position;
                dummy.GetComponent<SlimeSoldierSC>().Setting(sordierOrder);
            }
        }
    }

    public void StopSpawn() { StopCoroutine(SpawnIE); }
}
