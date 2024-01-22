using Sirenix.OdinInspector;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
	[HideInInspector] public Target Target;

	[SerializeField] float _baseDamage = 10f;
	[SerializeField] float _attackRange = 4f;
	[SerializeField] float _timeBetweenAttacks = 1f;

	[SerializeField] Projectile _projectilePrefab;
	[ShowIf("_projectilePrefab")][SerializeField, ChildGameObjectsOnly] Transform _attackOrigin;
	[ShowIf("_projectilePrefab")][SerializeField][InlineProperty(LabelWidth = 140)] ProjectileSettings _projectileSettings;
	[HideIf("_projectilePrefab")][SerializeField] AudioClip _attackSound;
	[HideIf("_projectilePrefab")][SerializeField] AudioSettings _audioSettings;

	AudioSource _audioSource;
	ITargeter _targeter;
	Animator _animator;
	float _lastAttackTime = 0f;

	void Awake()
	{
		_animator = GetComponentInChildren<Animator>();
		_targeter = GetComponentInParent<ITargeter>();
		_audioSource = GetComponent<AudioSource>();

		if (_attackOrigin == null)
		{
			_attackOrigin = transform;
		}
	}

	void Update()
	{
		if (Target == null)
		{
			Target = _targeter.GetTarget(_attackRange);
		}
		else if (Time.time - _lastAttackTime > _timeBetweenAttacks)
		{
			StartAttack();
			_lastAttackTime = Time.time + GameController.Instance.RandomGenerator.Variance(_timeBetweenAttacks);
		}
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
		projectile.Setup(Target, _baseDamage, false, _projectileSettings);
	}

	void InstantAttack()
	{
		if (Target != null)
		{
			Target.TakeDamage(Mathf.RoundToInt(_baseDamage));
		}

		if (_attackSound != null && _audioSource != null)
		{
			_audioSettings.ApplySettings(_audioSource);
			_audioSource.PlayOneShot(_attackSound);
		}
		else
		{
			Debug.LogWarning("EnemyAttack: No audio source or attack sound assigned to enemy.", gameObject);
		}
	}
}
