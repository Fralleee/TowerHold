using UnityEngine;
using UnityEngine.UIElements;

public class ShopItemButton : Button
{
	public VisualElement Border;
	public Label Hotkey;
	public VisualElement Content;
	public VisualElement Shadow;
	public VisualElement Image;
	public VisualElement Sparkling;
	public VisualElement TypeContainer;
	public VisualElement Category;

	public new class UxmlFactory : UxmlFactory<ShopItemButton, UxmlTraits> { }

	public ShopItemButton()
	{
		// Assign class names and structure based on your XML layout
		AddToClassList("shop-item-button");

		Border = new VisualElement();
		Border.AddToClassList("border");
		Add(Border);

		Hotkey = new Label();
		Hotkey.AddToClassList("hotkey");
		Hotkey.text = "1";
		Add(Hotkey);

		Content = new VisualElement();
		Content.AddToClassList("content");
		Add(Content);

		Shadow = new VisualElement();
		Shadow.AddToClassList("shadow");
		Content.Add(Shadow);

		Sparkling = new VisualElement();
		Sparkling.AddToClassList("sparkling");
		Content.Add(Sparkling);

		Image = new VisualElement();
		Image.AddToClassList("image");
		Content.Add(Image);

		TypeContainer = new VisualElement();
		TypeContainer.AddToClassList("type-container");
		Content.Add(TypeContainer);

		Category = new VisualElement();
		Category.AddToClassList("category");
		TypeContainer.Add(Category);
	}

	public void Setup(ShopItem item, StyleSettings styleSettings)
	{
		var rarityColor = styleSettings.GetRarityColor(item.RarityType);
		var rarityTintColor = styleSettings.GetRarityTintColor(item.RarityType);
		var shopTypeColor = styleSettings.GetShopTypeColor(item.ShopType);
		var shopTypeImage = styleSettings.GetShopTypeIcon(item.ShopType);

		Image.style.backgroundImage = item.Texture;
		Image.style.unityBackgroundImageTintColor = rarityTintColor;
		Border.style.backgroundColor = rarityColor;

		Category.style.backgroundImage = shopTypeImage;
		Category.style.unityBackgroundImageTintColor = shopTypeColor;

		SetEnabled(true);
	}

	public void SetHotkey(string hotkey)
	{
		Hotkey.text = hotkey;
	}
}
