using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class ShadowContainer : Button
{
	readonly VisualElement _shadow;
	readonly Label _title;
	readonly Label _description;

	[UxmlAttribute]
	string Title
	{
		set => _title.text = value;
	}

	[UxmlAttribute]
	string Description
	{
		set => _description.text = value;
	}

	[UxmlAttribute]
	string Image
	{
		set => SetBackgroundImage(value);
	}

	[UxmlAttribute]
	bool Enabled
	{
		get => enabledSelf;
		set => SetEnabled(value);
	}

	public ShadowContainer()
	{
		_shadow = new VisualElement();
		_shadow.AddToClassList("shadow");
		Add(_shadow);

		_title = new Label();
		_title.AddToClassList("shadow-title");
		Add(_title);

		_description = new Label();
		_description.AddToClassList("shadow-description");
		Add(_description);
	}

	public void SetBackgroundImage(string imagePath)
	{
		var texture = Resources.Load<Texture2D>(imagePath);
		if (texture != null)
		{
			style.backgroundImage = new StyleBackground(texture);
		}
		else
		{
			Debug.LogWarning($"Failed to load texture at path: {imagePath}");
		}
	}
}
