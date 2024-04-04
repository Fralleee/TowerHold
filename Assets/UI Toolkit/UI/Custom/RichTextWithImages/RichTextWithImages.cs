using System;
using UnityEngine;
using UnityEngine.UIElements;

public class RichTextWithImages : VisualElement
{
	const float PercentageThreshold = 5f;

	VisualElement _currentContainer;

	public void AddImageLabel(Texture2D texture, string text, Color? tint = null)
	{
		Clear();
		AddRow();

		var image = new Image { image = texture, tintColor = tint ?? Color.white };
		_currentContainer.Add(image);
		AddText(text, tint);
	}

	public void Write(string text, StyleSettings styleSettings, float amount, float duration, float chance, DamageType damageType, ShopType shopType)
	{
		AddRow();

		var parts = text.Split(new[] { '{', '}' }, StringSplitOptions.RemoveEmptyEntries);
		foreach (var part in parts)
		{
			switch (part)
			{
				case "Chance":
					AddChance(chance, damageType, styleSettings);
					break;
				case "Damage":
					AddDamage(amount, damageType, styleSettings);
					break;
				case "DamageType":
					AddDamageType(damageType, styleSettings);
					break;
				case "DPS":
					var damagePerSecond = amount / duration;
					AddDamage(damagePerSecond, damageType, styleSettings);
					break;
				case "Duration":
					AddDuration(duration, damageType, styleSettings);
					break;
				case "Type":
					AddType(shopType, styleSettings);
					break;
				case "TypeIcon":
					AddType(shopType, styleSettings, true);
					break;
				case "Amount":
					AddAmount(amount, shopType, styleSettings);
					break;
				case "Newline":
					AddRow();
					break;
				default:
					AddText(part);
					break;
			}
		}
	}

	void AddRow()
	{
		_currentContainer = new VisualElement();
		_currentContainer.AddToClassList("row");
		Add(_currentContainer);
	}

	void AddChance(float chance, DamageType damageType, StyleSettings styleSettings)
	{
		var color = styleSettings.GetDamageTypeColor(damageType);
		AddText($"{chance * 100:0.##}%", color);
	}

	void AddDamageType(DamageType damageType, StyleSettings styleSettings, bool iconOnly = false)
	{
		var color = styleSettings.GetDamageTypeColor(damageType);
		var icon = styleSettings.GetDamageTypeIcon(damageType);
		if (icon != null)
		{
			AddImage(icon, color);
		}

		if (!iconOnly)
		{
			AddText($" {damageType}", color);
		}
	}

	void AddDamage(float amount, DamageType damageType, StyleSettings styleSettings)
	{
		var amountText = amount < PercentageThreshold ? $"{amount * 100:0.##}%" : amount.ToString("0.##");
		var color = styleSettings.GetDamageTypeColor(damageType);
		AddText(amountText, color);
	}

	void AddDuration(float duration, DamageType damageType, StyleSettings styleSettings)
	{
		var color = styleSettings.GetDamageTypeColor(damageType);
		AddText(duration.ToString("0.##"), color);
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
			AddText($" {shopType}", color);
		}
	}

	void AddAmount(float amount, ShopType shopType, StyleSettings styleSettings)
	{
		var amountText = amount < PercentageThreshold ? $"{amount * 100:0.##}%" : amount.ToString("0.##");
		var color = styleSettings.GetShopTypeColor(shopType);
		AddText(amountText, color);
	}

	void AddText(string text, Color? color = null)
	{
		var label = new Label(text);
		if (color.HasValue)
		{
			label.style.color = color.Value;
		}
		_currentContainer.Add(label);
	}

	void AddImage(Texture2D texture, Color tint)
	{
		var image = new Image { image = texture, tintColor = tint };
		_currentContainer.Add(image);
	}
}
