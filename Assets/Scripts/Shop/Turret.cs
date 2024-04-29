using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "VAKT/Shop/Turret")]
public class Turret : DamageShopItem
{
	[Header("Turret Settings")]
	[SerializeField] float _baseDamage = 10f;
	[SerializeField] AttackRange _attackRange = AttackRange.Short;
	[SerializeField] float _timeBetweenAttacks = 1f;
	[SerializeField] float _criticalHitChance = 0f;
	[SerializeField] float _criticalHitMultiplier = 2f;
	public List<TurretBehavior> Behaviors = new List<TurretBehavior>();

	[SerializeField] Projectile _projectilePrefab;
	[ShowIf("_projectilePrefab")][SerializeField][InlineProperty(LabelWidth = 140)] ProjectileSettings _projectileSettings;
	[HideIf("_projectilePrefab")][SerializeField] AudioClip _attackSound;
	[HideIf("_projectilePrefab")][SerializeField] AudioSettings _audioSettings;
	[HideIf("_projectilePrefab")][SerializeField] GameObject _impactParticle;

	float _lastAttackTime = 0f;
	float _attackRangeValue;
	bool _preferNewTarget = false;
	Tower _tower;
	Enemy _target;
	AudioSource _audioSource;

	public void Reset()
	{
		Description = "Shoots towards nearest enemy causing #Damage# damage.";
	}

	public void Setup(Tower inputTower)
	{
		_tower = inputTower;
		_attackRangeValue = _attackRange.GetRange();
		_lastAttackTime = GameController.Instance.RandomGenerator.NextFloat(0f, _timeBetweenAttacks); // Add random delay for the first attack
		_audioSource = _tower.GetComponent<AudioSource>();
		_preferNewTarget = Behaviors.Any(behavior => behavior.PreferNewTarget);
	}

	public void FixedUpdate()
	{
		if (Time.time - _lastAttackTime > _timeBetweenAttacks)
		{
			_target = TowerTargeter.FindTargets(_attackRangeValue, _preferNewTarget ? Name : null);
			_lastAttackTime = Time.time;

			if (_target != null && !_target.IsDead)
			{
				Shoot();
			}
		}

	}

	public override void OnPurchase()
	{
		base.OnPurchase();

		Tower.Instance.AddTurret(this);
		ScoreManager.Instance.Turrets += 1;
	}

	public (float baseDamage, AttackRange attackRange, float timeBetweenAttacks, float criticalHitChance, float criticalHitMultiplier, string description) GetHoverData()
	{
		return (_baseDamage, _attackRange, _timeBetweenAttacks, _criticalHitChance, _criticalHitMultiplier, Description);
	}

	public void Shoot(bool executeBehaviors = true, TurretBehavior excludeBehavior = null)
	{
		if (_target == null || _target.IsDead)
		{
			return;
		}

		if (_projectilePrefab == null)
		{
			InstantAttack(executeBehaviors, excludeBehavior);
		}
		else
		{
			ProjectileAttack(executeBehaviors, excludeBehavior);
		}
	}

	void ExecuteBehaviors(TurretBehavior excludeBehavior = null)
	{
		foreach (var behavior in Behaviors)
		{
			if (behavior == excludeBehavior)
			{
				continue;
			}
			behavior.Execute(this, _target);
		}
	}

	void InstantAttack(bool executeBehaviors = true, TurretBehavior excludeBehavior = null)
	{
		if (_target != null)
		{
			var damage = _tower.GetDamage(DamageType, ShopType, _baseDamage, _criticalHitChance, _criticalHitMultiplier);
			_ = _target.TakeDamage(Mathf.RoundToInt(damage));

			if (executeBehaviors)
			{
				ExecuteBehaviors(excludeBehavior);
			}

			if (_impactParticle)
			{
				var direction = (_target.Center.position - _tower.Center.position).normalized;
				_impactParticle = Instantiate(_impactParticle, _target.Center.position, Quaternion.FromToRotation(Vector3.up, -direction));
				Destroy(_impactParticle, 5.0f);
			}
		}

		if (_attackSound != null && _audioSource != null)
		{
			_audioSettings.ApplySettings(_audioSource);
			_audioSource.PlayOneShot(_attackSound);
		}
		else
		{
			Debug.LogWarning("EnemyAttack: No audio source or attack sound assigned to enemy.");
		}
	}

	void ProjectileAttack(bool executeBehaviors = true, TurretBehavior excludeBehavior = null)
	{
		var rotation = Quaternion.LookRotation(_target.transform.position - _tower.Center.position);
		var projectile = Instantiate(_projectilePrefab, _tower.Center.position, rotation);
		var damage = _tower.GetDamage(DamageType, ShopType, _baseDamage, _criticalHitChance, _criticalHitMultiplier);
		projectile.Setup(_target, damage, _projectileSettings, this, executeBehaviors, excludeBehavior);
	}

	public override TooltipContent Tooltip(StyleSettings styleSettings)
	{
		var tooltip = new TurretTooltipContent();
		tooltip.UpdateInformation(this, styleSettings);
		return tooltip;
	}
}
