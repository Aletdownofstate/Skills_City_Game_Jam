using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int playerHp;
    public int playerMaxHp = 100;

    private void Start()
    {
        playerHp = playerMaxHp;
    }

    public void TakeDamage(int damage)
    {
        playerHp -= damage;
        if (playerHp <= 0)
        {
            playerHp = 0;
            Die();
        }
        else if (playerHp > playerMaxHp)
        {
            playerHp = playerMaxHp;
        }
    }

    private void Die()
    {
        Debug.Log("Player has died!");
    }

}
