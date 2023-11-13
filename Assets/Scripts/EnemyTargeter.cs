using UnityEngine;

public class EnemyTargeter : MonoBehaviour, ITargeter
{
    public float checkDistanceInterval = 1f;
    float lastDistanceCheck = 0f;
    float maxRange = 0;

    Target target;

    void Update()
    {
        if (target == null && Time.time - lastDistanceCheck > checkDistanceInterval)
        {
            lastDistanceCheck = Time.time;
            CheckDistance();
        }
    }

    void CheckDistance()
    {
        float distanceToTarget = Vector3.Distance(transform.position, Tower.instance.transform.position);
        if (distanceToTarget < maxRange)
        {
            target = Tower.instance;
        }
    }

    public Target GetTarget(float range)
    {
        maxRange = range;
        return target;
    }
}
