using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int playerHp;
    public int playerMaxHp = 100;

    public bool isDead;

    [SerializeField] private Slider healthBar;

    private void Start()
    {
        playerHp = playerMaxHp;
    }

    private void Update()
    {
        healthBar.value = playerHp;
    }

    public void TakeDamage(int damage)
    {
        playerHp -= damage;

        if (playerHp <= 0)
        {
            playerHp = 0;
            isDead = true;
        }        
    }
}