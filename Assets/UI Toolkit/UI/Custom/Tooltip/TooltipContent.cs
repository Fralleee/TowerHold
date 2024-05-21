using UnityEngine;
using UnityEngine.UIElements;

public class TooltipContent : VisualElement
{
	public VisualElement InformationContainer;
	public VisualElement TextContainer;
	public Image Image;
	public Label NameLabel;
	public RichTextWithImages CostContainer;
	public RichTextWithImages DescriptionContainer;
	public Color BorderColor;

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

	public virtual void UpdateInformation(ShopItem item)
	{
		Image.image = item.Texture;

		NameLabel.text = item.Name;
		NameLabel.style.color = StyleManager.Styles.GetRarityColor(item.RarityType);

		BorderColor = StyleManager.Styles.GetRarityColor(item.RarityType);

		CostContainer.AddImageLabel(StyleManager.Styles.GetIcon(GameIcons.Gold), item.Cost.ToString(), StyleManager.Styles.GetShopTypeColor(ShopType.Income));

		var safeDescription = string.IsNullOrEmpty(item.Description) ? "No description." : item.Description;
		var parsedDescription = safeDescription.Replace("#Amount#", item.Amount.ToString());
		var damageType = item is DamageShopItem damageShopItem ? damageShopItem.DamageType : DamageType.Global;
		DescriptionContainer.Write(parsedDescription, damageType, item.ShopType);
	}
}
