using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tower/Turret")]
public partial class Turret : ShopItem
{
    [Header("Turret Settings")]
    public Projectile projectilePrefab;
    public DamageType damageType;
    public float baseDamage = 10f;
    public float attackRange = 2f;
    public float timeBetweenAttacks = 1f;
    public float timeBetweenFindTarget = 1f;
    float lastAttackTime = 0f;
    float lastTargetSearch = 0f;
    Tower tower;
    Target target;

    public void Setup(Tower inputTower)
    {
        tower = inputTower;
        lastTargetSearch = Random.Range(0f, timeBetweenFindTarget);
        lastAttackTime = Random.Range(0f, timeBetweenAttacks); // Add random delay for the first attack
    }

    public void Update()
    {
        if (Time.time - lastTargetSearch > timeBetweenFindTarget)
        {
            target = TowerTargeter.GetTurretTarget(tower.Center, attackRange);
            lastTargetSearch = Time.time + Random.Range(-0.1f * timeBetweenFindTarget, 0.1f * timeBetweenFindTarget); // Add some variance to the search timing
        }

        if (target != null && !target.IsDead && Time.time - lastAttackTime > timeBetweenAttacks)
        {
            Shoot();
            lastAttackTime = Time.time + Random.Range(-0.1f * timeBetweenAttacks, 0.1f * timeBetweenAttacks); // Add some variance to the attack timing
            lastTargetSearch = Time.time; // This should probably be adjusted to have a delay as well
        }
    }

    public override void OnPurchase()
    {
        Tower.instance.AddTurret(this);
        ScoreManager.Instance.turrets += 1;
    }

    void Shoot()
    {
        var rotation = Quaternion.LookRotation(target.transform.position - tower.Center.position);
        var projectile = Instantiate(projectilePrefab, tower.Center.position, rotation);
        projectile.Setup(target, tower.GetDamage(damageType, baseDamage), true);
    }
}
