using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

[SelectionBase]
public class Enemy : Target
{
	public static List<Enemy> AllEnemies = new List<Enemy>();
	public static int AliveEnemies => AllEnemies.Count;

	[HideInInspector] public float DistanceToTower = 0;
	[HideInInspector] public int Attackers = 0;
	[HideInInspector] public bool HasAttackers => Attackers > 0;

	[Header("State")]
	[SerializeField, ReadOnly] EnemyState _currentState;

	[Header("Enemy Settings")]
	public int Value = 10;
	[SerializeField] GameObject _deathEffect;

	[Header("Attack Settings")]
	[SerializeField] EnemyAttackType _attackType;
	public DamageType DamageType;

	public float Damage = 10f;
	[SerializeField] float _attacksPerSecond = 1f;
	[HideIf("_attackType", EnemyAttackType.MELEE), SerializeField] AttackRange _attackRange = AttackRange.Melee;

	[Space(10)]
	[ShowIf("_attackType", EnemyAttackType.RANGED_PROJECTILE), SerializeField] Projectile _projectilePrefab;
	[ShowIf("_attackType", EnemyAttackType.RANGED_PROJECTILE), SerializeField, ChildGameObjectsOnly] Transform _attackOrigin;
	[ShowIf("_attackType", EnemyAttackType.RANGED_PROJECTILE), InlineProperty(LabelWidth = 140)] public ProjectileSettings ProjectileSettings;
	[HideIf("_attackType", EnemyAttackType.RANGED_PROJECTILE), SerializeField] AudioClip _attackSound;

	Target _target;
	Animator _animator;
	NavMeshAgent _agent;
	Bobbing _bobbing;

	public EnemyAttackInformation GetAttackInformation()
	{
		return new EnemyAttackInformation
		{
			Damage = Damage,
			DamageType = DamageType,
			AttacksPerSecond = _attacksPerSecond,
			AttackRange = _attackRange
		};
	}

	readonly int _attackTrigger = Animator.StringToHash("Attack");
	readonly float _timeBetweenDistanceChecks = 0.5f;
	float _lastAttackTime = 0f;
	float _nextDistanceCheck = 0f;
	float _attackRangeValue;
	float TimeBetweenAttacks => 1f / _attacksPerSecond;

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

		_attackRangeValue = _attackRange.GetRange();

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
				TurnTowardsTarget();
				AttackTarget();
				break;
			case EnemyState.Victory:
				Celebrate();
				break;
			default:
				break;
		}
	}

	public override float TakeDamage(int baseDamage, DamageType damageType)
	{
		var actualDamage = base.TakeDamage(baseDamage, damageType);
		if (actualDamage > 0)
		{
			AudioManager.PlayEffect(SoundEffect.Hit);
		}
		return actualDamage;
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
			DistanceToTower = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(Tower.Instance.transform.position.x, Tower.Instance.transform.position.z));
			if (DistanceToTower <= _attackRangeValue)
			{
				ChangeState(EnemyState.Attacking);
			}
			else
			{
				_nextDistanceCheck = Time.time + _timeBetweenDistanceChecks;
			}
		}
	}

	void TurnTowardsTarget()
	{
		if (_target != null)
		{
			var lookPos = _target.transform.position - transform.position;
			lookPos.y = 0;
			var rotation = Quaternion.LookRotation(lookPos);
			transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5);
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
		_animator.SetTrigger(_attackTrigger);
	}

	public void PerformAttack()
	{
		if (_projectilePrefab == null)
		{
			InstantAttack();
			return;
		}

		var projectile = Instantiate(_projectilePrefab, _attackOrigin.position, _attackOrigin.rotation);
		projectile.Setup(_target, this);
	}

	void InstantAttack()
	{
		if (_target != null)
		{
			_target.TakeDamage(Mathf.RoundToInt(Damage), DamageType);
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

	public override void Die()
	{
		base.Die();

		StopMovement();

		_ = AllEnemies.Remove(this);
		ResourceManager.Instance.AddResource(Value, transform.position + (Vector3.up * 2));

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

		EventBus<EnemyDeathEvent>.Raise(new EnemyDeathEvent { Enemy = this });
	}

	public void StartMovement()
	{
		_ = _agent.SetDestination(Tower.Instance.transform.position);
		DistanceToTower = Vector3.Distance(transform.position, Tower.Instance.transform.position);
		_nextDistanceCheck = Time.time + _timeBetweenDistanceChecks;
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
	}

	public static void GameOver()
	{
		foreach (var enemy in AllEnemies)
		{
			enemy.ChangeState(EnemyState.Victory);
		}
	}

	protected override void OnValidate()
	{
		base.OnValidate();
		if (_attackType == EnemyAttackType.MELEE)
		{
			_attackRange = AttackRange.Melee;
			_projectilePrefab = null;
		}
		else if (_attackType == EnemyAttackType.RANGED_PROJECTILE)
		{
			_attackSound = null;
		}
	}
}
