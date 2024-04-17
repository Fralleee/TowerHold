using System.Diagnostics;
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
			var parsedDescription = safeDescription.Replace("#Damage#", $"{{Flat:{currentDamage}:DamageType}} {{DamageType}}");
			DescriptionContainer.Write(parsedDescription, styleSettings, turret.DamageType, turret.ShopType);

			turret.Behaviors.ForEach(behavior => behavior.Tooltip(DescriptionContainer, styleSettings, turret));

			if (criticalHitChance > 0)
			{
				var template = $"This ability has a {{Percent:{criticalHitChance}:ShopType}} chance of a critical hit which causes {{Flat:{currentDamage * criticalHitMultiplier}:DamageType}} {{DamageType}} damage.";
				DescriptionContainer.Write(template, styleSettings, turret.DamageType, turret.ShopType);
			}

			ShopTypeContainer.AddImageLabel(styleSettings.GetShopTypeIcon(item.ShopType), item.ShopType.ToString(), styleSettings.GetShopTypeColor(item.ShopType));

			Cooldown.AddImageLabel(styleSettings.GetIcon(GameIcons.Cooldown), $"{timeBetweenAttacks:#.##}s");

			RangeLabel.text = attackRange.AsText();
		}
	}
}
