using UnityEngine.UIElements;

public class ShadowButton : Button
{
	readonly Label _label;

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

	public new class UxmlFactory : UxmlFactory<ShadowButton, UxmlTraits> { }

	public new class UxmlTraits : VisualElement.UxmlTraits
	{
		readonly UxmlStringAttributeDescription _text = new UxmlStringAttributeDescription { name = "buttonText" };
		readonly UxmlBoolAttributeDescription _enabled = new UxmlBoolAttributeDescription { name = "enabled", defaultValue = true };

		public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
		{
			base.Init(ve, bag, cc);

			var instance = ve as ShadowButton;
			instance.Enabled = _enabled.GetValueFromBag(bag, cc);
			instance.Q<Label>().text = _text.GetValueFromBag(bag, cc);
		}
	}

	public ShadowButton()
	{
		var shadowElement = new VisualElement();
		shadowElement.AddToClassList("shadow");
		Add(shadowElement);

		_label = new Label();
		Add(_label);
	}
}
