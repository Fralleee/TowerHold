using System;
using UnityEngine;
using UnityEngine.AI;

public class MoveToAttack : MonoBehaviour
{
    Animator animator;
    NavMeshAgent agent;
    EnemyAttack attack;

    Enemy enemy;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        attack = GetComponent<EnemyAttack>();
        enemy = GetComponent<Enemy>();
    }

    void Start()
    {
        agent.SetDestination(Tower.instance.transform.position);
        animator.SetBool("IsWalking", true);

        enemy.OnDeath += HandleDeath;
    }

    void Update()
    {
        if (attack.target != null)
        {
            animator.SetBool("IsWalking", false);
            agent.isStopped = true;
        }
    }

    void HandleDeath(Target target)
    {
        Stop();
    }

    public void Stop()
    {
        agent.isStopped = true;
    }
}
