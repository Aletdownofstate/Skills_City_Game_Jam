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

    private void Start()
    {
        enemyAgent = GetComponent<NavMeshAgent>();
        enemyAnimator = enemy.GetComponent<Animator>();

        if (enemyAgent == null)
        {
            Debug.LogError("NavMeshAgent mancante su " + gameObject.name);
        }

        if (enemyAnimator == null)
        {
            Debug.LogError("Animator mancante su " + enemy.name);
        }

        if (enemyDest == null)
        {
            Debug.LogError("Destinazione del nemico non assegnata!");
        }
    }

    private void Update()
    {
        if (isStalking)
        {
            enemyAnimator.Play("walk");
            enemyAgent.SetDestination(enemyDest.transform.position);
        }
        else
        {
            enemyAnimator.Play("idle");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isStalking && other.CompareTag("Player"))
        {
            GetComponent<BoxCollider>().enabled = false;
            isStalking = true;
        }
    }
}
