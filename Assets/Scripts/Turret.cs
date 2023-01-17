using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public bool isAlive = true;
    public bool onBlue;
    public Vector2 forwardsVector;
    public int currentHealth = 100;

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
    }
    public void Die()
    {
        isAlive = false;
        gameObject.SetActive(false);
    }
    public void Respawn()
    {
        isAlive = true;
        gameObject.SetActive(true);
    }
}
