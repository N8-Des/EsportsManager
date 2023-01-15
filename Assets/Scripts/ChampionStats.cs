using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Champion Data")]
public class ChampionStats : ScriptableObject
{
    public Sprite championIcon;
    [Header("Stats")]
    public float attack;
    public float defense;
    public float CC;
    public float teamfight;
    public float dueling;
    public float combatSpeed;
    public float outOfCombatSpeed;
    public float mobility;
    public float sustain;
    public float teamHealing;
    public float scaling;
    public float turretPush;
}
