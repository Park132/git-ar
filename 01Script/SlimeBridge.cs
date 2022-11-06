using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBridge : MonoBehaviour
{
    
    
    private StructorCollector.SoldierSetting soldierOrder;
    private SlimeBaseSC SPB, DPB;
    private float DelayAttack;
    private bool once;

    public IEnumerator SpawnIE;

	private void Start()
	{
        once = false; DelayAttack = StructorCollector.BASEDELAYATTACK;
	}
	// 구조체를 받아온 후 초기값 세팅.
	public void SetSD(StructorCollector.DestinationSet des, TEAM settingT)
    {
        if (!once)
        {
            once = true;
            
            soldierOrder.Start_Point = des.SetP1;
            soldierOrder.Destination_Point = des.SetP2;
            soldierOrder.team = settingT;
            soldierOrder.AttackDamage = 1;
            soldierOrder.Speed = StructorCollector.BASESPEED;

            if (ReferenceEquals(des.SetP1, null) || ReferenceEquals(des.SetP2, null))
                Destroy(this.gameObject);
            SPB = soldierOrder.Start_Point.GetComponent<SlimeBaseSC>();
            DPB = soldierOrder.Start_Point.GetComponent<SlimeBaseSC>();
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
                SPB.Health-=soldierOrder.AttackDamage;
                SPB.SlimeScaleChange();
                GameObject dummy = GameObject.Instantiate(PrefabManager.Instance.Prefabs[0]);
                dummy.transform.parent = this.transform;
                dummy.transform.position = soldierOrder.Start_Point.transform.position;
                dummy.GetComponent<SlimeSoldierSC>().Setting(soldierOrder);
            }
        }
    }

    public void SettingAtkSpeedDelay(int atk, float speed, float delay)
    {
        soldierOrder.Speed = speed;
        soldierOrder.AttackDamage = atk;
        this.DelayAttack = delay;
    }

    public void CancleAttack() {
        StopCoroutine(SpawnIE);
        for (int i = 0; i < this.transform.childCount; i++)
        {this.transform.GetChild(0).parent = GameManager.Instance.attackObjs.transform;}
        Destroy(this.gameObject, 1f);
    }
}
