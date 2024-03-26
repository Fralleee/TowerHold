using UnityEngine.UIElements;

public class TurretTooltipContent : TooltipContent
{
	protected Label TypeLabel;
	protected RichTextWithImages Cooldown;
	protected Label RangeLabel;

	public TurretTooltipContent() : base()
	{
		Cooldown = new RichTextWithImages();
		RangeLabel = new Label();

		Add(Cooldown);
		Add(RangeLabel);

		Cooldown.AddToClassList("item-attack-cooldown");
		RangeLabel.AddToClassList("item-range");
	}

	public override void UpdateInformation(ShopItem item, StyleSettings styleSettings)
	{
		base.UpdateInformation(item, styleSettings);

		if (item is Turret turret)
		{
			var (baseDamage, attackRange, timeBetweenAttacks, description) = turret.GetHoverData();
			var currentDamage = Tower.Instance.GetDamage(turret.Category, baseDamage);

			var safeDescription = string.IsNullOrEmpty(description) ? "No description." : description;
			DescriptionContainer.SetDescription(safeDescription, currentDamage, turret.ShopType, styleSettings);

			Cooldown.AddImageLabel(styleSettings.GetIcon(GameIcons.Cooldown), $"{timeBetweenAttacks:#.##}s");


			RangeLabel.text = attackRange.AsText();
		}
	}
}
