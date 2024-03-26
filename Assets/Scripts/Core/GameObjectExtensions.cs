using UnityEngine;

public static class GameObjectExtensions
{
	public static void SetLayerRecursively(this GameObject gameObject, int layer)
	{
		gameObject.layer = layer;
		foreach (Transform child in gameObject.transform)
		{
			SetLayerRecursively(child.gameObject, layer);
		}
	}
}
