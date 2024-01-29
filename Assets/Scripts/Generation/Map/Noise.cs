using UnityEngine;

public static class Noise
{
	public enum NormalizeMode { Local, Global };

	public static float[,] GenerateNoiseMap(int width, int height, RandomGenerator randomGenerator, NoiseSettings settings, Vector2 sampleCenter)
	{
		if (settings.Scale <= 0)
		{
			settings.Scale = 0.0001f;  // Prevent division by zero
		}

		var scaleInverse = 1 / settings.Scale;
		var noiseMap = new float[width, height];
		var octaveOffsets = new Vector2[settings.Octaves];
		float maxPossibleHeight = 0, amplitude = 1;

		var offsetSampleCenter = settings.Offset + sampleCenter;

		for (var i = 0; i < settings.Octaves; i++)
		{
			var offsetX = randomGenerator.Next(-100000, 100000) + offsetSampleCenter.x;
			var offsetY = randomGenerator.Next(-100000, 100000) - offsetSampleCenter.y;
			octaveOffsets[i] = new Vector2(offsetX, offsetY);

			maxPossibleHeight += amplitude;
			amplitude *= settings.Persistance;
		}

		var maxLocalNoiseHeight = float.MinValue;
		var minLocalNoiseHeight = float.MaxValue;
		var halfWidth = width / 2f;
		var halfHeight = height / 2f;

		for (var y = 0; y < height; y++)
		{
			for (var x = 0; x < width; x++)
			{
				amplitude = 1;
				float frequency = 1;
				float noiseHeight = 0;

				for (var i = 0; i < settings.Octaves; i++)
				{
					var sampleX = (x - halfWidth + octaveOffsets[i].x) * scaleInverse * frequency;
					var sampleY = (y - halfHeight + octaveOffsets[i].y) * scaleInverse * frequency;
					var perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
					noiseHeight += perlinValue * amplitude;

					amplitude *= settings.Persistance;
					frequency *= settings.Lacunarity;
				}

				maxLocalNoiseHeight = Mathf.Max(maxLocalNoiseHeight, noiseHeight);
				minLocalNoiseHeight = Mathf.Min(minLocalNoiseHeight, noiseHeight);
				noiseMap[x, y] = noiseHeight;
			}
		}

		for (var y = 0; y < height; y++)
		{
			for (var x = 0; x < width; x++)
			{
				if (settings.NormalizeMode == NormalizeMode.Global)
				{
					var normalizedHeight = (noiseMap[x, y] + 1) / (maxPossibleHeight / 0.9f);
					noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, 1);
				}
				else
				{
					noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
				}
			}
		}

		return noiseMap;
	}
}

[System.Serializable]
public class NoiseSettings
{
	public Noise.NormalizeMode NormalizeMode;
	public float Scale = 50;
	public int Octaves = 3;
	[Range(0, 1)]
	public float Persistance = .5f;
	public float Lacunarity = 2;
	public Vector2 Offset;
}
