using UnityEngine;

public class EnemyTargeter : MonoBehaviour, ITargeter
{
    public Health GetTarget(float maxTargetingRange = 10)
    {
        return Tower.asTarget;
    }
}
