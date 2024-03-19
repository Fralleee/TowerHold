using UnityEngine.UIElements;
using System;

public class UIScreen : VisualElement
{
	public VisualElement Root { get; }

	public UIScreen(VisualElement parentElement)
	{
		Root = parentElement ?? throw new ArgumentNullException(nameof(parentElement));
		Root.style.display = DisplayStyle.None;
	}

	public void Show()
	{
		Root.style.display = DisplayStyle.Flex;
	}

	public void Hide()
	{
		Root.style.display = DisplayStyle.None;
	}
}
