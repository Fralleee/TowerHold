using UnityEngine.UIElements;

public class ShadowButton : VisualElement
{
	readonly Button _button;
	readonly Label _label;

	public new class UxmlFactory : UxmlFactory<ShadowButton, UxmlTraits> { }

	public new class UxmlTraits : VisualElement.UxmlTraits
	{
		readonly UxmlStringAttributeDescription _text = new UxmlStringAttributeDescription { name = "buttonText" };
		readonly UxmlStringAttributeDescription _class = new UxmlStringAttributeDescription { name = "buttonClass" };
		readonly UxmlStringAttributeDescription _name = new UxmlStringAttributeDescription { name = "name" };

		public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
		{
			base.Init(ve, bag, cc);

			var shadowButton = ve as ShadowButton;
			var name = _name.GetValueFromBag(bag, cc);
			shadowButton.name = name;

			var shadowButtonButton = ve.Q<Button>();
			shadowButtonButton.name = $"{name}_Button";
			shadowButtonButton.AddToClassList(_class.GetValueFromBag(bag, cc));

			shadowButtonButton.Q<Label>().text = _text.GetValueFromBag(bag, cc);
		}
	}

	public ShadowButton()
	{
		_button = new Button();
		Add(_button);

		var shadowElement = new VisualElement();
		shadowElement.AddToClassList("shadow");
		_button.Add(shadowElement);

		_label = new Label();
		_button.Add(_label);
	}
}
