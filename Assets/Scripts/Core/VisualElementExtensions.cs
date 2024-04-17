using UnityEngine.UIElements;

public static class VisualElementExtensions
{
	public static void SetPickingModeRecursive(this VisualElement element, PickingMode mode)
	{
		if (element is Box or Button or Label or CustomProgressBar)
		{
			return;
		}

		element.pickingMode = mode;
		foreach (var child in element.Children())
		{
			child.SetPickingModeRecursive(mode);
		}
	}
}
