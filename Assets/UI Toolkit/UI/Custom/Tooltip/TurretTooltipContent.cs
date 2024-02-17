using UnityEngine.UIElements;

public class TurretTooltipContent : TooltipContent
{
	protected VisualElement Divider;
	protected Label TypeLabel;
	protected Label DamageTypeLabel;
	protected Label DamageLabel;
	protected Label DpsLabel;
	protected Label AttackCooldownLabel;
	protected Label RangeLabel;

	public TurretTooltipContent() : base()
	{
		Divider = new VisualElement();
		DamageTypeLabel = new Label();
		DamageLabel = new Label();
		DpsLabel = new Label();
		AttackCooldownLabel = new Label();
		RangeLabel = new Label();

		Add(Divider);
		Add(DamageTypeLabel);
		Add(DamageLabel);
		Add(DpsLabel);
		Add(AttackCooldownLabel);
		Add(RangeLabel);

		AddToClassList("turret-tooltip-data");
		Divider.AddToClassList("item-divider");
		DamageTypeLabel.AddToClassList("item-damage-type");
		DamageLabel.AddToClassList("item-damage");
		DpsLabel.AddToClassList("item-dps");
		AttackCooldownLabel.AddToClassList("item-attack-cooldown");
		RangeLabel.AddToClassList("item-range");
	}

	public override void UpdateInformation(ShopItem item)
	{
		base.UpdateInformation(item);

		if (item is Turret turret)
		{
			var (baseDamage, attackRange, timeBetweenAttacks) = turret.GetHoverData();
			DescriptionLabel.text = "Weapon";
			DamageTypeLabel.text = $"DamageType: {turret.ShopType}";
			DamageLabel.text = $"Damage: {baseDamage}";
			DpsLabel.text = $"DPS: {baseDamage / timeBetweenAttacks}";
			AttackCooldownLabel.text = $"Attack Cooldown: {timeBetweenAttacks}";
			RangeLabel.text = $"Range: {attackRange}";
		}
	}
}
