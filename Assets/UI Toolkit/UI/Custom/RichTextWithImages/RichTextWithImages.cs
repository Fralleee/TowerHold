using System;
using UnityEngine;
using UnityEngine.UIElements;

public class RichTextWithImages : VisualElement
{
	const float PercentageThreshold = 5f;

	public void SetDescription(string description, float amount, DamageType? damageType, ShopType shopType, StyleSettings styleSettings)
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
				case "DamageType":
					if (damageType.HasValue)
					{
						AddDamageType(damageType.Value, styleSettings);
					}
					else
					{
						throw new ArgumentException("DamageType is null");
					}

					break;
				case "DamageAmount":
					if (damageType.HasValue)
					{
						AddDamageAmount(amount, damageType.Value, styleSettings);
					}
					else
					{
						throw new ArgumentException("DamageType is null");
					}
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
				default:
					AddText(part);
					break;
			}
		}
	}

	public void SetCriticalHitDescription(float damage, float criticalHitChance, float criticalHitMultiplier, DamageType damageType, StyleSettings styleSettings)
	{
		Clear();

		var critChanceString = (criticalHitChance * 100).ToString() + "%";
		var critDamage = criticalHitMultiplier * damage;
		Add(new VisualElement { style = { width = Length.Percent(100) } });
		AddText("This ability has a ");
		AddText(critChanceString, styleSettings.GetDamageTypeColor(damageType));
		AddText(" chance of a critical hit which causes ");
		AddDamageAmount(critDamage, damageType, styleSettings);
		AddText(" ");
		AddDamageType(damageType, styleSettings);
		AddText(" damage");
	}

	public void AddImageLabel(Texture2D texture, string text, Color? tint = null)
	{
		Clear();
		var image = new Image { image = texture, tintColor = tint ?? Color.white };
		Add(image);
		AddText(text, tint);
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

	void AddDamageAmount(float amount, DamageType damageType, StyleSettings styleSettings)
	{
		var amountText = amount < PercentageThreshold ? $"{amount * 100:0.##}%" : amount.ToString("0.##");
		var color = styleSettings.GetDamageTypeColor(damageType);
		AddText(amountText, color);
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
		Add(label);
	}

	void AddImage(Texture2D texture, Color tint)
	{
		var image = new Image { image = texture, tintColor = tint };
		Add(image);
	}
}
