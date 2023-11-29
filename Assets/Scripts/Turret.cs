using UnityEngine;

[CreateAssetMenu(menuName = "Tower/Turret")]
public partial class Turret : DamageShopItem
{
	[Header("Turret Settings")]
	public Projectile ProjectilePrefab;
	public float BaseDamage = 10f;
	public float AttackRange = 2f;
	public float TimeBetweenAttacks = 1f;
	public float TimeBetweenFindTarget = 1f;
	public AudioClip ShootSound;
	float _lastAttackTime = 0f;
	float _lastTargetSearch = 0f;
	Tower _tower;
	Target _target;

	public void Setup(Tower inputTower)
	{
		_tower = inputTower;
		_lastTargetSearch = Random.Range(0f, TimeBetweenFindTarget);
		_lastAttackTime = Random.Range(0f, TimeBetweenAttacks); // Add random delay for the first attack
	}

	public void Update()
	{
		if (Time.time - _lastTargetSearch > TimeBetweenFindTarget)
		{
			_target = TowerTargeter.GetTurretTarget(_tower.Center, AttackRange);
			_lastTargetSearch = Time.time + Random.Range(-0.1f * TimeBetweenFindTarget, 0.1f * TimeBetweenFindTarget); // Add some variance to the search timing
		}

		if (_target != null && !_target.IsDead && Time.time - _lastAttackTime > TimeBetweenAttacks)
		{
			Shoot();
			_lastAttackTime = Time.time + Random.Range(-0.1f * TimeBetweenAttacks, 0.1f * TimeBetweenAttacks); // Add some variance to the attack timing
			_lastTargetSearch = Time.time; // This should probably be adjusted to have a delay as well
		}
	}

	public override void OnPurchase()
	{
		Tower.Instance.AddTurret(this);
		ScoreManager.Instance.Turrets += 1;
	}

	void Shoot()
	{
		var rotation = Quaternion.LookRotation(_target.transform.position - _tower.Center.position);
		var projectile = Instantiate(ProjectilePrefab, _tower.Center.position, rotation);
		projectile.Setup(_target, _tower.GetDamage(Category, BaseDamage), true);

		SoundManager.Instance.PlayEffect(ShootSound);
	}
}
