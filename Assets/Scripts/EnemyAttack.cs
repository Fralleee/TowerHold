using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
	public Projectile ProjectilePrefab;
	public float BaseDamage = 10f;
	public float AttackRange = 2f;
	public float TimeBetweenAttacks = 1f;
	float _lastAttackTime = 0f;
	[HideInInspector] public Target Target;
	ITargeter _targeter;
	Animator _animator;

	void Awake()
	{
		_animator = GetComponentInChildren<Animator>();
		_targeter = GetComponentInParent<ITargeter>();
	}

	void Update()
	{
		if (Target == null)
		{
			Target = _targeter.GetTarget(AttackRange);
		}
		else if (Time.time - _lastAttackTime > TimeBetweenAttacks)
		{
			Shoot();
			_lastAttackTime = Time.time;
		}
	}

	void Shoot()
	{
		var projectile = Instantiate(ProjectilePrefab, transform.position, transform.rotation);
		projectile.Setup(Target, BaseDamage, false);
		_animator.SetTrigger("Attack");
	}
}
