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

		if (item is Turret turret)
		{
			var (baseDamage, attackRange, timeBetweenAttacks, criticalHitChance, criticalHitMultiplier, description) = turret.GetHoverData();
			var currentDamage = Tower.Instance.GetDamage(turret.DamageType, turret.ShopType, baseDamage, 0, 0);

			var safeDescription = string.IsNullOrEmpty(description) ? "No description." : description;
			DescriptionContainer.SetDescription(safeDescription, currentDamage, turret.DamageType, turret.ShopType, styleSettings);

			if (criticalHitChance > 0)
			{
				CritContainer.SetCriticalHitDescription(currentDamage, criticalHitChance, criticalHitMultiplier, turret.DamageType, styleSettings);
			}

			ShopTypeContainer.AddImageLabel(styleSettings.GetShopTypeIcon(item.ShopType), item.ShopType.ToString(), styleSettings.GetShopTypeColor(item.ShopType));

			Cooldown.AddImageLabel(styleSettings.GetIcon(GameIcons.Cooldown), $"{timeBetweenAttacks:#.##}s");

			RangeLabel.text = attackRange.AsText();
		}
	}
}
