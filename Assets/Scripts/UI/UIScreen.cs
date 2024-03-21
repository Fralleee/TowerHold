using UnityEngine.UIElements;
using System;
using UnityEngine;

public class UIScreen : VisualElement
{
	public VisualElement Root { get; }

	const float ScaleUpValue = 1.1f;
	const float ScaleDownValue = 0.9f; // Adjust as needed for the "zoom out" effect
	const float NormalScale = 1f;

	public UIScreen(VisualElement parentElement)
	{
		Root = parentElement ?? throw new ArgumentNullException(nameof(parentElement));
		Root.transform.scale = new Vector3(ScaleDownValue, ScaleDownValue, 1);
		Root.style.display = DisplayStyle.None;
	}

	public void Show()
	{
		Root.style.display = DisplayStyle.Flex;
		Root.transform.scale = new Vector3(NormalScale, NormalScale, 1);
	}

	public void Hide(bool scaleDown = true)
	{
		if (scaleDown)
		{
			Root.transform.scale = new Vector3(ScaleDownValue, ScaleDownValue, 1);
		}
		else
		{
			Root.transform.scale = new Vector3(ScaleUpValue, ScaleUpValue, 1);
		}
		Root.style.display = DisplayStyle.None;
	}
}
