using UnityEngine.UIElements;

[UxmlElement]
public partial class ShadowButton : Button
{
	[UxmlAttribute]
	string Text
	{
		set
		{
			_label.text = value;
		}
	}

	[UxmlAttribute]
	bool Enabled
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

	readonly Label _label;

	public ShadowButton()
	{
		var shadowElement = new VisualElement();
		shadowElement.AddToClassList("shadow");
		Add(shadowElement);

		_label = new Label();
		Add(_label);
	}
}
