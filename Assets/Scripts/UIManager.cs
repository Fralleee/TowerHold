using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using Sirenix.OdinInspector;
using UnityEngine;

public class UIManager : SerializedSingleton<UIManager>
{
    public Dictionary<Category, Sprite> categorySprites = new Dictionary<Category, Sprite>();
    public Dictionary<Category, Color> categoryColors = new Dictionary<Category, Color>();

}
