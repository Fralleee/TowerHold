using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveToAttack : MonoBehaviour
{
    GameObject target;
    NavMeshAgent agent;

    public float attackRange = 2f;
    public float attackRate = 1f;
    public float attackDamage = 10f;
    float lastAttackTime = 0f;


    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();    
    }

    void Update()
    {
        // Check distance
        if (target != null)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance < attackRange)
            {
                agent.isStopped = true;
                // Check time
                if (Time.time - lastAttackTime > attackRate)
                {
                    Attack();
                    lastAttackTime = Time.time;
                }
            }
            else
            {
                agent.SetDestination(target.transform.position);
            }
        }
    }

    void Attack()
    {
        Debug.Log("Attack!");
    }

    public void SetTarget(GameObject newTarget) 
    {
        target = newTarget;

        agent.SetDestination(target.transform.position);
    }
}
