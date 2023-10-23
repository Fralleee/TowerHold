using UnityEngine;
using UnityEngine.AI;

public class MoveToAttack : MonoBehaviour
{
    NavMeshAgent agent;
    EnemyAttack attack;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        attack = GetComponent<EnemyAttack>();
    }

    void Start()
    {
        agent.SetDestination(Tower.instance.transform.position);
    }

    void Update()
    {
        if (attack.target != null)
        {
            agent.isStopped = true;
        }
    }
}
