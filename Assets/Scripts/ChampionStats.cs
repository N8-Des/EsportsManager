using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Champion Data")]
public class ChampionStats : ScriptableObject
{

    public Sprite championIcon;
    public string championName;
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
    public float poke;

    public List<float> stats = new List<float>();
    //the sacrifice I am making for JSON.
    public void CreateList()
    {
        stats.Clear();
        stats.Add(attack);
        stats.Add(defense);
        stats.Add(CC);
        stats.Add(teamfight);
        stats.Add(dueling);
        stats.Add(combatSpeed);
        stats.Add(outOfCombatSpeed);
        stats.Add(mobility);
        stats.Add(sustain);
        stats.Add(teamHealing);
        stats.Add(scaling);
        stats.Add(turretPush);
        stats.Add(poke);
    }
}
