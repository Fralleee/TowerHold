using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;

public class TooltipController : MonoBehaviour
{
    private VisualElement hoverMenu;
    private Label tooltipContent;

    // Store references to elements and their callbacks for cleanup
    private List<(VisualElement element, EventCallback<MouseEnterEvent> enterCallback, EventCallback<MouseLeaveEvent> leaveCallback)> eventRegistrations = new List<(VisualElement, EventCallback<MouseEnterEvent>, EventCallback<MouseLeaveEvent>)>();

    void Awake()
    {
        var uiDocument = GetComponent<UIDocument>();
        hoverMenu = uiDocument.rootVisualElement.Q<VisualElement>("HoverMenu");
        tooltipContent = hoverMenu.Q<Label>("TooltipContent");
    }

    public void SetupHoverMenuForElement(VisualElement targetElement, string tooltipText)
    {
        EventCallback<MouseEnterEvent> enterCallback = evt =>
        {
            tooltipContent.text = tooltipText;
            hoverMenu.style.display = DisplayStyle.Flex;
            hoverMenu.style.left = evt.localMousePosition.x + 10;
            hoverMenu.style.top = evt.localMousePosition.y + 10;
        };

        EventCallback<MouseLeaveEvent> leaveCallback = evt => { hoverMenu.style.display = DisplayStyle.None; };

        targetElement.RegisterCallback(enterCallback);
        targetElement.RegisterCallback(leaveCallback);

        // Store the callbacks for cleanup
        eventRegistrations.Add((targetElement, enterCallback, leaveCallback));
    }

    // Method for dynamic content remains similar, not shown for brevity

    void OnDestroy()
    {
        // Unregister callbacks
        foreach (var registration in eventRegistrations)
        {
            registration.element.UnregisterCallback(registration.enterCallback);
            registration.element.UnregisterCallback(registration.leaveCallback);
        }
    }
}
