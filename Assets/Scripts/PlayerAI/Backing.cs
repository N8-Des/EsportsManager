using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Backing : State
{
    bool isBacking;
    bool hasBacked;
    public State farming;
    public override State stateTick(PlayerAI player)
    {
        if (!isBacking)
        {
            if (Vector2.Distance(player.transform.position, player.agent.destination) <= player.agent.stoppingDistance)
            {
                player.agent.enabled = false;
                StartCoroutine(BackTimer(player));
                isBacking = true;
            }
        }
        if (hasBacked)
        {
            return farming;
        }
        player.transform.eulerAngles = Vector3.zero;

        return this;
    }
    IEnumerator BackTimer(PlayerAI player)
    {
        yield return new WaitForSeconds(1);
        player.Back();
        hasBacked = true;
    }
}
