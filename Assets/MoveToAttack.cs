using UnityEngine;
using UnityEngine.AI;

public class MoveToAttack : MonoBehaviour
{
    Health target;
    NavMeshAgent agent;
    ITargeter targeter;
    Shooter shooter;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        targeter = GetComponent<ITargeter>();
        shooter = GetComponent<Shooter>();
        target = targeter.GetTarget();
    }

    void Update()
    {
        if (target != null)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance < shooter.attackRange)
            {
                agent.isStopped = true;
            }
            else
            {
                agent.SetDestination(target.transform.position);
            }
        }
    }
}
