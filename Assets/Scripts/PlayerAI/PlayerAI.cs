using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerAI : MonoBehaviour
{
    public PlayerStats myStats;
    public State currentState;
    public int totalGold;
    public bool blueTeam;
    public ChampionStats championStats;
    public int currentHealth;
    public int maxHealth;
    public NavMeshAgent agent;
    public GameManager gameManager;
    [HideInInspector]
    public float farmingTick;
    [HideInInspector]
    public float tradingTick;
    public List<PlayerAI> myTeam = new List<PlayerAI>();
    List<Turret> friendlyTurrets = new List<Turret>();
    [Header("Decision Values")]
    //these values differ from individual player stats, as they can be affected based on strategy and game state.
    public float performanceMultiplier = 1;
    public float roamScore = 1;
    public float aggressionScore = 1;
    public float pushScore = 1;


    [Header("PointsOfInterest")]
    public Turret topBlueTurretT1;
    public Turret topRedTurretT1;
    public Turret topBlueTurretT2;
    public Turret topRedTurretT2;
    public Turret midBlueTurretT1;
    public Turret midRedTurretT1;
    public Turret midBlueTurretT2;
    public Turret midRedTurretT2;
    public Turret botBlueTurretT1;
    public Turret botRedTurretT1;
    public Turret botBlueTurretT2;
    public Turret botRedTurretT2;
    public Turret baseTopBlueTurret;
    public Turret baseTopRedTurret;
    public Turret baseBotBlueTurret;
    public Turret baseBotRedTurret;
    Vector3 mySpawn;
    public SpriteRenderer characterIcon;
    public enum role
    { 
        TOP,
        MIDDLE,
        BOTTOM, 
        SUPPORT, 
        JUNGLE
    }
    public role myRole;

    public void Update()
    {
        currentState.stateTick(this);
    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
    }
    public void Activate(GameManager gm)
    {
        gameManager = gm;
        mySpawn = transform.position;
        agent.speed = championStats.outOfCombatSpeed;
        characterIcon.sprite = championStats.championIcon;
        if (blueTeam)
        {
            myTeam = gameManager.bluePlayers;
            //I really should've structured the way we save turrets better. LOL.
            friendlyTurrets.Add(topBlueTurretT1);
            friendlyTurrets.Add(topBlueTurretT2);
            friendlyTurrets.Add(midBlueTurretT1);
            friendlyTurrets.Add(midBlueTurretT2);
            friendlyTurrets.Add(botBlueTurretT1);
            friendlyTurrets.Add(botBlueTurretT2);
            friendlyTurrets.Add(baseBotBlueTurret);
            friendlyTurrets.Add(baseTopBlueTurret);
        }
        else
        {
            myTeam = gameManager.redPlayers;
            friendlyTurrets.Add(topRedTurretT1);
            friendlyTurrets.Add(topRedTurretT2);
            friendlyTurrets.Add(midRedTurretT1);
            friendlyTurrets.Add(midRedTurretT2);
            friendlyTurrets.Add(botRedTurretT1);
            friendlyTurrets.Add(botRedTurretT2);
            friendlyTurrets.Add(baseBotRedTurret);
            friendlyTurrets.Add(baseTopRedTurret);
        }

        //consistency check. This part is gonna be even worse to look at LOL

        float consistencyAmount = Mathf.Lerp(0.15f, 0, myStats.consistency / 100);
        float consistencyMultipler = Random.Range(1.0f - consistencyAmount, 1.0f + consistencyAmount);
        //multiply *EVERY VALUE* by consistency. 

        //myStats.aggression = (int)(myStats.aggression * consistencyAmount);
        myStats.allIn = (int)(myStats.allIn * consistencyMultipler);
        myStats.communication = (int)(myStats.communication * consistencyMultipler);
        myStats.composure = (int)(myStats.composure * consistencyMultipler);
        myStats.concentration = (int)(myStats.concentration * consistencyMultipler);
        myStats.decisionMaking = (int)(myStats.decisionMaking * consistencyMultipler);
        myStats.determination = (int)(myStats.determination * consistencyMultipler);
        myStats.farming = (int)(myStats.farming * consistencyMultipler);
        myStats.flair = (int)(myStats.flair * consistencyMultipler);
        myStats.leadership = (int)(myStats.leadership * consistencyMultipler);
        myStats.playmaking = (int)(myStats.playmaking * consistencyMultipler);
        myStats.positioning = (int)(myStats.positioning * consistencyMultipler);
        myStats.teamwork = (int)(myStats.teamwork * consistencyMultipler);
        myStats.trading = (int)(myStats.trading * consistencyMultipler);
        myStats.vision = (int)(myStats.vision * consistencyMultipler);
        myStats.warding = (int)(myStats.warding * consistencyMultipler);
    }
    public float DistanceToTurret()
    {
        float distance = Mathf.Infinity;
        foreach(Turret t in friendlyTurrets)
        {
            float currDistance = Vector3.Distance(transform.position, t.transform.position);
            if (currDistance < distance)
            {
                distance = currDistance;
            }
        }
        return distance;
    }

    public void Die()
    {
        transform.position = mySpawn;
        currentHealth = maxHealth;
        Debug.Log("I died LOL");
    }

    public void Back()
    {
        transform.position = mySpawn;
        currentHealth = maxHealth;
    }

    public int GuessCombatPower(List<PlayerAI> team)
    {
        int combatPower = 0;
        foreach (PlayerAI player in team)
        {
            combatPower += (int)((player.championStats.attack + player.championStats.defense + player.championStats.CC) * 1.5f);
            combatPower += player.currentHealth * 3;
            //add simple counter here to add based on champion mastery

            //now, take the arguably most important trait, decision making. This allows players to guess what the skill level of opponents is. 
            //since health and powerlevel are so obvious, this is important. 
            float variance = Mathf.Lerp(15, 2, 100 / myStats.decisionMaking);
            int randomStatVariance = (int)Random.Range(-variance, variance);
            int playerStatPower = player.myStats.allIn + randomStatVariance;
            randomStatVariance = (int)Random.Range(-variance, variance);
            playerStatPower += player.myStats.consistency + randomStatVariance;
            randomStatVariance = (int)Random.Range(-variance, variance);
            playerStatPower += player.myStats.positioning + randomStatVariance;
            if (team.Count > 1)
            {
                randomStatVariance = (int)Random.Range(-variance, variance);
                playerStatPower += player.myStats.teamwork + randomStatVariance;
            }
            playerStatPower = (int)(playerStatPower * 0.05f);
            combatPower += playerStatPower;

        }
        return combatPower;
    }
}
