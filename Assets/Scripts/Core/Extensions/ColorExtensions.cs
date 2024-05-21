using UnityEngine;

public static class ColorExtensions
{
	public static Color TintColor(this Color color, float whitePercentage = 0.5f)
	{
		return Color.Lerp(color, Color.white, whitePercentage);
	}
}
