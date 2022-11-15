using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

// struct�� enum�� ������

public enum TEAM
{ PLAYER, ENEMY, NONE }
public enum GAMESTATE
{ READY, START, PAUSE }
public enum PLATESTATE
{ UNCLICKED = 0, CLICKED = 1, CANCLICK = 2, CANUSESKILL = 3, CANCLE = 4 }
public enum ENEMYTYPE
{ TUTORIAL, NORMAL, HARD }
public enum ENEMYCHAR
{DEFENSIVE, AGRESSIVE  }
public enum ENEMYATTACKTYPE
{ ATTACK = 0, SUPPORT = 1, EMERGENCY = 2, RECHARGING = 3 }
public class StructorCollector : MonoBehaviour
{
    public const float BASESPEED = 0.04f;
    public const float BASEDELAYATTACK = 1.5f;
    public const float BASERECHARGEDELAY = 1.5f;
    [Serializable] public struct SoldierSetting
    {
        public GameObject Start_Point, Destination_Point;
        public TEAM team;
        public int AttackDamage;
        public float Speed;
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
        public float delayThink;
        public int skills;
        
		public ENEMYCHAR e_char;
        public int maxRechargeCount;
        public int emergencyHP; ////
		public int stopAttackHP;
		public int maxSupportHP;

        public AI_Setting(ENEMYTYPE et, int mine, int maxa, float delayt, int sk, ENEMYCHAR ch)
        {
            e_type = et; minEmergencyBase = mine; maxAttackCount = maxa; delayThink = delayt; skills = sk;
            e_char = ch;
            
            switch (ch)
            {
                case ENEMYCHAR.AGRESSIVE:
                    maxRechargeCount = 2;
                    emergencyHP = 10;
                    stopAttackHP = 13;
                    maxSupportHP = 25;
                    break;
                case ENEMYCHAR.DEFENSIVE:
                    maxRechargeCount = 2;
                    emergencyHP = 17;
                    stopAttackHP = 20;
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
