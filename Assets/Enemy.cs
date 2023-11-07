using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Target
{
    [SerializeField] int bounty = 10;
    public static List<Enemy> AllEnemies = new List<Enemy>();

    public int attackers = 0;
    public bool HasAttackers => attackers > 0;

    protected override void Awake()
    {
        base.Awake();

        AllEnemies.Add(this);

        // Increase bounty based on level
        bounty += GameController.Instance.currentLevel * 2;
    }

    void OnDestroy()
    {
        AllEnemies.Remove(this);

        GoldManager.Instance.EarnGold(bounty);
    }

    public static void ResetGameState()
    {
        AllEnemies.Clear();
        OnDeath = delegate { };
    }
}
