using UnityEngine;
using UnityEngine.AI;

public class MoveToAttack : MonoBehaviour
{
    NavMeshAgent agent;
    Shooter shooter;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        shooter = GetComponent<Shooter>();
    }

    void Start()
    {
        agent.SetDestination(Tower.asTarget.transform.position);
    }

    void Update()
    {
        if (shooter.target != null)
        {
            agent.isStopped = true;
        }
    }
}
