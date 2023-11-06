using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Health
{
    public static List<Enemy> AllEnemies = new List<Enemy>();

    public bool IsTargeted;

    protected override void Awake()
    {
        base.Awake();

        AllEnemies.Add(this);
    }

    void OnDestroy()
    {
        AllEnemies.Remove(this);
    }


    public static void ResetGameState()
    {
        AllEnemies = new List<Enemy>();
        OnDeath = delegate { };
    }
}
