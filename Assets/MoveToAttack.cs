using UnityEngine;
using UnityEngine.AI;

public class MoveToAttack : MonoBehaviour
{
    Animator animator;
    NavMeshAgent agent;
    EnemyAttack attack;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        attack = GetComponent<EnemyAttack>();
    }

    void Start()
    {
        agent.SetDestination(Tower.instance.transform.position);
        animator.SetBool("IsWalking", true);
    }

    void Update()
    {
        if (attack.target != null)
        {
            animator.SetBool("IsWalking", false);
            agent.isStopped = true;
        }
    }
}
