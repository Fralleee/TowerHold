using UnityEngine;
using UnityEngine.UIElements;

public class CustomProgressBar : VisualElement
{
	readonly VisualElement _progressBar;
	readonly VisualElement _progressBarContainer;
	readonly VisualElement _progressBarChange;

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
			_progressBarChange.style.display = value ? DisplayStyle.Flex : DisplayStyle.None;
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
			_progressBar.style.width = new StyleLength(Length.Percent(widthPercentage));
			_progressBarChange.style.width = new StyleLength(Length.Percent(widthPercentage));
		}
	}

	public CustomProgressBar()
	{
		AddToClassList("ProgressBar");

		_progressBarContainer = new VisualElement();
		_progressBarContainer.AddToClassList("ProgressBar__container");
		Add(_progressBarContainer);

		_progressBarChange = new VisualElement();
		_progressBarChange.AddToClassList("ProgressBar__progress-change");
		_progressBarContainer.Add(_progressBarChange);

		_progressBar = new VisualElement();
		_progressBar.AddToClassList("ProgressBar__progress");
		_progressBarContainer.Add(_progressBar);
	}
}
