using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopSlot : MonoBehaviour
{
	public TextMeshProUGUI ItemNameText;
	public TextMeshProUGUI CostText;
	public Image ItemImage;
	public Image CategoryImage;
	public Button PurchaseButton;

	ShopItem _item;
	Button _button;

	void Awake() => _button = GetComponent<Button>();

	public void SetupSlot(ShopItem newItem)
	{
		SetUIAlpha(1f);

		_item = newItem;
		ItemNameText.text = _item.ItemName;
		ItemImage.sprite = _item.Image;

		if (UIManager.Instance.CategorySprites.TryGetValue(_item.Category, out var sprite))
		{
			CategoryImage.sprite = sprite;
		}
		else
		{
			Debug.LogError("No sprite found for category: " + _item.Category);
		}

		if (UIManager.Instance.CategoryColors.TryGetValue(_item.Category, out var color))
		{
			CategoryImage.color = color;
			ItemImage.color = color;

			_button.colors = new ColorBlock()
			{
				normalColor = new Color(color.r, color.g, color.b, 0.25f),
				highlightedColor = new Color(color.r, color.g, color.b, 0.85f),
				pressedColor = new Color(color.r, color.g, color.b, 0.5f),
				disabledColor = new Color(color.r, color.g, color.b, 0.1f),
				colorMultiplier = 1,
				fadeDuration = 0.1f
			};
		}
		else
		{
			Debug.LogError("No color found for category: " + _item.Category);
		}

		CostText.text = _item.Cost.ToString();
		PurchaseButton.interactable = true;
		foreach (Transform child in transform)
		{
			child.gameObject.SetActive(true);
		}
	}

	void SetUIAlpha(float alpha)
	{
		ItemNameText.alpha = alpha;
		CostText.alpha = alpha;
		ItemImage.color = new Color(ItemImage.color.r, ItemImage.color.g, ItemImage.color.b, alpha);
		CategoryImage.color = new Color(CategoryImage.color.r, CategoryImage.color.g, CategoryImage.color.b, alpha);
	}

	void DisableButton()
	{
		PurchaseButton.interactable = false;
		SetUIAlpha(0.1f);
	}

	public void PurchaseItem()
	{
		if (GoldManager.Instance.SpendGold(_item.Cost))
		{
			_item.OnPurchase();
			DisableButton();
		}
	}
}
