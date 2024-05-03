using System;
using UnityEngine;
using UnityEngine.UIElements;

public class RichTextWithImages : VisualElement
{
	VisualElement _currentContainer;

	public void AddImageLabel(Texture2D texture, string text, Color? tint = null)
	{
		Clear();
		AddRow();

		var image = new Image { image = texture, tintColor = tint ?? Color.white };
		_currentContainer.Add(image);
		AddText(text, tint);
	}

	public void Write(string text, StyleSettings styleSettings, DamageType damageType, ShopType shopType)
	{
		AddRow();

		var parts = text.Split(new[] { '{', '}' }, StringSplitOptions.RemoveEmptyEntries);
		foreach (var part in parts)
		{
			var segments = part.Split(':');
			if (segments.Length == 1)
			{
				switch (part)
				{
					case "DamageTypeIcon":
						AddDamageType(damageType, styleSettings, true);
						break;
					case "ShopTypeIcon":
						AddShopType(shopType, styleSettings, true);
						break;
					case "DamageType":
						AddDamageType(damageType, styleSettings);
						break;
					case "ShopType":
						AddShopType(shopType, styleSettings);
						break;
					case "Newline":
						AddRow();
						break;
					default:
						AddText(part);
						break;
				}
				continue;
			}


			var formatType = segments[0];
			var value = float.Parse(segments[1]);
			var colorType = segments.Length > 2 ? segments[2] : null;
			Color? color = colorType != null ? GetColorByType(colorType, damageType, shopType, styleSettings) : null;
			switch (formatType)
			{
				case "Percent":
					AddFormattedText($"{value * 100:0.##}%", color);
					break;
				case "Flat":
					AddFormattedText($"{value:0.##}", color);
					break;
				default:
					AddText(part);
					break;
			}
		}
	}

	Color GetColorByType(string type, DamageType damageType, ShopType shopType, StyleSettings styleSettings)
	{
		return type switch
		{
			"ShopType" => styleSettings.GetShopTypeColor(shopType),
			"DamageType" => styleSettings.GetDamageTypeColor(damageType),
			_ => Color.white // Default color
		};
	}

	void AddRow()
	{
		_currentContainer = new VisualElement();
		_currentContainer.AddToClassList("row");
		Add(_currentContainer);
	}

	void AddFormattedText(string value, Color? color = null)
	{
		AddText($" <b>{value:0.##}</b> ", color);
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
			AddText($" <b>{damageType}</b>", color);
		}
	}

	void AddShopType(ShopType shopType, StyleSettings styleSettings, bool iconOnly = false)
	{
		var color = styleSettings.GetShopTypeColor(shopType);
		var icon = styleSettings.GetShopTypeIcon(shopType);
		if (icon != null)
		{
			AddImage(icon, color);
		}

		if (!iconOnly)
		{
			AddText($" <b>{shopType}</b>", color);
		}
	}

	void AddText(string text, Color? color = null)
	{
		text = text.Replace(" ", "\u00A0"); // Replace all normal spaces with non-breaking spaces
		var label = new Label()
		{
			enableRichText = true,
			text = text
		};
		if (color.HasValue)
		{
			label.style.color = color.Value;
		}
		_currentContainer.Add(label);
	}

	void AddImage(Texture2D texture, Color tint)
	{
		var image = new Image { image = texture, tintColor = tint };
		image.style.marginLeft = 2;
		_currentContainer.Add(image);
	}
}
