using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "VAKT/Shop/Turret")]
public class Turret : DamageShopItem
{
	[Header("Turret Settings")]
	[SerializeField] float _baseDamage = 10f;
	[SerializeField] AttackRange _attackRange = AttackRange.Short;
	[SerializeField] float _timeBetweenAttacks = 1f;
	[SerializeField] float _timeBetweenFindTarget = 1f;
	[SerializeField] float _criticalHitChance = 0f;
	[SerializeField] float _criticalHitMultiplier = 2f;

	[SerializeField] Projectile _projectilePrefab;
	[ShowIf("_projectilePrefab")][SerializeField][InlineProperty(LabelWidth = 140)] ProjectileSettings _projectileSettings;
	[HideIf("_projectilePrefab")][SerializeField] AudioClip _attackSound;
	[HideIf("_projectilePrefab")][SerializeField] AudioSettings _audioSettings;
	[HideIf("_projectilePrefab")][SerializeField] GameObject _impactParticle;

	[Header("Damage over time settings")]
	[SerializeField] bool _isDamageOverTime;
	[ShowIf("_isDamageOverTime")][SerializeField] float _dotDuration = 5f;
	[ShowIf("_isDamageOverTime")][SerializeField] float _dotTotalDamage = 10f;
	[ShowIf("_isDamageOverTime")][SerializeField] float _dotTickRate = 1f;

	float _lastAttackTime = 0f;
	float _lastTargetSearch = 0f;
	Tower _tower;
	Enemy _target;
	AudioSource _audioSource;

	public void Reset()
	{
		Description = "Shoots towards nearest enemy causing {DamageAmount} {DamageType} damage.";
	}

	public void Setup(Tower inputTower)
	{
		_tower = inputTower;
		_lastTargetSearch = GameController.Instance.RandomGenerator.NextFloat(0f, _timeBetweenFindTarget);
		_lastAttackTime = GameController.Instance.RandomGenerator.NextFloat(0f, _timeBetweenAttacks); // Add random delay for the first attack
		_audioSource = _tower.GetComponent<AudioSource>();
	}

	public void FixedUpdate()
	{
		if (Time.time - _lastTargetSearch > _timeBetweenFindTarget)
		{
			_target = TowerTargeter.GetEnemyTarget(_attackRange, _isDamageOverTime ? Name : null);
			_lastTargetSearch = Time.time + GameController.Instance.RandomGenerator.Variance(_timeBetweenFindTarget); // Add some variance to the search timing
		}

		if (_target != null && !_target.IsDead && Time.time - _lastAttackTime > _timeBetweenAttacks)
		{
			Shoot();
			_lastAttackTime = Time.time + GameController.Instance.RandomGenerator.Variance(_timeBetweenAttacks); // Add some variance to the attack timing
			_lastTargetSearch = Time.time; // This should probably be adjusted to have a delay as well
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

	void Shoot()
	{
		if (_projectilePrefab == null)
		{
			InstantAttack();
			return;
		}
		var rotation = Quaternion.LookRotation(_target.transform.position - _tower.Center.position);
		var projectile = Instantiate(_projectilePrefab, _tower.Center.position, rotation);
		projectile.Setup(_target, _tower.GetDamage(DamageType, ShopType, _baseDamage, _criticalHitChance, _criticalHitMultiplier), true, _projectileSettings);
		if (_isDamageOverTime)
		{
			projectile.SetupDamageOverTime(_dotDuration, _dotTotalDamage, _dotTickRate);
		}
	}

	void InstantAttack()
	{
		if (_target != null)
		{
			var damage = _tower.GetDamage(DamageType, ShopType, _baseDamage, _criticalHitChance, _criticalHitMultiplier);
			_ = _target.TakeDamage(Mathf.RoundToInt(damage));

			if (_isDamageOverTime)
			{
				_target.ApplyDebuff(new DamageOverTimeDebuff(Name, _dotDuration, _dotTotalDamage, _dotTickRate));
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

	public override TooltipContent Tooltip(StyleSettings styleSettings)
	{
		var tooltip = new TurretTooltipContent();
		tooltip.UpdateInformation(this, styleSettings);
		return tooltip;
	}
}
