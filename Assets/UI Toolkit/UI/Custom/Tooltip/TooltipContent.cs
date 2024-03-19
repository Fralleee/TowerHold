using System;
using UnityEngine;
using UnityEngine.UIElements;

public class TooltipContent : VisualElement
{
	public Action<TooltipContent> OnUpdate = delegate { };

	public VisualElement InformationContainer;
	public VisualElement TextContainer;
	public Image Image;
	public Label NameLabel;
	public RichTextWithImages CostContainer;
	public RichTextWithImages DescriptionContainer;

	public TooltipContent(Texture2D texture2D = null, string name = null, string cost = null, string description = null)
	{
		InformationContainer = new VisualElement();
		TextContainer = new VisualElement();
		Image = new Image();
		NameLabel = new Label();
		CostContainer = new RichTextWithImages();
		DescriptionContainer = new RichTextWithImages();

		Add(InformationContainer);
		InformationContainer.Add(Image);
		InformationContainer.Add(TextContainer);
		TextContainer.Add(NameLabel);
		TextContainer.Add(CostContainer);
		Add(DescriptionContainer);

		AddToClassList("data");
		Image.AddToClassList("item-image");
		InformationContainer.AddToClassList("item-information");
		TextContainer.AddToClassList("item-texts");
		NameLabel.AddToClassList("item-name");
		CostContainer.AddToClassList("item-cost");
		DescriptionContainer.AddToClassList("item-description");

		if (texture2D != null)
		{
			Image.image = texture2D;
		}

		if (name != null)
		{
			NameLabel.text = name;
		}

		if (cost != null)
		{
			CostContainer.Add(new Label(cost));
		}

		if (description != null)
		{
			DescriptionContainer.Add(new Label(description));
		}
	}

	public virtual void UpdateInformation(ShopItem item, StyleSettings styleSettings)
	{
		Image.image = item.Texture;

		NameLabel.text = item.Name;
		NameLabel.style.color = styleSettings.GetRarityColor(item.RarityType);

		CostContainer.AddImageLabel(styleSettings.GetIcon(GameIcons.Gold), item.Cost.ToString(), styleSettings.GetShopTypeColor(ShopType.Income));

		var safeDescription = string.IsNullOrEmpty(item.Description) ? "No description available" : item.Description;
		DescriptionContainer.SetDescription(safeDescription);
	}

	public virtual void Update(TooltipContent tooltipContent)
	{
		OnUpdate(tooltipContent);
	}
}
