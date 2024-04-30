using System.Collections.Generic;
using UnityEngine;

public abstract class AttackType : ScriptableObject
{
	public abstract IEnumerable<Enemy> AcquireTargets(Turret turret);
	public abstract void ExecuteAttack(IEnumerable<Enemy> targets, Turret turret, bool executeBehaviors = true, Affliction excludeBehavior = null);
}
