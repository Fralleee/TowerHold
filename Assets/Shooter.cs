using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Shooter : MonoBehaviour
{
    public float baseDamage = 10f;
    public float attackRange = 2f;
    public float timeBetweenAttacks = 1f;
    public float timeBetweenFindTarget = .25f;
    float lastAttackTime = 0f;

    float lastTargetSearch = 0f;

    public Projectile projectilePrefab;
    public Health target;

    ITargeter targeter;

    void Awake()
    {
        targeter = GetComponentInParent<ITargeter>();
        lastAttackTime = Random.Range(0f, timeBetweenAttacks);
        lastTargetSearch = Random.Range(0f, timeBetweenFindTarget);
    }

    void Update()
    {
        if (target == null)
        {
            if (Time.time - lastTargetSearch > timeBetweenFindTarget)
            {
                target = targeter.GetTarget(attackRange);
                lastTargetSearch = Time.time;
            }
        }
        else if (Time.time - lastAttackTime > timeBetweenAttacks)
        {
            Shoot();
            lastAttackTime = Time.time;
        }
    }

    void Shoot()
    {
        var projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
        projectile.target = target;
        projectile.damage = baseDamage;
    }
}
