using UnityEngine;
using UnityEngine.UI;

public abstract class Spell : ScriptableObject
{
	public Sprite Image;
	[HideInInspector] public Button Button;
	public virtual void Perform()
	{
		if (Button != null)
		{
			Button.interactable = false;
		}
	}
}
