using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    public static Action<Health> OnDeath = delegate { };
    public Action<int> OnDamageTaken = delegate { };
    public Transform center;
    public int maxHealth = 100;
    public int health = 100;

    protected virtual void Awake()
    {
        if (center == null)
        {
            center = transform;
        }
    }

    public float TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
            var actualDamage = damage + health;
            OnDamageTaken(actualDamage);
            return actualDamage;
        }
        OnDamageTaken(damage);
        return damage;

    }

    public void Die()
    {
        OnDeath(this);
        Destroy(gameObject);
    }
}