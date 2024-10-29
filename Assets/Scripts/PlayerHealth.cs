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

    private void Update()
    {
        if (playerHp <= 0)
        {
            playerHp = 0;
        }
        if (playerHp >= playerMaxHp)
        {
            playerHp = playerMaxHp;
        }
    }

    public void TakeDamage(int damage)
    {
        playerHp -= damage;
    }
}