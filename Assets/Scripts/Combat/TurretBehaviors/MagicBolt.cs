using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "VAKT/TurretBehaviors/Echo")]
public class EchoBehavior : TurretBehavior
{
	public float Delay = 0.2f;
	public float Chance = 0.5f;

	public override void Execute(Turret turret, Enemy target)
	{
		var random = GameController.Instance.RandomGenerator.NextFloat();
		if (random < Chance)
		{

			_ = Tower.Instance.StartCoroutine(DelayedShoot(turret));
		}
	}

	public override void Tooltip(RichTextWithImages descriptionContainer, StyleSettings styleSettings, Turret turret)
	{
		var template = "This turret has a {PercentDamageType} chance to fire an additional projectile after a {Duration} second delay. {Newline}<i>Echo projectiles can trigger any additional effects and critically hit.</i>";
		descriptionContainer.Write(template, styleSettings, 0, Delay, Chance, turret.DamageType, turret.ShopType);
	}

	IEnumerator DelayedShoot(Turret turret)
	{
		yield return new WaitForSeconds(Delay);
		turret.Shoot(true, this);
	}
}
