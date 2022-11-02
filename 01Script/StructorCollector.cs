using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// struct와 enum의 모음집

public enum TEAM
{ PLAYER, ENEMY, NONE }
public enum GAMESTATE
{ READY, START, PAUSE }
public class StructorCollector : MonoBehaviour
{
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
