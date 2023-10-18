using UnityEngine;

public interface ITargeter
{
    public Health GetTarget(float maxTargetingRange = 10f);
}
