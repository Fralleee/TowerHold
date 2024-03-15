using UnityEngine;
using UnityEngine.UIElements;

public class ShadowContainer : Button
{
	readonly VisualElement _shadow;
	readonly Label _title;
	readonly Label _description;

	public bool Enabled
	{
		get
		{
			return enabledSelf;
		}

		set
		{
			SetEnabled(value);
		}
	}

	public new class UxmlFactory : UxmlFactory<ShadowContainer, UxmlTraits> { }

	public new class UxmlTraits : VisualElement.UxmlTraits
	{
		readonly UxmlStringAttributeDescription _title = new UxmlStringAttributeDescription { name = "title" };
		readonly UxmlStringAttributeDescription _description = new UxmlStringAttributeDescription { name = "description" };
		readonly UxmlStringAttributeDescription _image = new UxmlStringAttributeDescription { name = "image" };
		readonly UxmlBoolAttributeDescription _enabled = new UxmlBoolAttributeDescription { name = "enabled", defaultValue = true };

		public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
		{
			base.Init(ve, bag, cc);

			var instance = ve as ShadowContainer;
			var image = _image.GetValueFromBag(bag, cc);
			instance.SetBackgroundImage(image);
			instance.Enabled = _enabled.GetValueFromBag(bag, cc);

			ve.Q<Label>(className: "shadow-title").text = _title.GetValueFromBag(bag, cc);
			ve.Q<Label>(className: "shadow-description").text = _description.GetValueFromBag(bag, cc);
		}
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
