using UnityEngine;

[CreateAssetMenu(menuName = "VAKT/TurretBehaviors/DamageBuildUp")]
public class DamageBuildUpBehavior : TurretBehavior
{
	public float DamageMultiplierPerStack = 0.1f;
	public int MaxStacks = 5;

	public override bool PreferNewTarget => false;

	public override void Execute(Turret turret, Target target)
	{
		var debuff = new DamageBuildUpDebuff(turret.Name, DamageMultiplierPerStack, MaxStacks);
		target.ApplyDebuff(debuff);
	}

	public override void Tooltip(RichTextWithImages descriptionContainer, StyleSettings styleSettings, Turret turret)
	{
		var template = $"Each hit increases damage by {{Percent:{DamageMultiplierPerStack}:DamageType}} up to {{Flat:{MaxStacks}:DamageType}} times.";
		descriptionContainer.Write(template, styleSettings, turret.DamageType, turret.ShopType);
	}
}
