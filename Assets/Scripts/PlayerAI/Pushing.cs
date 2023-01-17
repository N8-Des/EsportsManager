using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pushing : State
{
    bool pushing = false;
    public State farming;
    public override State stateTick(PlayerAI player)
    {
        if (pushing)
        {
            //check if there are enemies nearby. 
            //So I could make this in the update function of the PlayerAI and use this to hold all the different places the AI checks for the positions of players. 
            //Here's the kicker - depending on the situation, the distance of the players matter, so the 
            Collider2D[] nearbyPlayers = Physics2D.OverlapCircleAll(transform.position, 1.45f, player.playerMask);
            if (nearbyPlayers.Length > 1)
            {
                bool nearbyPlayer = false;
                foreach (Collider2D c in nearbyPlayers)
                {
                    PlayerAI otherPlayer = c.GetComponent<PlayerAI>();
                    if (!player.myTeam.Contains(otherPlayer) && player.isSeen)
                    {
                        nearbyPlayer = true;
                        break;
                    }
                }
            }
        }
        else
        {
            player.agent.enabled = true;
            player.agent.SetDestination(player.GetClosestEnemyTurret().transform.position + (Vector3)player.GetClosestEnemyTurret().forwardsVector);
        }
        if (Vector2.Distance(player.transform.position, player.agent.destination) <= player.agent.stoppingDistance)
        {
            pushing = true;
            player.agent.enabled = false;
        }
        player.transform.eulerAngles = Vector3.zero;

        return this;
    }

}
