using System;
using UnityEngine.UIElements;

public class TooltipContent : VisualElement
{
	public Action<TooltipContent> OnUpdate = delegate { };

	public Label NameLabel;
	public Label CostLabel;
	public Label DescriptionLabel;

	public TooltipContent(string name = null, string cost = null, string description = null)
	{
		NameLabel = new Label();
		CostLabel = new Label();
		DescriptionLabel = new Label();

		Add(NameLabel);
		Add(CostLabel);
		Add(DescriptionLabel);

		AddToClassList("tooltip-data");
		NameLabel.AddToClassList("item-name");
		CostLabel.AddToClassList("item-cost");
		DescriptionLabel.AddToClassList("item-description");

		if (name != null)
		{
			NameLabel.text = name;
		}

		if (cost != null)
		{
			CostLabel.text = cost;
		}

		if (description != null)
		{
			DescriptionLabel.text = description;
		}
	}

	public virtual void UpdateInformation(ShopItem item)
	{
		NameLabel.text = item.name;
		CostLabel.text = item.Cost.ToString();
		DescriptionLabel.text = item.ShopType.ToString();

		NameLabel.AddToClassList($"text-{item.RarityType.AsColorClass()}");
	}

	public virtual void Update(TooltipContent tooltipContent)
	{
		OnUpdate(tooltipContent);
	}
}
