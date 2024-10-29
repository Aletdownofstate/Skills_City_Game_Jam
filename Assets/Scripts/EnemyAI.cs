using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent enemyAgent;
    private Animator enemyAnimator;

    public static bool isStalking;

    // Health and Attack system
    public int maxHealth = 100;

    private int currentHealth;
    public float attackRange = 2.0f;
    public int damageAmount = 10;
    public float attackCooldown = 1.5f;
    private float lastAttackTime;

    private void Start()
    {
        currentHealth = maxHealth;
        enemyAgent = GetComponent<NavMeshAgent>();
        enemyAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isStalking)
        {
            // Find the player by tag
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
                if (distanceToPlayer > attackRange)
                {
                    enemyAgent.SetDestination(player.transform.position);
                }
                else
                {
                    enemyAgent.ResetPath(); // Stop moving when within attack range
                }
            }
        }

        enemyAnimator.SetBool("isWalking", isStalking);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isStalking && other.CompareTag("Player"))
        {
            isStalking = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                AttackPlayer(other.gameObject);
                lastAttackTime = Time.time;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isStalking = false;
            enemyAnimator.SetBool("isWalking", false);
            enemyAgent.ResetPath();
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        enemyAgent.enabled = false;
        enemyAnimator.SetTrigger("Die");
        Destroy(gameObject, 2f);
    }

    private void AttackPlayer(GameObject player)
    {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damageAmount);
            Debug.Log("Player took damage: " + damageAmount);
        }
    }
}