using UnityEngine;
using UnityEngine.AI;

public class MoveToAttack : MonoBehaviour
{
	NavMeshAgent _agent;
	EnemyAttack _attack;
	Bobbing _bobbing;
	Enemy _enemy;

	void Awake()
	{
		_agent = GetComponent<NavMeshAgent>();
		_attack = GetComponent<EnemyAttack>();
		_bobbing = GetComponentInChildren<Bobbing>();
		_enemy = GetComponent<Enemy>();
	}

	void Start()
	{
		_ = _agent.SetDestination(Tower.Instance.transform.position);

		_enemy.OnDeath += HandleDeath;
	}

	void Update()
	{
		if (_attack.Target != null)
		{
			_agent.isStopped = true;
			_bobbing.Stop();
		}
	}

	void HandleDeath(Target target) => Stop();

	public void Stop()
	{
		if (_agent && _agent.enabled)
		{
			_agent.isStopped = true;
		}
	}
}
