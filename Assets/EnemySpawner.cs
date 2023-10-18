using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public int minRadius = 30;
    public int maxRadius = 40;

    public float spawnRate = 1f;

    float lastSpawnTime = 0f;

    public Health target;
    public GameObject prefab;

    void Update()
    {
        if (Time.time - lastSpawnTime > spawnRate)
        {
            Spawn();
            lastSpawnTime = Time.time;
        }
    }

    void Spawn()
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        Vector3 spawnPosition = transform.position + new Vector3(randomDirection.x, 0, randomDirection.y) * Random.Range(minRadius, maxRadius);
        Quaternion rotation = Quaternion.LookRotation(transform.position - spawnPosition, Vector3.up);

        Instantiate(prefab, spawnPosition, rotation);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        // Draw circles for min and max radius, not spheres
        Draw2dCircle(transform.position, minRadius);
        Draw2dCircle(transform.position, maxRadius);

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
