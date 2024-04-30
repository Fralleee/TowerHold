using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "VAKT/AttackType/InstantAttack")]
public class InstantAttack : AttackType
{
	[SerializeField] GameObject _impactParticle;

	public override IEnumerable<Enemy> AcquireTargets(Turret turret)
	{
		var target = TowerTargeter.FindTargets(turret.AttackRange.GetRange(), turret.PreferNewTarget ? turret.Name : null);
		return target != null ? new List<Enemy> { target } : new List<Enemy>();
	}

	public override void ExecuteAttack(IEnumerable<Enemy> targets, Turret turret, bool executeBehaviors = true, Affliction excludeBehavior = null)
	{
		var target = targets.First();
		if (target != null && !target.IsDead)
		{
			var tower = Tower.Instance;
			var damage = tower.GetDamage(turret);
			_ = target.TakeDamage(Mathf.RoundToInt(damage));

			if (executeBehaviors)
			{
				turret.Afflictions.TriggerAfflictions(target, excludeBehavior);
			}

			if (_impactParticle)
			{
				var direction = (target.Center.position - tower.Center.position).normalized;
				_impactParticle = Instantiate(_impactParticle, target.Center.position, Quaternion.FromToRotation(Vector3.up, -direction));
				Destroy(_impactParticle, 5.0f);
			}

			turret.PlayAttackSound();
		}
	}
}
