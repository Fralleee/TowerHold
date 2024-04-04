using UnityEngine.UIElements;

public class TurretTooltipContent : TooltipContent
{
	protected Label TypeLabel;
	protected RichTextWithImages ShopTypeContainer;
	protected RichTextWithImages Cooldown;
	protected Label RangeLabel;

	public TurretTooltipContent() : base()
	{
		ShopTypeContainer = new RichTextWithImages();
		Cooldown = new RichTextWithImages();
		RangeLabel = new Label();

		Add(Cooldown);
		Add(RangeLabel);
		Add(ShopTypeContainer);

		ShopTypeContainer.AddToClassList("item-shoptype");
		Cooldown.AddToClassList("item-attack-cooldown");
		RangeLabel.AddToClassList("item-range");
	}

	public override void UpdateInformation(ShopItem item, StyleSettings styleSettings)
	{
		base.UpdateInformation(item, styleSettings);

		DescriptionContainer.Clear();

		if (item is Turret turret)
		{
			var (baseDamage, attackRange, timeBetweenAttacks, criticalHitChance, criticalHitMultiplier, description) = turret.GetHoverData();
			var currentDamage = Tower.Instance.GetDamage(turret.DamageType, turret.ShopType, baseDamage, 0, 0);

			var safeDescription = string.IsNullOrEmpty(description) ? "No description." : description;
			DescriptionContainer.Write(safeDescription, styleSettings, currentDamage, timeBetweenAttacks, 0, turret.DamageType, turret.ShopType);

			if (criticalHitChance > 0)
			{
				var template = "This ability has a {PercentDamageType} chance of a critical hit which causes {Damage} {DamageType} damage.";
				DescriptionContainer.Write(template, styleSettings, criticalHitMultiplier * currentDamage, 0, criticalHitChance, turret.DamageType, turret.ShopType);
			}

			var (isDamageOverTime, dotDuration, dotTotalDamage) = turret.GetDOTData();
			if (isDamageOverTime)
			{
				var template = "Deals {DPS} {DamageType} damage per second for {Duration} seconds for a total of {Damage} {DamageType} damage.";
				DescriptionContainer.Write(template, styleSettings, dotTotalDamage, dotDuration, 0, turret.DamageType, turret.ShopType);
			}

			ShopTypeContainer.AddImageLabel(styleSettings.GetShopTypeIcon(item.ShopType), item.ShopType.ToString(), styleSettings.GetShopTypeColor(item.ShopType));

			Cooldown.AddImageLabel(styleSettings.GetIcon(GameIcons.Cooldown), $"{timeBetweenAttacks:#.##}s");

			RangeLabel.text = attackRange.AsText();
		}
	}
}
