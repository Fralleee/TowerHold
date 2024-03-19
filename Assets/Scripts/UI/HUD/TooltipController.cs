using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class TooltipController : MonoBehaviour
{
	Tooltip _tooltip;
	VisualElement _activeElement;

	readonly List<(VisualElement element, EventCallback<MouseEnterEvent> enterCallback, EventCallback<MouseLeaveEvent> leaveCallback)> _eventRegistrations = new List<(VisualElement, EventCallback<MouseEnterEvent>, EventCallback<MouseLeaveEvent>)>();

	readonly Dictionary<VisualElement, TooltipContent> _tooltipContents = new Dictionary<VisualElement, TooltipContent>();

	void Awake()
	{
		var uiDocument = GetComponent<UIDocument>();
		_tooltip = uiDocument.rootVisualElement.Q<Tooltip>();
	}

	public void RegisterTooltip(VisualElement element, TooltipContent tooltipContent)
	{
		_tooltipContents[element] = tooltipContent;
		var enterCallback = new EventCallback<MouseEnterEvent>(evt => ShowTooltip(element));
		var leaveCallback = new EventCallback<MouseLeaveEvent>(evt => HideTooltip());

		element.RegisterCallback(enterCallback);
		element.RegisterCallback(leaveCallback);

		_eventRegistrations.Add((element, enterCallback, leaveCallback));
	}

	public void UpdateTooltip(VisualElement element, TooltipContent newContent)
	{
		_tooltipContents[element] = newContent;
		if (_activeElement == element)
		{
			ShowTooltip(element);
		}
	}

	void ShowTooltip(VisualElement element)
	{
		_activeElement = element;
		var tooltipContent = _tooltipContents[element];
		_tooltip.AddToClassList("active");
		_tooltip.AddData(tooltipContent);
	}

	void HideTooltip()
	{
		_activeElement = null;
		_tooltip.RemoveFromClassList("active");
	}

	void OnDestroy()
	{
		foreach (var (element, enterCallback, leaveCallback) in _eventRegistrations)
		{
			element.UnregisterCallback(enterCallback);
			element.UnregisterCallback(leaveCallback);
		}
	}
}
