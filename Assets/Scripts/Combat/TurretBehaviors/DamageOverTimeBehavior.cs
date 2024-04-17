using UnityEngine;

[CreateAssetMenu(menuName = "VAKT/TurretBehaviors/DamageOverTimeBehavior")]
public class DamageOverTimeBehavior : TurretBehavior
{
	public float Duration = 5f;
	public float TotalDamage = 50f;
	public float TickRate = 1f;
	public GameObject ImpactParticle;
	public override bool PreferNewTarget => true;

	public override void Execute(Turret turret, Enemy target)
	{
		var debuff = new DamageOverTimeDebuff(turret.Name, Duration, TotalDamage, TickRate, ImpactParticle);
		target.ApplyDebuff(debuff);
	}

	public override void Tooltip(RichTextWithImages descriptionContainer, StyleSettings styleSettings, Turret turret)
	{
		var template = "Deals {DPS} {DamageType} damage per second for {Duration} seconds for a total of {Damage} {DamageType} damage.";
		descriptionContainer.Write(template, styleSettings, TotalDamage, Duration, 0, turret.DamageType, turret.ShopType);
	}
}
