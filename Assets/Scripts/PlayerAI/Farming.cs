using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farming : State
{
    bool isFarming;
    bool isBacking;
    bool tryingToBack;
    public LayerMask playerMask;
    Turret currentTurret;
    public State backing;
    public override State stateTick(PlayerAI player)
    {
        if (isFarming)
        {
            player.farmingTick += Time.deltaTime;
            float timeReq = Mathf.Lerp(1.5f, 0.65f, (float)player.myStats.farming / 100);
            if (player.farmingTick >= timeReq)
            {
                player.currentGold += 60;
                player.farmingTick = 0;
            }
            //determine what players do while farming,based on what they can see.
            //we need to determine if they should push, trade, all-in, back, or roam. 
            //unfortunately roaming in League is tied to wave state or, less frequently, lack of presence from the opposing laner. 
            //there are not lane states in this game. 

            //anyways, here comes backing. 
            if ((float)player.currentHealth / (float)player.maxHealth < 0.5f + Mathf.Lerp(0.07f, -0.07f, player.myStats.aggression / 100))
            {
                player.agent.enabled = true;
                player.agent.SetDestination(player.GetClosestTurret().transform.position - (Vector3)player.GetClosestTurret().forwardsVector);
                return backing;
            }
            Collider2D[] nearbyPlayers = Physics2D.OverlapCircleAll(transform.position, 1.45f, playerMask);
            if (nearbyPlayers.Length == 2)
            {
                foreach (Collider2D c in nearbyPlayers)
                {
                    PlayerAI otherPlayer = c.GetComponent<PlayerAI>();
                    if (!player.myTeam.Contains(otherPlayer))
                    {
                        //trade.
                        player.tradingTick += Time.deltaTime;
                        float damageInterval = Mathf.Lerp(2f, 0.8f, (float)player.myStats.trading / 100);
                        damageInterval *= Mathf.Lerp(1, 0.75f, (float)player.championStats.poke / 10);
                        if (player.tradingTick >= damageInterval)
                        {
                            otherPlayer.TakeDamage(1);
                            player.tradingTick = 0;
                        }
                    }
                }
            }


            //determine if a fight is to break out. yikers.
            List<PlayerAI> playersMyTeam = new List<PlayerAI>();
            List<PlayerAI> playersEnemyTeam = new List<PlayerAI>();
            //this goes off every frame. Hope its not gonna lag everything lol


            //currently thinking an alternative is to have vision determine how frequently a player checks to see if they go in? 
            foreach (Collider2D c in nearbyPlayers)
            {
                if (!player.myTeam.Contains(c.GetComponent<PlayerAI>()))
                {
                    playersEnemyTeam.Add(c.GetComponent<PlayerAI>());
                }
                else
                {
                    playersMyTeam.Add(c.GetComponent<PlayerAI>());
                }
            }
            if (playersEnemyTeam.Count == 1)
            {

                float allyCombatScore = player.GuessCombatPower(playersMyTeam);
                float enemyCombatScore = player.GuessCombatPower(playersEnemyTeam);
                float agressionModifier = Mathf.Lerp(25, 10, player.myStats.aggression / 100);
                if (allyCombatScore - enemyCombatScore >= agressionModifier && !player.gameManager.resolvingCombat)
                {
                    player.gameManager.ResolveCombat1v1(player, playersEnemyTeam[0]);
                }
            }
        }
        else
        {
            //cases depending on lane designation
            if (player.blueTeam)
            {
                player.agent.enabled = true;
                //move the player to their turret, plus extra based on their aggression. 
                switch (player.myRole)
                {
                    case PlayerAI.role.TOP:
                        //go down the priority queue. 
                        if (player.topBlueTurretT1.isAlive)
                        {
                            player.agent.SetDestination(player.topBlueTurretT1.transform.position + 
                                (Vector3)(player.topBlueTurretT1.forwardsVector * (0.85f +   player.myStats.aggression * 0.0045f)));
                            currentTurret = player.topBlueTurretT1;
                        }
                        else if (player.topBlueTurretT2.isAlive)
                        {
                            player.agent.SetDestination(player.topBlueTurretT2.transform.position + 
                                (Vector3)(player.topBlueTurretT2.forwardsVector * (0.85f +   player.myStats.aggression * 0.0045f)));
                            currentTurret = player.topBlueTurretT2;

                        }
                        else if (player.baseTopBlueTurret.isAlive)
                        {
                            player.agent.SetDestination(player.topBlueTurretT2.transform.position + 
                                (Vector3)(player.topBlueTurretT2.forwardsVector * (0.85f +   player.myStats.aggression * 0.0045f)));
                            currentTurret = player.baseTopBlueTurret;

                        }
                        else if (player.baseBotBlueTurret.isAlive)
                        {
                            player.agent.SetDestination(player.baseBotBlueTurret.transform.position + 
                                (Vector3)(player.baseBotBlueTurret.forwardsVector * (0.85f +   player.myStats.aggression * 0.0045f)));
                            currentTurret = player.baseBotBlueTurret;

                        }
                        else
                        {
                            //TODO: PANIC???
                        }
                        break;
                    case PlayerAI.role.MIDDLE:
                        //go down the priority queue. 
                        if (player.midBlueTurretT1.isAlive)
                        {
                            player.agent.SetDestination(player.midBlueTurretT1.transform.position + 
                                (Vector3)(player.midBlueTurretT1.forwardsVector * (0.85f +   player.myStats.aggression * 0.0045f)));
                            currentTurret = player.midBlueTurretT1;

                        }
                        else if (player.midBlueTurretT2.isAlive)
                        {
                            player.agent.SetDestination(player.topBlueTurretT2.transform.position + 
                                (Vector3)(player.topBlueTurretT2.forwardsVector * (0.85f +   player.myStats.aggression * 0.0045f)));
                            currentTurret = player.midBlueTurretT2;
                        }
                        else if (player.baseBotBlueTurret.isAlive)
                        {
                            player.agent.SetDestination(player.baseBotBlueTurret.transform.position + 
                                (Vector3)(player.baseBotBlueTurret.forwardsVector * (0.85f +   player.myStats.aggression * 0.0045f)));
                            currentTurret = player.baseBotBlueTurret;

                        }
                        else if (player.baseTopBlueTurret.isAlive)
                        {
                            player.agent.SetDestination(player.baseTopBlueTurret.transform.position + 
                                (Vector3)(player.baseTopBlueTurret.forwardsVector * (0.85f +   player.myStats.aggression * 0.0045f)));
                            currentTurret = player.baseTopBlueTurret;
                        }
                        else
                        {
                            //TODO: PANIC???
                        }
                        break;
                    case PlayerAI.role.BOTTOM:
                        //go down the priority queue. 
                        if (player.botBlueTurretT1.isAlive)
                        {
                            player.agent.SetDestination(player.botBlueTurretT1.transform.position + 
                                (Vector3)(player.botBlueTurretT1.forwardsVector * (0.85f +   player.myStats.aggression * 0.0045f)));
                            currentTurret = player.botBlueTurretT1;
                        }
                        else if (player.botBlueTurretT2.isAlive)
                        {
                            player.agent.SetDestination(player.botBlueTurretT2.transform.position + 
                                (Vector3)(player.botBlueTurretT2.forwardsVector * (0.85f +   player.myStats.aggression * 0.0045f)));
                            currentTurret = player.botBlueTurretT2;

                        }
                        else if (player.baseBotBlueTurret.isAlive)
                        {
                            player.agent.SetDestination(player.baseBotBlueTurret.transform.position + 
                                (Vector3)(player.baseBotBlueTurret.forwardsVector * (0.85f +   player.myStats.aggression * 0.0045f)));
                            currentTurret = player.baseBotBlueTurret;

                        }
                        else if (player.baseTopBlueTurret.isAlive)
                        {
                            player.agent.SetDestination(player.baseTopBlueTurret.transform.position + 
                                (Vector3)(player.baseTopBlueTurret.forwardsVector * (0.85f +   player.myStats.aggression * 0.0045f)));
                            currentTurret = player.baseTopBlueTurret;

                        }
                        else
                        {
                            //TODO: PANIC???
                        }
                        break;

                }
            }
            else
            {
                //whole thing again. yeah there's 100% a better way to do this. 
                player.agent.enabled = true;
                //move the player to their turret, plus extra based on their aggression. 
                switch (player.myRole)
                {
                    case PlayerAI.role.TOP:
                        //go down the priority queue. 
                        if (player.topRedTurretT1.isAlive)
                        {
                            player.agent.SetDestination(player.topRedTurretT1.transform.position + (Vector3)(player.topRedTurretT1.forwardsVector * (0.85f +   player.myStats.aggression * 0.0045f)));
                            currentTurret = player.topRedTurretT1;

                        }
                        else if (player.topRedTurretT2.isAlive)
                        {
                            player.agent.SetDestination(player.topRedTurretT2.transform.position + (Vector3)(player.topRedTurretT2.forwardsVector * (0.85f +   player.myStats.aggression * 0.0045f)));
                            currentTurret = player.topRedTurretT2;

                        }
                        else if (player.baseTopRedTurret.isAlive)
                        {
                            player.agent.SetDestination(player.topRedTurretT2.transform.position + (Vector3)(player.topRedTurretT2.forwardsVector * (0.85f +   player.myStats.aggression * 0.0045f)));
                            currentTurret = player.baseTopRedTurret;

                        }
                        else if (player.baseBotRedTurret.isAlive)
                        {
                            player.agent.SetDestination(player.baseBotRedTurret.transform.position + 
                                (Vector3)(player.baseBotRedTurret.forwardsVector * (0.85f +   player.myStats.aggression * 0.0045f)));
                            currentTurret = player.baseTopRedTurret;

                        }
                        else
                        {
                            //TODO: PANIC???
                        }
                        break;
                    case PlayerAI.role.MIDDLE:
                        //go down the priority queue. 
                        if (player.midRedTurretT1.isAlive)
                        {
                            player.agent.SetDestination(player.midRedTurretT1.transform.position + 
                                (Vector3)(player.midRedTurretT1.forwardsVector * (0.85f +   player.myStats.aggression * 0.0045f)));
                            currentTurret = player.midRedTurretT1;


                        }
                        else if (player.midRedTurretT2.isAlive)
                        {
                            player.agent.SetDestination(player.topRedTurretT2.transform.position + 
                                (Vector3)(player.topRedTurretT2.forwardsVector * (0.85f +   player.myStats.aggression * 0.0045f)));
                            currentTurret = player.midRedTurretT2;

                        }
                        else if (player.baseBotRedTurret.isAlive)
                        {
                            player.agent.SetDestination(player.baseBotRedTurret.transform.position + 
                                (Vector3)(player.baseBotRedTurret.forwardsVector * (0.85f +   player.myStats.aggression * 0.0045f)));
                            currentTurret = player.baseBotRedTurret;

                        }
                        else if (player.baseTopRedTurret.isAlive)
                        {
                            player.agent.SetDestination(player.baseTopRedTurret.transform.position + 
                                (Vector3)(player.baseTopRedTurret.forwardsVector * (0.85f +   player.myStats.aggression * 0.0045f)));
                            currentTurret = player.baseTopRedTurret;
                        }
                        else
                        {
                            //TODO: PANIC???
                        }
                        break;
                    case PlayerAI.role.BOTTOM:
                        //go down the priority queue. 
                        if (player.botRedTurretT1.isAlive)
                        {
                            player.agent.SetDestination(player.botRedTurretT1.transform.position + 
                                (Vector3)(player.botRedTurretT1.forwardsVector * (0.85f +   player.myStats.aggression * 0.0045f)));
                            currentTurret = player.botRedTurretT1;

                        }
                        else if (player.botRedTurretT2.isAlive)
                        {
                            player.agent.SetDestination(player.botRedTurretT2.transform.position + 
                                (Vector3)(player.botRedTurretT2.forwardsVector * (0.85f +   player.myStats.aggression * 0.0045f)));
                            currentTurret = player.botRedTurretT2;

                        }
                        else if (player.baseBotRedTurret.isAlive)
                        {
                            player.agent.SetDestination(player.baseBotRedTurret.transform.position + 
                                (Vector3)(player.baseBotRedTurret.forwardsVector * (0.85f +   player.myStats.aggression * 0.0045f)));
                            currentTurret = player.baseBotRedTurret;

                        }
                        else if (player.baseTopRedTurret.isAlive)
                        {
                            player.agent.SetDestination(player.baseTopRedTurret.transform.position + 
                                (Vector3)(player.baseTopRedTurret.forwardsVector * (0.85f +   player.myStats.aggression * 0.0045f)));
                            currentTurret = player.baseTopRedTurret;

                        }
                        else
                        {
                            //TODO: PANIC???
                        }
                        break;


                }
            }
        }
        if (Vector2.Distance(player.transform.position, player.agent.destination) <= player.agent.stoppingDistance)
        {
            isFarming = true;
            player.agent.enabled = false;
        }
        else
        {
            isFarming = false;
        }
        
        //player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, 1);
        player.transform.eulerAngles = Vector3.zero;
        return this;
    }
}
