using System;
using UnityEngine;
using UnityEngine.UIElements;

public class RichTextWithImages : VisualElement
{
	const float PercentageThreshold = 5f;

	public void SetDescription(string description, float amount, ShopType shopType, StyleSettings styleSettings)
	{
		Clear();

		if (string.IsNullOrEmpty(description))
		{
			AddText("No description available");
			return;
		}

		var parts = description.Split(new[] { '{', '}' }, StringSplitOptions.RemoveEmptyEntries);
		foreach (var part in parts)
		{
			switch (part)
			{
				case "Type":
					AddType(shopType, styleSettings);
					break;
				case "TypeIcon":
					AddType(shopType, styleSettings, true);
					break;
				case "Amount":
					AddAmount(amount, shopType, styleSettings);
					break;
				// case "AmountDamage":
				// 	AddAmount(amount, true, shopType, styleSettings);
				// 	break;
				default:
					AddText(part);
					break;
			}
		}
	}

	public void AddImageLabel(Texture2D texture, string text, Color? tint = null)
	{
		Clear();
		var image = new Image { image = texture, tintColor = tint ?? Color.white };
		Add(image);
		Add(new Label(text));
	}

	void AddType(ShopType shopType, StyleSettings styleSettings, bool iconOnly = false)
	{
		var color = styleSettings.GetShopTypeColor(shopType);
		var icon = styleSettings.GetShopTypeIcon(shopType);
		if (icon != null)
		{
			AddImage(icon, color);
		}

		if (!iconOnly)
		{
			AddText(shopType.ToString());
		}
	}

	void AddAmount(float amount, ShopType shopType, StyleSettings styleSettings)
	{
		var amountText = amount < PercentageThreshold ? $"{amount * 100:0.##}%" : amount.ToString("0.##");
		var color = styleSettings.GetShopTypeColor(shopType);
		AddText($"{amountText} ", color);
	}

	// void AddAmount(float amount, bool asDamageText, ShopType shopType, StyleSettings styleSettings)
	// {
	// 	var amountText = amount < PercentageThreshold ? $"{amount * 100:0.##}%" : amount.ToString("0.##");
	// 	var color = styleSettings.GetShopTypeColor(shopType);
	// 	AddText($"{amountText} ", color);

	// 	var icon = styleSettings.GetShopTypeIcon(shopType);
	// 	if (icon != null)
	// 	{
	// 		AddImage(icon, color);
	// 	}

	// 	if (asDamageText)
	// 	{
	// 		AddText($"{shopType} damage", color);
	// 	}
	// }

	void AddText(string text, Color? color = null)
	{
		var label = new Label(text);
		if (color.HasValue)
		{
			label.style.color = color.Value;
		}
		Add(label);
	}

	void AddImage(Texture2D texture, Color tint)
	{
		var image = new Image { image = texture, tintColor = tint };
		Add(image);
	}
}
