using System;
using UnityEditor;
using UnityEngine;

public class Health : MonoBehaviour
{
    public Transform center;
    public int maxHealth = 100;
    public int health = 100;

    void Awake()
    {
        if (center == null)
        {
            center = transform;
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}