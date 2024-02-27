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
		DamageLabel = new Label();
		AttackCooldownLabel = new Label();
		RangeLabel = new Label();

		Add(Divider);
		Add(DamageLabel);
		Add(AttackCooldownLabel);
		Add(RangeLabel);

		AddToClassList("turret-tooltip-data");
		Divider.AddToClassList("item-divider");
		DamageLabel.AddToClassList("item-damage");
		AttackCooldownLabel.AddToClassList("item-attack-cooldown");
		RangeLabel.AddToClassList("item-range");
	}

	public override void UpdateInformation(ShopItem item, StyleSettings styleSettings)
	{
		base.UpdateInformation(item, styleSettings);

		if (item is Turret turret)
		{
			var (baseDamage, attackRange, timeBetweenAttacks) = turret.GetHoverData();

			var currentDamage = Tower.Instance.GetDamage(turret.Category, baseDamage);

			DescriptionLabel.text = $"DamageType: {turret.ShopType}";
			DamageLabel.text = $"Damage: {currentDamage} ({currentDamage / timeBetweenAttacks} DPS)";
			AttackCooldownLabel.text = $"Attack Cooldown: {timeBetweenAttacks}";
			RangeLabel.text = $"Range: {attackRange}";
		}
	}
}
