using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public PlayerStats[] players;
    public ChampionStats[] champions;
    public UIManager uiManager;
    public GameCanvas gameCanvas;
    public GameObject playerDisplay;
    public List<PlayerStats> blue;
    public List<PlayerStats> red;
    public List<PlayerDisplay> blueDisplays;
    public List<PlayerDisplay> redDisplays;
    public List<PlayerAI> bluePlayers = new List<PlayerAI>();
    public List<PlayerAI> redPlayers = new List<PlayerAI>();
    [Header("Players")]
    public PlayerAI blueTop;
    public PlayerAI blueJungle;
    public PlayerAI blueMid;
    public PlayerAI blueBot;
    public PlayerAI blueSupport;
    public PlayerAI redTop;
    public PlayerAI redJungle;
    public PlayerAI redMid;
    public PlayerAI redBot;
    public PlayerAI redSupport;
    public bool resolvingCombat;
    Vector3 cameraPosition = new Vector3(0, 0, -10);
    

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        ReadPlayerData.ReadAndLoadPlayers(this);
        foreach (PlayerStats p in players)
        {
            GameObject go = Instantiate(playerDisplay, uiManager.playerDisplayLocation);
            go.GetComponent<PlayerDisplay>().SetPlayer(p, uiManager);
        }
        foreach (ChampionStats c in champions)
        {
            GameObject go = Instantiate(uiManager.championDisplay, uiManager.championPicker.transform);
            go.GetComponent<ChampDisplay>().OnCreation(c, uiManager.championStatDisplay);
           
        }
        //TESTING: REMEMBER TO REMOVE!
        blueTop.myStats = players[0];
    }

    public void StartGame()
    {
        if (blue.Count == 4 && red.Count == 4)
        {
            uiManager.gameObject.SetActive(false);

            for (int i = 0; i < 4; i++)
            {
                bluePlayers[i].myStats = blue[i];
                redPlayers[i].myStats = red[i];
            }
            ActivatePlayers();
        }
    }

    public void QuickStart()
    {
        uiManager.gameObject.SetActive(false);
        blueTop.myStats = players[0];
        redTop.myStats = players[1];
        blueMid.myStats = players[2];
        redMid.myStats = players[3];
        blueBot.myStats = players[4];
        redBot.myStats = players[5];
        blueSupport.myStats = players[6];
        redSupport.myStats = players[7];
        ActivatePlayers();
    }

    void ActivatePlayers()
    {
        for (int i = 0; i < 4; i++)
        {
            bluePlayers[i].championStats = champions[bluePlayers[i].myStats.championSelected];
            redPlayers[i].championStats = champions[redPlayers[i].myStats.championSelected];
        }
        blueTop.enabled = true;
        blueMid.enabled = true;
        blueBot.enabled = true;
        blueSupport.enabled = true;
        redTop.enabled = true;
        redMid.enabled = true;
        redBot.enabled = true;
        redSupport.enabled = true;
        foreach(PlayerAI p in bluePlayers)
        {
            p.Activate(this);
        }
        foreach (PlayerAI p in redPlayers)
        {
            p.Activate(this);
        }
    }
    public void SaveGame()
    {
        ReadPlayerData.SavePlayers(this);
    }

    public void AddPlayerBlue(PlayerStats p)
    {
        if (red.Contains(p))
        {
            redDisplays[red.IndexOf(p)].RemovePlayer();
            red.Remove(p);
        }
        if(!blue.Contains(p) && blue.Count < 4)
        {
            blue.Add(p);
            blueDisplays[blue.Count - 1].SetPlayer(p, uiManager);
        }
    }
    public void ResolveCombat1v1(PlayerAI initiator, PlayerAI otherPlayer)
    {
        StartCoroutine(ResolveAndDisplayCombat1v1(initiator, otherPlayer));
        resolvingCombat = true;
    }
    public IEnumerator ResolveAndDisplayCombat1v1(PlayerAI initiator, PlayerAI otherPlayer)
    {
        //first, do a fancy thing with the camera. 
        Vector3 midpoint = initiator.transform.position + otherPlayer.transform.position;
        midpoint = new Vector3(midpoint.x / 2, midpoint.y / 2, -10);
        Camera.main.transform.position = midpoint;
        Camera.main.orthographicSize = 1;
        //then get the combat scores and see who wins
        float initiatorScore = GetCombatScore(initiator, true);
        float defenderScore = GetCombatScore(otherPlayer, true);
        if (initiator.blueTeam)
        {
            gameCanvas.Display1v1(initiator.myStats.name, otherPlayer.myStats.name, initiatorScore, defenderScore, initiator.transform.position);
        }
        else 
        {
            gameCanvas.Display1v1(otherPlayer.myStats.name, initiator.myStats.name, defenderScore, initiatorScore, initiator.transform.position);
        }
        Time.timeScale = 0;
        yield return new WaitUntil(() => Time.timeScale != 0);

        if (initiatorScore > defenderScore)
        {
            //the defender then tries to disengage. 
            //using how far away they are from their nearest turret, mixed with their mobility/combat speed and positioning.
            //then it checks the initiator's CC score, mixed with their mobility and combat speed. 
            //if the defender loses, they die. 
            float escapeScore = -otherPlayer.DistanceToTurret() * 23;
            escapeScore += otherPlayer.championStats.combatSpeed * 3;
            escapeScore += otherPlayer.championStats.mobility * 4;
            escapeScore += otherPlayer.myStats.positioning * 0.8f;

            float chaseScore = initiator.championStats.combatSpeed * 3;
            chaseScore += initiator.championStats.mobility * 4.5f;
            chaseScore += initiator.championStats.CC * 5;

            if (initiator.blueTeam)
            {
                gameCanvas.DisplayEscape(otherPlayer.myStats.name, otherPlayer.myStats.name, chaseScore, escapeScore, true);
            }
            else
            {
                gameCanvas.DisplayEscape(otherPlayer.myStats.name, otherPlayer.myStats.name, chaseScore, escapeScore, false);
            }
            Time.timeScale = 0;
            yield return new WaitUntil(() => Time.timeScale != 0);
            //players will take damage either way, based on the difference in combat score. 
            float scoreDiff = initiatorScore - defenderScore;
            if (scoreDiff < 100)
            {
                float damage = Mathf.Lerp(99, 0, scoreDiff / 100);
                float damageToTake = (float)initiator.currentHealth * (damage / 100);
                initiator.TakeDamage((int)damageToTake);
            }
            if (chaseScore >= escapeScore)
            {
                otherPlayer.Die();
                gameCanvas.DisplayResult("escape failed.");
                Time.timeScale = 0;
                yield return new WaitUntil(() => Time.timeScale != 0);

            }
            else
            {
                float damage = Mathf.Lerp(99, 0, scoreDiff / 100);
                float damageToTake = (float)otherPlayer.currentHealth * (damage / 100);
                otherPlayer.TakeDamage((int)damageToTake);
                otherPlayer.Back();
                gameCanvas.DisplayResult("escape succeeded.");
                Time.timeScale = 0;
                yield return new WaitUntil(() => Time.timeScale != 0);
            }
        }
        else if(initiatorScore < defenderScore)
        {
            //the initiator then tries to disengage. 
            //using how far away they are from their nearest turret, mixed with their mobility/combat speed and positioning.
            //this checks the initiator's CC score, mixed with their mobility and combat speed. 
            //if the defender loses, they die. 
            float escapeScore = -initiator.DistanceToTurret() * 23;
            escapeScore += initiator.championStats.combatSpeed * 3;
            escapeScore += initiator.championStats.mobility * 3.5f;
            escapeScore += initiator.myStats.positioning * 0.8f;

            float chaseScore = otherPlayer.championStats.combatSpeed * 3;
            chaseScore += otherPlayer.championStats.mobility * 4.5f;
            chaseScore += otherPlayer.championStats.CC * 8;

            if (otherPlayer.blueTeam)
            {
                gameCanvas.DisplayEscape(initiator.myStats.name, otherPlayer.myStats.name, chaseScore, escapeScore, true);
            }
            else
            {
                gameCanvas.DisplayEscape(initiator.myStats.name, otherPlayer.myStats.name, chaseScore, escapeScore, false);
            }
            Time.timeScale = 0;
            yield return new WaitUntil(() => Time.timeScale != 0);
            float scoreDiff = defenderScore - initiatorScore;
            if (scoreDiff < 100)
            {
                float damage = Mathf.Lerp(99, 0, scoreDiff / 100);
                float damageToTake = (float)otherPlayer.currentHealth * (damage / 100);
                otherPlayer.TakeDamage((int)damageToTake);
            }
            if (chaseScore >= escapeScore)
            {
                initiator.Die();
                gameCanvas.DisplayResult("escape failed.");
                
                Time.timeScale = 0;
                yield return new WaitUntil(() => Time.timeScale != 0);
            }
            else
            {
                float damage = Mathf.Lerp(99, 0, scoreDiff / 100);
                float damageToTake = (float)initiator.currentHealth * (damage / 100);

                initiator.TakeDamage((int)damageToTake);
                initiator.Back();
                gameCanvas.DisplayResult("escape succeeded.");
                Time.timeScale = 0;
                yield return new WaitUntil(() => Time.timeScale != 0);
            }

        }
        Camera.main.transform.position = cameraPosition;
        Camera.main.orthographicSize = 5;
        resolvingCombat = false;
    }

    public float GetCombatScore(PlayerAI player, bool isDuel)
    {

        //NOTE: eventually, each champion will need to have matchup-reliant stats, that modify the combat scores. 
        float combatScore = 0;
        combatScore += (player.championStats.attack + player.championStats.defense + player.championStats.CC);
        combatScore += (((float)player.currentHealth / (float)player.maxHealth) * 80f);
        if(isDuel)
        {
            combatScore += (player.championStats.dueling * 10f);
        }
        else
        {
            combatScore += (player.myStats.positioning + player.myStats.teamwork) * 0.9f;
        }
        combatScore += (player.myStats.playmaking * 0.9f);
        combatScore += player.myStats.allIn * 0.65f;
        combatScore *= player.performanceMultiplier;
        combatScore *= Mathf.Pow(player.totalGoldModifier, player.championStats.scaling);
        //add some randomness to get some spice!
        float randomSpice = Random.Range(-11.0f, 11.0f);
        combatScore += randomSpice;
        return combatScore;
    }
    public void AddPlayerRed(PlayerStats p)
    {
        if (blue.Contains(p))
        {
            blueDisplays[blue.IndexOf(p)].RemovePlayer();
            blue.Remove(p);
        }
        if (!red.Contains(p) && red.Count < 4)
        {
            red.Add(p);
            redDisplays[red.Count - 1].SetPlayer(p, uiManager);

        }
    }
}
