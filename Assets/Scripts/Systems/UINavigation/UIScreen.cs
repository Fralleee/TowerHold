using UnityEngine.UIElements;
using System;
using UnityEngine;

public class UIScreen : VisualElement
{
	public VisualElement Root { get; }
	public VisualElement Content { get; }

	const float ScaleUpValue = 1.1f;
	const float ScaleDownValue = 0.9f; // Adjust as needed for the "zoom out" effect
	const float NormalScale = 1f;

	public UIScreen(VisualElement parentElement)
	{
		Root = parentElement ?? throw new ArgumentNullException(nameof(parentElement));
		Content = Root.Q<VisualElement>(className: "screen-content");
		Content.transform.scale = new Vector3(ScaleDownValue, ScaleDownValue, 1);
		Root.style.display = DisplayStyle.None;
	}

	public void Show()
	{
		Root.style.display = DisplayStyle.Flex;
		Content.transform.scale = new Vector3(NormalScale, NormalScale, 1);
	}

	public void Hide(bool scaleDown = true)
	{
		if (scaleDown)
		{
			Content.transform.scale = new Vector3(ScaleDownValue, ScaleDownValue, 1);
		}
		else
		{
			Content.transform.scale = new Vector3(ScaleUpValue, ScaleUpValue, 1);
		}
		Root.style.display = DisplayStyle.None;
	}
}
