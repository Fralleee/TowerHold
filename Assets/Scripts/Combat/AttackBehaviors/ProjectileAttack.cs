using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "VAKT/AttackType/ProjectileAttack")]
public class ProjectileAttack : AttackType
{
	[SerializeField, InlineProperty(LabelWidth = 140)] ProjectileSettings _projectileSettings;

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
			var rotation = Quaternion.LookRotation(target.transform.position - tower.Center.position);
			var projectile = Instantiate(_projectileSettings.ProjectilePrefab, tower.Center.position, rotation);
			var damage = tower.GetDamage(turret);
			projectile.Setup(target, turret, _projectileSettings, executeBehaviors, excludeBehavior);

			turret.PlayAttackSound();
		}
	}
}
