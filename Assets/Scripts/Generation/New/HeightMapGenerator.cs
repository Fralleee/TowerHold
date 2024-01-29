using UnityEngine;

public static class HeightMapGenerator
{

	public static HeightMap GenerateHeightMap(int width, int height, HeightMapSettings settings, Vector2 sampleCentre, float[,] originalFalloffMap = null)
	{
		var values = Noise.GenerateNoiseMap(width, height, settings.NoiseSettings, sampleCentre);
		var circularFalloffMap = GenerateCircularFalloffMap(width, settings.IslandMaxRadius);

		var heightCurve_threadsafe = new AnimationCurve(settings.HeightCurve.keys);

		var minValue = float.MaxValue;
		var maxValue = float.MinValue;
		var center = new Vector2(width / 2, height / 2);

		for (var i = 0; i < width; i++)
		{
			for (var j = 0; j < height; j++)
			{
				var value = Mathf.Clamp01(values[i, j] - originalFalloffMap[i, j]);

				var distanceFromCenter = Vector2.Distance(new Vector2(i, j), center);

				value *= heightCurve_threadsafe.Evaluate(value) * settings.HeightMultiplier;

				// Blend with circular falloff map for smooth transition
				if (distanceFromCenter <= settings.IslandMinRadius)
				{
					value = settings.IslandHeight;
				}
				else if (distanceFromCenter <= settings.IslandMaxRadius)
				{
					// Inside the radius, blend towards settings.islandHeight based on circular falloff map
					value = Mathf.Lerp(value, settings.IslandHeight, circularFalloffMap[i, j]);
				}
				else
				{
					// Outside the radius, apply circular falloff map to smoothly transition
					value *= circularFalloffMap[i, j];
				}

				if (value > settings.IslandHeight)
				{
					value = settings.IslandHeight;
				}

				values[i, j] = value;

				// Update min and max values
				if (value > maxValue)
				{
					maxValue = value;
				}
				if (value < minValue)
				{
					minValue = value;
				}
			}
		}

		return new HeightMap(values, minValue, maxValue);
	}

	public static float[,] GenerateFalloffMap(int size)
	{
		var map = new float[size, size];

		for (var i = 0; i < size; i++)
		{
			for (var j = 0; j < size; j++)
			{
				var x = (i / (float)size * 2) - 1;
				var y = (j / (float)size * 2) - 1;

				var value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
				map[i, j] = Evaluate(value);
			}
		}

		return map;
	}

	// Method to generate a circular falloff map
	public static float[,] GenerateCircularFalloffMap(int size, float islandRadius)
	{
		var map = new float[size, size];
		var center = new Vector2(size / 2, size / 2);
		var maxDistance = islandRadius; // Use the island radius as the maximum distance for falloff calculation

		for (var i = 0; i < size; i++)
		{
			for (var j = 0; j < size; j++)
			{
				var distance = Vector2.Distance(new Vector2(i, j), center);
				var value = Mathf.Clamp01(distance / maxDistance);

				// Reverse the value so it's 1 at the center and 0 at the edge
				value = 1 - value;

				// Apply an easing function to make the falloff smoother
				value = Mathf.SmoothStep(0, 1, value);

				map[i, j] = value;
			}
		}

		return map;
	}

	static float Evaluate(float value)
	{
		float a = 3;
		var b = 2.2f;

		return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - (b * value), a));
	}
}

public readonly struct HeightMap
{
	public readonly float[,] Values;
	public readonly float MinValue;
	public readonly float MaxValue;

	public HeightMap(float[,] values, float minValue, float maxValue)
	{
		Values = values;
		MinValue = minValue;
		MaxValue = maxValue;
	}
}


[System.Serializable]
public class HeightMapSettings
{
	public NoiseSettings NoiseSettings;

	public float IslandHeight = 6f;
	public float IslandMinRadius = 60f;
	public float IslandMaxRadius = 128f;
	public float HeightMultiplier = 100f;
	public AnimationCurve HeightCurve = AnimationCurve.Linear(0, 0, 1, 1);

	public float MinHeight => HeightMultiplier * HeightCurve.Evaluate(0);
	public float MaxHeight => HeightMultiplier * HeightCurve.Evaluate(1);
}
