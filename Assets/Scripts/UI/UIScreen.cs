using UnityEngine.UIElements;
using System;

public class UIScreen : VisualElement
{
	protected bool HideOnAwake;

	protected VisualElement RootElement;

	public VisualElement ParentElement => RootElement;

	public UIScreen(VisualElement parentElement, bool hideOnAwake = true)
	{
		RootElement = parentElement ?? throw new ArgumentNullException(nameof(parentElement));
		HideOnAwake = hideOnAwake;
		Initialize();
	}

	public virtual void Initialize()
	{
		if (HideOnAwake)
		{
			Hide();
		}
	}

	public virtual void Show()
	{
		RootElement.style.display = DisplayStyle.Flex;
	}

	public void Hide()
	{
		RootElement.style.display = DisplayStyle.None;
	}
}
