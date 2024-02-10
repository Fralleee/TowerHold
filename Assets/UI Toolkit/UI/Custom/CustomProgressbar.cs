using UnityEngine;
using UnityEngine.UIElements;

public class CustomProgressBar : VisualElement
{
	readonly VisualElement _container;
	readonly VisualElement _progress;
	readonly VisualElement _change;

	readonly Label _label;

	public new class UxmlFactory : UxmlFactory<CustomProgressBar, UxmlTraits> { }

	public new class UxmlTraits : VisualElement.UxmlTraits
	{
		readonly UxmlFloatAttributeDescription _value = new UxmlFloatAttributeDescription { name = "value", defaultValue = 0 };

		public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
		{
			base.Init(ve, bag, cc);
			((CustomProgressBar)ve).Value = _value.GetValueFromBag(bag, cc);
		}
	}

	public bool UseChangeBar
	{
		set
		{
			_change.style.display = value ? DisplayStyle.Flex : DisplayStyle.None;
		}
	}

	float _value;
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
	}
}
