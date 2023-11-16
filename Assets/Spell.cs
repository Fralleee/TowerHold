using UnityEngine;
using UnityEngine.UI;

public abstract class Spell : ScriptableObject
{
  public Sprite image;
  [HideInInspector] public Button button;
  public virtual void Perform()
  {
    if (button != null)
    {
      button.interactable = false;
    }
  }
}
