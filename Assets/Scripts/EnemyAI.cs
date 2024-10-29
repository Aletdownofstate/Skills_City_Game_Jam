using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent enemyAgent;
    private Animator enemyAnimator;

    public static bool isStalking;
    private bool isAlive = true;

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
        enemyAnimator = GetComponentInChildren<Animator>();

        isStalking = true;
    }

    private void Update()
    {
        if (currentHealth <= 0)
        {
            isAlive = false;
            enemyAnimator.SetBool("isWalking", false);
            enemyAnimator.SetTrigger("isDead");
            Die();
        }

        if (enemyAgent.velocity.x != 0 || enemyAgent.velocity.y != 0)
        {
            enemyAnimator.SetBool("isWalking", true);
        }
        else
        {
            enemyAnimator.SetBool("isWalking", false);
        }

        if (isStalking && isAlive)
        {
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
                    enemyAgent.ResetPath();
                    enemyAnimator.SetBool("isWalking", false);
                }
            }
        }        
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
    }

    private void Die()
    {        
        enemyAgent.enabled = false;
        Destroy(gameObject, 3f);
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