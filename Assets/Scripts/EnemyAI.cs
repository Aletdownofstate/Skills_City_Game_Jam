using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public GameObject enemyDest;
    private NavMeshAgent enemyAgent;
    private Animator enemyAnimator;
    public GameObject enemy;

    public static bool isStalking;

    // Health system
    public int maxHealth = 100;
    private int currentHealth;

    private void Start()
    {
        // Initialize health
        currentHealth = maxHealth;

        // Cache components
        enemyAgent = GetComponent<NavMeshAgent>();
        enemyAnimator = enemy.GetComponent<Animator>();

        // Check for required components
        if (enemyAgent == null)
        {
            Debug.LogError("NavMeshAgent component missing on " + gameObject.name);
        }

        if (enemyAnimator == null)
        {
            Debug.LogError("Animator component missing on " + enemy.name);
        }

        if (enemyDest == null)
        {
            Debug.LogError("Enemy destination not assigned!");
        }
    }

    private void Update()
    {
        // Set the animator parameter to control the animation
        enemyAnimator.SetBool("isWalking", isStalking);

        // Set destination only if stalking
        if (isStalking)
        {
            enemyAgent.SetDestination(enemyDest.transform.position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Activate stalking only if the collider belongs to the player
        if (!isStalking && other.CompareTag("Player"))
        {
            GetComponent<BoxCollider>().enabled = false;
            isStalking = true;
        }
    }

    // Health system method
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
        // Disable the enemy's movement and play the death animation
        enemyAgent.enabled = false;
        enemyAnimator.SetTrigger("die"); 

        Destroy(gameObject, 0.5f);
    }
}
