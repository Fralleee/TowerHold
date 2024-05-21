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

	public override void UpdateInformation(ShopItem item)
	{
		base.UpdateInformation(item);

		DescriptionContainer.Clear();

		if (item is Turret turret)
		{
			var currentDamage = Tower.Instance.GetDamage(turret, true);

			var safeDescription = string.IsNullOrEmpty(turret.Description) ? "No description." : turret.Description;
			var parsedDescription = safeDescription.Replace("#Damage#", $"{{Flat:{currentDamage}:DamageType}} {{DamageType}}");
			DescriptionContainer.Write(parsedDescription, turret.DamageType, turret.ShopType);

			turret.AfflictionsController.Tooltip(DescriptionContainer, turret);

			if (turret.CriticalHitChance > 0)
			{
				var template = $"This ability has a {{Percent:{turret.CriticalHitChance}:ShopType}} chance of a critical hit which causes {{Flat:{currentDamage * turret.CriticalHitMultiplier}:DamageType}} {{DamageType}} damage.";
				DescriptionContainer.Write(template, turret.DamageType, turret.ShopType);
			}

			ShopTypeContainer.AddImageLabel(StyleManager.Styles.GetShopTypeIcon(item.ShopType), item.ShopType.ToString(), StyleManager.Styles.GetShopTypeColor(item.ShopType));

			Cooldown.AddImageLabel(StyleManager.Styles.GetIcon(GameIcons.Cooldown), $"{turret.TimeBetweenAttacks:#.##}s");

			RangeLabel.text = turret.AttackRange.AsText();
		}
	}
}
