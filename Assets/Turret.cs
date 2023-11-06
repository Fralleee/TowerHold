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
    Health target;

    public void Setup(Tower inputTower)
    {
        tower = inputTower;
        lastTargetSearch = Random.Range(0f, timeBetweenFindTarget);
    }

    public void Update()
    {
        if (target == null)
        {
            if (Time.time - lastTargetSearch > timeBetweenFindTarget)
            {
                target = Targeter.GetTurretTarget(tower.center, attackRange);
                lastTargetSearch = Time.time;
            }
        }
        else if (Time.time - lastAttackTime > timeBetweenAttacks)
        {
            Shoot();
            lastAttackTime = Time.time;
            lastTargetSearch = 0;
        }
    }

    public override void OnPurchase()
    {
        Tower.instance.AddTurret(this);
        ScoreManager.Instance.turrets += 1;
    }

    void Shoot()
    {
        var rotation = Quaternion.LookRotation(target.transform.position - tower.center.position);
        var projectile = Instantiate(projectilePrefab, tower.center.position, rotation);
        projectile.Setup(target, tower.GetDamage(damageType, baseDamage), true);
    }
}
