using System;
using UnityEngine;
using UnityEngine.UIElements;

public class RichTextWithImages : VisualElement
{
	public void SetTurretDescription(string description, float damage, ShopType shopType, StyleSettings styleSettings)
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
				case "Damage":
					AddDamageDescription(damage, shopType, styleSettings);
					break;
				default:
					AddText(part);
					break;
			}
		}
	}

	public void SetDescription(string description)
	{
		Clear();
		AddText(description);
	}

	public void AddImageLabel(Texture2D texture, string text, Color? tint = null)
	{
		Clear();
		var image = new Image { image = texture, tintColor = tint ?? Color.white };
		Add(image);
		Add(new Label(text));
	}

	void AddDamageDescription(float damage, ShopType shopType, StyleSettings styleSettings)
	{
		var color = styleSettings.GetShopTypeColor(shopType);
		AddText($"{damage} ", color);

		var icon = styleSettings.GetShopTypeIcon(shopType);
		if (icon != null)
		{
			AddImage(icon, color);
		}

		AddText($" {shopType} damage", color);
	}

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
