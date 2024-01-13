using Sirenix.OdinInspector;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
	public Projectile ProjectilePrefab;
	public float BaseDamage = 10f;
	public float AttackRange = 2f;
	public float TimeBetweenAttacks = 1f;

	[SerializeField, ChildGameObjectsOnly]
	Transform _attackOrigin;

	[SerializeField]
	[InlineProperty(LabelWidth = 140)]
	ProjectileSettings _projectileSettings;
	float _lastAttackTime = 0f;
	[HideInInspector] public Target Target;
	ITargeter _targeter;
	Animator _animator;

	void Awake()
	{
		_animator = GetComponentInChildren<Animator>();
		_targeter = GetComponentInParent<ITargeter>();

		if (_attackOrigin == null)
		{
			_attackOrigin = transform;
		}
	}

	void Update()
	{
		if (Target == null)
		{
			Target = _targeter.GetTarget(AttackRange);
		}
		else if (Time.time - _lastAttackTime > TimeBetweenAttacks)
		{
			StartAttack();
			_lastAttackTime = Time.time + RandomManager.RandomDelay(TimeBetweenAttacks);
		}
	}

	void StartAttack()
	{
		_animator.SetTrigger("Attack");
	}

	public void PerformAttack()
	{
		var projectile = Instantiate(ProjectilePrefab, _attackOrigin.position, _attackOrigin.rotation);
		projectile.Setup(Target, BaseDamage, false, _projectileSettings);
	}
}
