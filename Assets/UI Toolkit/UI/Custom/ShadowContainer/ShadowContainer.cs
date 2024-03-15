using UnityEngine;
using UnityEngine.UIElements;

public class ShadowContainer : VisualElement
{
	readonly Button _button;
	readonly VisualElement _shadow;
	readonly Label _title;
	readonly Label _description;

	public new class UxmlFactory : UxmlFactory<ShadowContainer, UxmlTraits> { }

	public new class UxmlTraits : VisualElement.UxmlTraits
	{
		readonly UxmlStringAttributeDescription _name = new UxmlStringAttributeDescription { name = "name" };
		readonly UxmlStringAttributeDescription _title = new UxmlStringAttributeDescription { name = "title" };
		readonly UxmlStringAttributeDescription _description = new UxmlStringAttributeDescription { name = "description" };
		readonly UxmlStringAttributeDescription _image = new UxmlStringAttributeDescription { name = "image" };
		readonly UxmlStringAttributeDescription _class = new UxmlStringAttributeDescription { name = "buttonClass" };
		readonly UxmlStringAttributeDescription _disabled = new UxmlStringAttributeDescription { name = "disabled" };

		public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
		{
			base.Init(ve, bag, cc);

			var shadowContainer = ve as ShadowContainer;
			var name = _name.GetValueFromBag(bag, cc);
			shadowContainer.name = name;

			var shadowContainerButton = ve.Q<Button>();
			shadowContainerButton.name = $"{name}_Button";
			shadowContainerButton.AddToClassList(_class.GetValueFromBag(bag, cc));

			var isDisabled = _disabled.GetValueFromBag(bag, cc).Equals("true");
			shadowContainer.AddToClassList(isDisabled ? "disabled" : "enabled");
			shadowContainerButton.SetEnabled(!isDisabled);

			var image = _image.GetValueFromBag(bag, cc);
			shadowContainer.SetBackgroundImage(image);

			ve.Q<Label>(className: "shadow-title").text = _title.GetValueFromBag(bag, cc);
			ve.Q<Label>(className: "shadow-description").text = _description.GetValueFromBag(bag, cc);
		}
	}

	public ShadowContainer()
	{
		_button = new Button();
		Add(_button);

		_shadow = new VisualElement();
		_shadow.AddToClassList("shadow");
		_button.Add(_shadow);

		_title = new Label();
		_title.AddToClassList("shadow-title");
		_button.Add(_title);

		_description = new Label();
		_description.AddToClassList("shadow-description");
		_button.Add(_description);
	}

	public void SetBackgroundImage(string imagePath)
	{
		var texture = Resources.Load<Texture2D>(imagePath);
		if (texture != null)
		{
			_button.style.backgroundImage = new StyleBackground(texture);
		}
		else
		{
			Debug.LogWarning($"Failed to load texture at path: {imagePath}");
		}
	}
}
