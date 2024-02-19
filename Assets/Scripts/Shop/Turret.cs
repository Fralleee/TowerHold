using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Tower/Turret")]
public class Turret : DamageShopItem
{
	[Header("Turret Settings")]
	[SerializeField] float _baseDamage = 10f;
	[SerializeField] AttackRange _attackRange = AttackRange.Short;
	[SerializeField] float _timeBetweenAttacks = 1f;
	[SerializeField] float _timeBetweenFindTarget = 1f;

	[SerializeField] Projectile _projectilePrefab;
	[ShowIf("_projectilePrefab")][SerializeField][InlineProperty(LabelWidth = 140)] ProjectileSettings _projectileSettings;
	[HideIf("_projectilePrefab")][SerializeField] AudioClip _attackSound;
	[HideIf("_projectilePrefab")][SerializeField] AudioSettings _audioSettings;

	float _lastAttackTime = 0f;
	float _lastTargetSearch = 0f;
	Tower _tower;
	Enemy _target;
	AudioSource _audioSource;

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
			_target = TowerTargeter.GetEnemyTarget(_attackRange);
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
		Tower.Instance.AddTurret(this);
		ScoreManager.Instance.Turrets += 1;
	}

	public (float baseDamage, float attackRange, float timeBetweenAttacks) GetHoverData() => (_baseDamage, _attackRange.GetRange(), _timeBetweenAttacks);

	void Shoot()
	{
		if (_projectilePrefab == null)
		{
			InstantAttack();
			return;
		}
		var rotation = Quaternion.LookRotation(_target.transform.position - _tower.Center.position);
		var projectile = Instantiate(_projectilePrefab, _tower.Center.position, rotation);
		projectile.Setup(_target, _tower.GetDamage(Category, _baseDamage), true, _projectileSettings);
	}

	void InstantAttack()
	{
		if (_target != null)
		{
			var damage = _tower.GetDamage(Category, _baseDamage);
			_target.TakeDamage(Mathf.RoundToInt(damage));
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

	public override TooltipContent Tooltip()
	{
		var tooltip = new TurretTooltipContent();
		tooltip.UpdateInformation(this);
		return tooltip;
	}
}
