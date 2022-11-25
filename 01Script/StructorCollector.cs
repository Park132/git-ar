using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

// struct와 enum의 모음집

public enum TEAM
{ PLAYER, ENEMY, NONE }
public enum GAMESTATE
{ MAIN, READY, START, PAUSE=0, WIN=1, OVER=2, READYFORSTART, SKILLTIME }
public enum PLATESTATE
{ UNCLICKED = 0, CLICKED = 1, CANCLICK = 2, CANUSESKILL = 3, CANCLE = 4 }
public enum ENEMYTYPE
{ TUTORIAL, NORMAL, HARD }
public enum ENEMYCHAR
{DEFENSIVE, AGGRESSIVE  }
public enum ENEMYATTACKTYPE
{ ATTACK = 0, SUPPORT = 1, EMERGENCY = 2, RECHARGING = 3, SUDDENLY = 4, CHECKATTACK = 5, FINALATTACK }
public class StructorCollector : MonoBehaviour
{
    public const float BASESPEED = 3.5f;
    public const float BASEDELAYATTACK = 1.5f;
    public const float BASERECHARGEDELAY = 1.5f;
    public const float BASESKILLTIMES = 15f;

    [Serializable] public struct SoldierSetting
    {
        public GameObject Start_Point, Destination_Point;
        public TEAM team;
        public int AttackDamage;
        public float Speed;
        public SoldierSetting(SoldierSetting s)
        {
            this.Start_Point = s.Start_Point; this.Destination_Point = s.Destination_Point;
            this.team = s.team; this.AttackDamage = s.AttackDamage; this.Speed = s.Speed;
        }
    }

    [Serializable] public struct DestinationSet
    { public GameObject SetP1, SetP2; }

    [Serializable] public struct Markers
    {
        public List<GameObject> markerObj;
        public List<bool> markerExist;
        public List<TEAM> markerTeam;
        public int markerLen;
    }
    [Serializable] public struct AI_Setting
    {
        public ENEMYTYPE e_type;
        public int minEmergencyBase;
		public int maxAttackCount;
        public float multipler;
        public float delayCheckAttack;
        public float delayThink;
        
        
		public ENEMYCHAR e_char;
        public int maxRechargeCount;
        public int emergencyHP; ////
		public int stopAttackHP;
		public int maxSupportHP;

        public AI_Setting(ENEMYTYPE et, int mine, int maxa, float delayt, float sk, float dca,ENEMYCHAR ch)
        {
            e_type = et; minEmergencyBase = mine; maxAttackCount = maxa; delayThink = delayt; multipler = sk; delayCheckAttack = dca;
            e_char = ch;
            
            switch (ch)
            {
                case ENEMYCHAR.AGGRESSIVE:
                    maxRechargeCount = 2;
                    emergencyHP = 10;
                    stopAttackHP = 7;
                    maxSupportHP = 30;
                    break;
                case ENEMYCHAR.DEFENSIVE:
                    maxRechargeCount = 2;
                    emergencyHP = 17;
                    stopAttackHP = 13;
                    maxSupportHP = 40;
                    break;
                default:
                    maxRechargeCount = 1; emergencyHP = 1; stopAttackHP = 1; maxSupportHP = 1;
                    break;
            }
        }

    }
    [Serializable] public struct AI_CampCheck
    {
        public GameObject obj;
        public SlimeBaseSC obj_sc;
        public List<Bridge_Info> connectedObj;
        public AI_CampCheck(GameObject h, SlimeBaseSC t, List<Bridge_Info> b)
        { obj = h; obj_sc = t; connectedObj = b.ToList(); }
    }
    [Serializable] public struct Bridge_Info
    {
        public GameObject obj;
        public SlimeBaseSC obj_sc;
        public Bridge_Info(GameObject o, SlimeBaseSC t) { obj = o; obj_sc = t; }
    }
    [Serializable] public struct AI_StateofAttack
    {
        public int index;
        public int enoughHealth;
        public int playerHealth;
        public AI_StateofAttack(int i, int e, int p) { index = i; enoughHealth = e; playerHealth = p; }

    }
}
