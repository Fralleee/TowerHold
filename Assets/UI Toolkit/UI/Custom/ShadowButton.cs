using UnityEngine.UIElements;

public class ShadowButton : VisualElement
{
	readonly Button _button;

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
			shadowButtonButton.text = _text.GetValueFromBag(bag, cc);
			shadowButtonButton.AddToClassList(_class.GetValueFromBag(bag, cc));
		}
	}

	public ShadowButton()
	{
		_button = new Button();
		Add(_button);
	}
}
