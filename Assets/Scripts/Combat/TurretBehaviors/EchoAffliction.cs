using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "VAKT/Affliction/Echo")]
public class EchoAffliction : Affliction
{
	public float Delay = 0.2f;
	public float Chance = 0.5f;

	public override bool PreferNewTarget => false;

	public override void Trigger(Turret turret, Target target)
	{
		var random = GameController.Instance.RandomGenerator.NextFloat();
		if (random < Chance)
		{

			_ = Tower.Instance.StartCoroutine(DelayedShoot(turret));
		}
	}

	public override void Tooltip(RichTextWithImages descriptionContainer, Turret turret)
	{
		var template = $"This turret has a {{Percent:{Chance}:DamageType}} chance to fire an additional projectile after a {{Flat:{Delay}:DamageType}} second delay. {{Newline}}<i>Echo projectiles can trigger any additional effects and critically hit.</i>";
		descriptionContainer.Write(template, turret.DamageType, turret.ShopType);
	}

	IEnumerator DelayedShoot(Turret turret)
	{
		yield return new WaitForSeconds(Delay);
		turret.Attack(true, this);
	}
}
