using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    [HideInInspector] public bool IsSpawning;
    [HideInInspector] public Tower target;
    float lastSpawnTime = 0f;

    void Update()
    {
        if (IsSpawning && Time.time - lastSpawnTime > GameController.Instance.enemySpawnerSettings.spawnRate)
        {
            Spawn();
            lastSpawnTime = Time.time;
        }
    }

    void Spawn()
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        Vector3 spawnPosition = transform.position + new Vector3(randomDirection.x, 0, randomDirection.y) * Random.Range(GameController.Instance.enemySpawnerSettings.minRadius, GameController.Instance.enemySpawnerSettings.maxRadius);
        Quaternion rotation = Quaternion.LookRotation(transform.position - spawnPosition, Vector3.up);

        Instantiate(GameController.Instance.enemySpawnerSettings.prefab, spawnPosition, rotation);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        // Draw circles for min and max radius, not spheres
        Draw2dCircle(transform.position, GameController.Instance.enemySpawnerSettings.minRadius);
        Draw2dCircle(transform.position, GameController.Instance.enemySpawnerSettings.maxRadius);

    }

    void Draw2dCircle(Vector3 center, float radius)
    {
        Vector3 prevPos = center + new Vector3(radius, 0, 0);
        for (int i = 0; i < 30; i++)
        {
            float angle = (float)i / 30f * Mathf.PI * 2f;
            Vector3 newPos = center + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
            Gizmos.DrawLine(prevPos, newPos);
            prevPos = newPos;
        }
    }
}
