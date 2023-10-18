using System.Collections.Generic;
using UnityEngine;

public class TowerTargeting : MonoBehaviour, ITargeter
{
    public float selectValidEnemiesInterval = 1f;
    float lastValidEnemiesUpdate = 0f;

    float maxRange = 40f;

    void Update()
    {
        if (Time.time - lastValidEnemiesUpdate > selectValidEnemiesInterval)
        {
            lastValidEnemiesUpdate = Time.time;
            validEnemies.Clear();
            SelectValidEnemies();
        }
    }

    void SelectValidEnemies()
    {
        Enemy[] enemies = Enemy.AllEnemies.ToArray();
        foreach (Enemy enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy <= maxRange)
            {
                validEnemies.Add(enemy);
            }
        }
    }

    List<Enemy> validEnemies = new List<Enemy>();
    public Health GetTarget(float targetingRange)
    {
        if (maxRange < targetingRange)
        {
            maxRange = targetingRange;
            SelectValidEnemies();
        }

        Enemy selectedTarget = null;

        foreach (Enemy enemy in validEnemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy <= targetingRange)
            {
                if (distanceToEnemy <= targetingRange)
                {
                    selectedTarget = enemy;
                }
            }
        }

        if (selectedTarget == null)
        {
            return null;
        }

        return selectedTarget.health;
    }
}