using UnityEngine;

[CreateAssetMenu(menuName = "VAKT/TurretBehaviors/DamageBuildUp")]
public class DamageBuildUpBehavior : TurretBehavior
{
	public float DamageMultiplierPerStack = 0.1f;
	public int MaxStacks = 5;

	public override void Execute(Turret turret, Enemy target)
	{
		DamageBuildUpDebuff debuff;
		if (target.ActiveDebuffs.ContainsKey(turret.Name))
		{
			debuff = target.ActiveDebuffs[turret.Name] as DamageBuildUpDebuff;
			debuff?.Refresh();
		}
		else
		{
			debuff = new DamageBuildUpDebuff(turret.Name, DamageMultiplierPerStack, MaxStacks);
			target.ApplyDebuff(debuff);
		}

		turret.Shoot(false, this);
	}

	public override void Tooltip(RichTextWithImages descriptionContainer, StyleSettings styleSettings, Turret turret)
	{
		var template = "Each hit increases damage by {PercentDamageType} up to {Damage} times.";
		descriptionContainer.Write(template, styleSettings, MaxStacks, 0, DamageMultiplierPerStack, turret.DamageType, turret.ShopType);
	}
}
