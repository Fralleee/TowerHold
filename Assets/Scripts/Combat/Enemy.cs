using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

[SelectionBase]
public class Enemy : Target
{
	public static Action<Enemy> OnAnyDeath = delegate { };
	public static List<Enemy> AllEnemies = new List<Enemy>();

	[HideInInspector] public int Attackers = 0;
	[HideInInspector] public bool HasAttackers => Attackers > 0;

	[Header("State")]
	[SerializeField, ReadOnly] EnemyState _currentState;

	[Header("Enemy Settings")]
	public int Value = 10;
	[SerializeField] GameObject _deathEffect;

	[Header("Attack Settings")]
	[SerializeField] AttackType _attackType;
	[SerializeField] float _baseDamage = 10f;
	[SerializeField] float _attacksPerSecond = 1f;
	[HideIf("_attackType", AttackType.MELEE), SerializeField] float _attackRange = 4f;

	[Space(10)]
	[ShowIf("_attackType", AttackType.RANGED_PROJECTILE), SerializeField] Projectile _projectilePrefab;
	[ShowIf("_attackType", AttackType.RANGED_PROJECTILE), SerializeField, ChildGameObjectsOnly] Transform _attackOrigin;
	[ShowIf("_attackType", AttackType.RANGED_PROJECTILE), SerializeField, InlineProperty(LabelWidth = 140)] ProjectileSettings _projectileSettings;
	[HideIf("_attackType", AttackType.RANGED_PROJECTILE), SerializeField] AudioClip _attackSound;

	Target _target;
	Animator _animator;
	NavMeshAgent _agent;
	Bobbing _bobbing;

	float _lastAttackTime = 0f;
	float _nextDistanceCheck = 0f;
	float TimeBetweenAttacks => 1f / _attacksPerSecond;
	readonly float _minTimeBetweenDistanceChecks = 0.5f;

	protected override void Awake()
	{
		base.Awake();

		_agent = GetComponent<NavMeshAgent>();
		_bobbing = GetComponentInChildren<Bobbing>();
		_animator = GetComponentInChildren<Animator>();

		_currentState = EnemyState.Idle;

		if (_attackOrigin == null)
		{
			_attackOrigin = transform;
		}

		Value += GameController.Instance.CurrentLevel * 2;
		OnDeath += HandleDeath;

		AllEnemies.Add(this);
	}

	protected override void Start()
	{
		base.Start();
	}

	void FixedUpdate()
	{
		switch (_currentState)
		{
			case EnemyState.Idle:
				SearchForTarget();
				break;
			case EnemyState.MovingToTarget:
				MoveToTarget();
				break;
			case EnemyState.Attacking:
				AttackTarget();
				break;
			case EnemyState.Victory:
				Celebrate();
				break;
			default:
				break;
		}
	}

	void ChangeState(EnemyState newState)
	{
		_currentState = newState;
		switch (newState)
		{
			case EnemyState.Idle:
				StopMovement();
				break;
			case EnemyState.MovingToTarget:
				StartMovement();
				break;
			case EnemyState.Attacking:
				StopMovement();
				break;
			case EnemyState.Victory:
				StopMovement();
				break;
			default:
				break;
		}
	}

	void SearchForTarget()
	{
		if (Tower.Instance != null)
		{
			_target = Tower.Instance;
			ChangeState(EnemyState.MovingToTarget);
		}
	}

	void MoveToTarget()
	{
		if (Time.time >= _nextDistanceCheck)
		{
			var distance = Vector3.Distance(transform.position, Tower.Instance.transform.position);
			if (distance <= _attackRange)
			{
				ChangeState(EnemyState.Attacking);
			}
			else
			{
				_nextDistanceCheck = GetNextDistanceCheckTime(distance);
			}
		}
	}

	void AttackTarget()
	{
		if (Time.time - _lastAttackTime > TimeBetweenAttacks)
		{
			StartAttack();
			_lastAttackTime = Time.time + GameController.Instance.RandomGenerator.Variance(TimeBetweenAttacks);
		}
	}

	void Celebrate()
	{
		// Victory state logic here, if any
	}

	void StartAttack()
	{
		_animator.SetTrigger("Attack");
	}

	public void PerformAttack()
	{
		if (_projectilePrefab == null)
		{
			InstantAttack();
			return;
		}

		var projectile = Instantiate(_projectilePrefab, _attackOrigin.position, _attackOrigin.rotation);
		projectile.Setup(_target, _baseDamage, false, _projectileSettings);
	}

	void InstantAttack()
	{
		if (_target != null)
		{
			_target.TakeDamage(Mathf.RoundToInt(_baseDamage));
		}

		if (_attackSound != null && AudioSource != null)
		{
			AudioSource.PlayOneShot(_attackSound);
		}
		else
		{
			Debug.LogWarning("EnemyAttack: No audio source or attack sound assigned to enemy.", gameObject);
		}
	}

	void HandleDeath(Target target)
	{
		StopMovement();

		_ = AllEnemies.Remove(this);
		ResourceManager.Instance.AddResource(Value);

		foreach (Transform child in transform)
		{
			child.gameObject.SetActive(false);
		}

		foreach (var component in GetComponents<MonoBehaviour>())
		{
			component.enabled = false;
		}

		var instance = Instantiate(_deathEffect, Center.position, Quaternion.LookRotation(-transform.forward));
		Destroy(instance, 10f);
		Destroy(gameObject, 3f);
	}

	public void StartMovement()
	{
		_ = _agent.SetDestination(Tower.Instance.transform.position);
		var distance = Vector2.Distance(transform.position, Tower.Instance.transform.position);
		_nextDistanceCheck = GetNextDistanceCheckTime(distance);
		_agent.isStopped = false;
		_bobbing.StartBobbing();
	}

	public void StopMovement()
	{
		if (_agent && _agent.enabled)
		{
			_agent.isStopped = true;
		}
		_bobbing.StopBobbing();
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
			enemy.ChangeState(EnemyState.Victory);
		}
	}

	public override void Die()
	{
		base.Die();

		OnAnyDeath(this);
	}

	float GetNextDistanceCheckTime(float distance)
	{
		return Time.time + Mathf.Max((distance - _attackRange) / _agent.speed, _minTimeBetweenDistanceChecks);
	}

	protected override void OnValidate()
	{
		base.OnValidate();
		if (_attackType == AttackType.MELEE)
		{
			_attackRange = 6f;
			_projectilePrefab = null;
		}
		else if (_attackType == AttackType.RANGED_PROJECTILE)
		{
			_attackSound = null;
		}
	}
}
