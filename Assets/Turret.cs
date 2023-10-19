using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tower/Turret")]
public partial class Turret : ScriptableObject
{
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
                target = Targeter.GetTurretTarget(tower.origin, attackRange);
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

    void Shoot()
    {
        var rotation = Quaternion.LookRotation(target.transform.position - tower.origin.position);
        var projectile = Instantiate(projectilePrefab, tower.origin.position, rotation);
        projectile.target = target;
        projectile.damage = tower.GetDamage(damageType, baseDamage);
    }
}
