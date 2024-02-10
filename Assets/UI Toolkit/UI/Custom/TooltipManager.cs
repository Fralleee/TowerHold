using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TooltipManager : MonoBehaviour
{
	Label _tooltipLabel;
	VisualElement _rootVisualElement;

	readonly List<(VisualElement element, EventCallback<MouseEnterEvent> enterCallback, EventCallback<MouseLeaveEvent> leaveCallback)> _tooltipTargets = new List<(VisualElement, EventCallback<MouseEnterEvent>, EventCallback<MouseLeaveEvent>)>();
	EventCallback<MouseMoveEvent> _mouseMoveCallback;

	bool _tooltipVisible;

	void Start()
	{
		_rootVisualElement = GetComponent<UIDocument>().rootVisualElement;
		_tooltipLabel = _rootVisualElement.Q<Label>("ToolTipLabel");

		var allElements = _rootVisualElement.Query<VisualElement>().Build().ToList();
		foreach (var element in allElements)
		{
			if (!string.IsNullOrWhiteSpace(element.tooltip))
			{
				EventCallback<MouseEnterEvent> enterCallback = e => OnMouseEnter(element);
				EventCallback<MouseLeaveEvent> leaveCallback = e => OnMouseLeave();
				element.RegisterCallback(enterCallback);
				element.RegisterCallback(leaveCallback);

				// Store the element and its callbacks for later removal
				_tooltipTargets.Add((element, enterCallback, leaveCallback));
			}
		}

		_mouseMoveCallback = e => OnMouseMove();
		_rootVisualElement.RegisterCallback(_mouseMoveCallback);
	}

	void OnMouseEnter(VisualElement target)
	{
		_tooltipVisible = true;
		_tooltipLabel.text = target.tooltip;
		_tooltipLabel.AddToClassList("visible");
		UpdateTooltipPosition();
	}

	void OnMouseLeave()
	{
		_tooltipVisible = false;
		_tooltipLabel.RemoveFromClassList("visible");
	}

	void OnMouseMove()
	{
		if (_tooltipVisible)
		{
			UpdateTooltipPosition();
		}
	}

	void UpdateTooltipPosition()
	{
		var screenPosition = Input.mousePosition;

		// Invert only the Y coordinate because Unity's screen coordinates start from the bottom left,
		// while UI Toolkit's coordinates start from the top left.
		screenPosition.y = Screen.height - screenPosition.y;

		// Convert the screen position to a position within the UI Toolkit's coordinate system
		// This accounts for any potential scaling due to DPI settings
		var uiPosition = RuntimePanelUtils.ScreenToPanel(_rootVisualElement.panel, screenPosition);


		// Wait for the UI Toolkit to update the layout
		_tooltipLabel.MarkDirtyRepaint();
		_tooltipLabel.style.position = Position.Absolute;

		// Use scheduled item to ensure the layout is updated before getting the layout size
		_tooltipLabel.schedule.Execute(() =>
		{
			// Adjust the X position by subtracting half the tooltip's width to center it
			var adjustedX = uiPosition.x - (_tooltipLabel.layout.width / 2);

			// Adjust the Y position to move the tooltip above the cursor by a certain offset
			// Here, we use the tooltip's height plus an additional offset (e.g., 24 pixels)
			var adjustedY = uiPosition.y - (_tooltipLabel.layout.height + 24); // Adjust the 24px offset as needed

			_tooltipLabel.style.left = adjustedX;
			_tooltipLabel.style.top = adjustedY;
		});
	}

	void OnDestroy()
	{
		foreach (var (element, enterCallback, leaveCallback) in _tooltipTargets)
		{
			element.UnregisterCallback(enterCallback);
			element.UnregisterCallback(leaveCallback);
		}

		// Unregister mouse move callback
		_rootVisualElement?.UnregisterCallback(_mouseMoveCallback);
	}
}
