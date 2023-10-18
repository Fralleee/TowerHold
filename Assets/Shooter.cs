using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    public float baseDamage = 10f;
    public float attackRange = 2f;
    public float timeBetweenAttacks = 1f;
    public float timeBetweenFindTarget = 1f;
    float lastAttackTime = 0f;

    float lastTargetSearch = 0f;

    public Projectile projectilePrefab;
    public Health target;

    ITargeter targeter;

    void Awake()
    {
        targeter = GetComponentInParent<ITargeter>();
    }

    public void Shoot()
    {
        var projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
        projectile.target = target;
        projectile.damage = baseDamage;
    }

    public void FindTarget()
    {
        target = targeter.GetTarget(attackRange);
    }

    void Update()
    {
        if (target == null)
        {
            if (Time.time - lastTargetSearch > timeBetweenFindTarget)
            {
                FindTarget();
                lastTargetSearch = Time.time;
            }
        }
        else
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
            if (distanceToTarget > attackRange)
            {
                target = null;
            }
            else if (Time.time - lastAttackTime > timeBetweenAttacks)
            {
                Shoot();
                lastAttackTime = Time.time;
            }
        }
    }
}
