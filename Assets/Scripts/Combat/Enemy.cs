using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Target
{
	public static Action<Enemy> OnAnyDeath = delegate { };
	public int Value = 10;
	[SerializeField] GameObject _deathEffect;
	public static List<Enemy> AllEnemies = new List<Enemy>();

	[ReadOnly] public int Attackers = 0;
	public bool HasAttackers => Attackers > 0;

	NavMeshAgent _agent;
	EnemyAttack _attack;
	Bobbing _bobbing;

	protected override void Awake()
	{
		base.Awake();

		_agent = GetComponent<NavMeshAgent>();
		_attack = GetComponent<EnemyAttack>();
		_bobbing = GetComponentInChildren<Bobbing>();

		AllEnemies.Add(this);

		Value += GameController.Instance.CurrentLevel * 2;

		OnDeath += HandleDeath;
	}

	protected override void Start()
	{
		base.Start();

		_ = _agent.SetDestination(Tower.Instance.transform.position);
	}

	void FixedUpdate()
	{
		if (_attack.Target != null)
		{
			_agent.isStopped = true;
			_bobbing.Stop();
		}
	}

	void HandleDeath(Target target)
	{
		StopMovement();

		_ = AllEnemies.Remove(this);
		ResourceManager.Instance.AddResource(Value);

		foreach (var component in GetComponents<MonoBehaviour>())
		{
			component.enabled = false;
		}

		var instance = Instantiate(_deathEffect, Center.position, Quaternion.LookRotation(-transform.forward));
		Destroy(instance, 10f);

		Destroy(gameObject);
	}

	public void StopMovement()
	{
		if (_agent && _agent.enabled)
		{
			_agent.isStopped = true;
		}
		_bobbing.Stop();
	}

	public static void ResetGameState()
	{
		AllEnemies.Clear();
		OnAnyDeath = delegate
		{ };
	}

	public static void GameOver()
	{
		foreach (var enemy in AllEnemies)
		{
			enemy.StopMovement();
			foreach (var component in enemy.GetComponents<MonoBehaviour>())
			{
				component.enabled = false;
			}
		}
	}

	public override void Die()
	{
		base.Die();

		OnAnyDeath(this);
	}
}
