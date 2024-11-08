using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class CustomProgressBar : VisualElement
{
	readonly VisualElement _container;
	readonly VisualElement _progress;
	readonly VisualElement _change;
	readonly VisualElement _icon;
	readonly Label _label;

	public bool UseChangeBar
	{
		set
		{
			_change.style.display = value ? DisplayStyle.Flex : DisplayStyle.None;
		}
	}

	float _value;
	[UxmlAttribute]
	public float Value
	{
		get => _value;
		set
		{
			_value = Mathf.Clamp01(value);
			var widthPercentage = _value * 100;
			_progress.style.width = new StyleLength(Length.Percent(widthPercentage));
			_change.style.width = new StyleLength(Length.Percent(widthPercentage));
		}
	}
	public (int, int) MinMaxValue
	{
		set
		{
			_label.text = $"{value.Item1} / {value.Item2}";
		}
	}

	public CustomProgressBar()
	{
		AddToClassList("ProgressBar");

		_container = new VisualElement();
		_container.AddToClassList("ProgressBar__container");
		Add(_container);

		_change = new VisualElement();
		_change.AddToClassList("ProgressBar__progress-change");
		_container.Add(_change);

		_progress = new VisualElement();
		_progress.AddToClassList("ProgressBar__progress");
		_container.Add(_progress);

		_label = new Label();
		_label.AddToClassList("ProgressBar__label");
		_container.Add(_label);

		_icon = new VisualElement();
		_icon.AddToClassList("ProgressBar__icon");
		_container.Add(_icon);
	}
}
