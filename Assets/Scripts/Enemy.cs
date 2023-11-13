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
    Animator animator;

    protected override void Awake()
    {
        base.Awake();

        animator = GetComponentInChildren<Animator>();

        AllEnemies.Add(this);

        // Increase bounty based on level
        bounty += GameController.Instance.currentLevel * 2;

        OnDeath += HandleDeath;
    }

    void HandleDeath(Target target)
    {
        AllEnemies.Remove(this);
        GoldManager.Instance.EarnGold(bounty);
        animator.SetTrigger("Die");

        // Disable all monobehaviours on gameObject
        foreach (var component in GetComponents<MonoBehaviour>())
        {
            component.enabled = false;
        }

        Destroy(gameObject, 3f);
    }

    public static void ResetGameState()
    {
        AllEnemies.Clear();
        OnAnyDeath = delegate { };
    }

    public static void GameOver()
    {
        foreach (var enemy in AllEnemies)
        {
            enemy.animator.SetTrigger("Victory");
            enemy.GetComponent<MoveToAttack>().Stop();
            foreach (var component in enemy.GetComponents<MonoBehaviour>())
            {
                component.enabled = false;
            }
        }
    }
}
