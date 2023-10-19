using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public static List<Enemy> AllEnemies = new List<Enemy>();

    [HideInInspector]
    public Health health;

    public bool IsTargeted;

    void Awake()
    {
        AllEnemies.Add(this);
        health = GetComponent<Health>();
    }

    void OnDestroy()
    {
        AllEnemies.Remove(this);
    }
}
