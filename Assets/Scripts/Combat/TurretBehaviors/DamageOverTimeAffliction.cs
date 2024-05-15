using UnityEngine;

[CreateAssetMenu(menuName = "VAKT/Affliction/DamageOverTime")]
public class DamageOverTimeAffliction : Affliction
{
	public float Duration = 5f;
	public float TotalDamage = 50f;
	public float TickRate = 1f;
	public GameObject ImpactParticle;
	public override bool PreferNewTarget => true;

	public override void Trigger(Turret turret, Target target)
	{
		var debuff = new DamageOverTimeDebuff(turret.Name, Duration, TotalDamage, TickRate, ImpactParticle, turret.DamageType);
		target.ApplyDebuff(debuff);
	}

	public override void Tooltip(RichTextWithImages descriptionContainer, StyleSettings styleSettings, Turret turret)
	{
		var template = $"Deals {{Flat:{TotalDamage / Duration}:DamageType}} {{DamageType}} damage per second for {{Flat:{Duration}:DamageType}} seconds for a total of {{Flat:{TotalDamage}:DamageType}} {{DamageType}} damage.";
		descriptionContainer.Write(template, styleSettings, turret.DamageType, turret.ShopType);
	}
}
