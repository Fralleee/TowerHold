using UnityEngine.UIElements;

[UxmlElement]
public partial class ShopItemButton : Button
{
	public VisualElement Overlay;
	public VisualElement Border;
	public Label Hotkey;
	public VisualElement Content;
	public VisualElement Shadow;
	public VisualElement Image;
	public VisualElement Sparkling;
	public VisualElement TypeContainer;
	public VisualElement Category;

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

		Overlay = new VisualElement();
		Overlay.AddToClassList("overlay");
		Add(Overlay);
	}

	public void Setup(ShopItem item)
	{
		var rarityColor = StyleManager.Styles.GetRarityColor(item.RarityType);

		Image.style.backgroundImage = item.Texture;
		Border.style.backgroundColor = rarityColor;

		var shopTypeImage = StyleManager.Styles.GetShopTypeIcon(item.ShopType);
		Category.style.backgroundImage = shopTypeImage;
		var shopTypeColor = StyleManager.Styles.GetShopTypeColor(item.ShopType);
		Category.style.unityBackgroundImageTintColor = shopTypeColor;


		SetEnabled(true);
	}

	public void Disable()
	{
		SetEnabled(false);
		Overlay.style.display = DisplayStyle.None;
	}

	public void SetCanBePurchased(bool canBePurchased)
	{
		if (enabledSelf)
		{
			Overlay.style.display = DisplayStyle.Flex;
			Overlay.style.opacity = canBePurchased ? 0 : 1;
		}
	}

	public void SetHotkey(string hotkey)
	{
		Hotkey.text = hotkey;
	}
}
