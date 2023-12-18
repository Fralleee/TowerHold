using UnityEngine;
using UnityEngine.AI;

public class MoveToAttack : MonoBehaviour
{
	Animator _animator;
	NavMeshAgent _agent;
	EnemyAttack _attack;
	Bobbing _bobbing;
	Enemy _enemy;

	void Awake()
	{
		_animator = GetComponentInChildren<Animator>();
		_agent = GetComponent<NavMeshAgent>();
		_attack = GetComponent<EnemyAttack>();
		_bobbing = GetComponentInChildren<Bobbing>();
		_enemy = GetComponent<Enemy>();
	}

	void Start()
	{
		_ = _agent.SetDestination(Tower.Instance.transform.position);
		_animator.SetBool("IsWalking", true);

		_enemy.OnDeath += HandleDeath;
	}

	void Update()
	{
		if (_attack.Target != null)
		{
			_animator.SetBool("IsWalking", false);
			_agent.isStopped = true;
			_bobbing.Stop();
		}
	}

	void HandleDeath(Target target) => Stop();

	public void Stop() => _agent.isStopped = true;
}
