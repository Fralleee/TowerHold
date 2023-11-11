using System;
using UnityEngine;

public class Target : MonoBehaviour
{
    public static Action<Target> OnAnyDeath = delegate { };
    public Action<Target> OnDeath = delegate { };
    public Action<int> OnDamageTaken = delegate { };
    public Transform Center;
    public int MaxHealth = 100;
    public int Health = 100;
    [SerializeField] protected HealthBar _healthBar;
    [SerializeField] float healthBarOffset = 0f;

    public bool IsDead;

    protected virtual void Awake()
    {
        if (Center == null)
        {
            Center = transform;
        }
    }

    protected virtual void Start()
    {
        _healthBar = Instantiate(_healthBar, transform.position + Vector3.up * healthBarOffset, Quaternion.identity, transform);
    }

    public float TakeDamage(int damage)
    {
        if (IsDead)
        {
            return 0;
        }

        Health -= damage;
        _healthBar.SetHealth(Health);

        if (Health <= 0)
        {
            Die();
            var actualDamage = damage + Health;
            OnDamageTaken(actualDamage);
            return actualDamage;
        }
        OnDamageTaken(damage);
        return damage;

    }

    public void Die()
    {
        OnDeath(this);
        OnAnyDeath(this);
        IsDead = true;
    }
}