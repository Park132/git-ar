using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// struct와 enum의 모음집

public enum TEAM
{ PLAYER, ENEMY, NONE }
public enum GAMESTATE
{ READY, START, PAUSE }
public enum PLATESTATE
{ UNCLICKED = 0, CLICKED = 1, CANCLICK = 2, CANUSESKILL = 3, CANCLE = 4 }
public enum ENEMYTYPE
{ TUTORIAL, NORMAL }
public enum ENEMYCHAR
{DEFENSIVE, AGRESSIVE  }
public class StructorCollector : MonoBehaviour
{
    public const float BASESPEED = 0.025f;
    public const float BASEDELAYATTACK = 1.5f;
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
}
