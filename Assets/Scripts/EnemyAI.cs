using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent enemyAgent;
    private Animator enemyAnimator;
    private PlayerHealth playerHealth;

    private bool isAlive = true;
    public static bool isStalking;

    public int maxHealth = 100;
    private int currentHealth;

    private bool canAttack;
    public float attackRange = 2.0f;
    public int damageAmount = 10;

    private void Start()
    {
        currentHealth = maxHealth;
        enemyAgent = GetComponent<NavMeshAgent>();
        enemyAnimator = GetComponentInChildren<Animator>();
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        canAttack = true;
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
        if (other.gameObject.name.Equals("Player"))
        {
            if (!isStalking)
            {
                isStalking = true;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.name.Equals("Player"))
        {
            if (canAttack && isAlive)
            {
                canAttack = false;
                AttackPlayer();
            }
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

    private void AttackPlayer()
    {
        if (playerHealth != null)
        {            
            enemyAnimator.SetTrigger("isAttacking");
            StartCoroutine(AttackDelay());                              
        }
    }

    private IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(1.2f);
        playerHealth.TakeDamage(damageAmount);
        Debug.Log("Player took damage: " + damageAmount);
        yield return new WaitForSeconds(1.4f);
        canAttack = true;
    }
}