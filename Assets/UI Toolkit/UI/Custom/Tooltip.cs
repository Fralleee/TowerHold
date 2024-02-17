using UnityEngine.UIElements;

public class Tooltip : VisualElement
{
	public new class UxmlFactory : UxmlFactory<Tooltip, UxmlTraits> { }

	public new class UxmlTraits : VisualElement.UxmlTraits
	{
		// Add attributes here if needed, for example, you might want an attribute for the item name, cost, etc.
	}

	public Tooltip()
	{
		AddToClassList("tooltip-container");
	}
}
