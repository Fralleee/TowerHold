using UnityEngine;

public static class Noise
{
	public enum NormalizeMode { Local, Global };

	public static float[,] GenerateNoiseMap(int width, int height, NoiseSettings settings, Vector2 sampleCenter)
	{
		var noiseMap = new float[width, height];
		var prng = new System.Random(settings.Seed);
		var octaveOffsets = new Vector2[settings.Octaves];
		float maxPossibleHeight = 0, amplitude = 1, frequency = 1;

		for (var i = 0; i < settings.Octaves; i++)
		{
			var offsetX = prng.Next(-100000, 100000) + settings.Offset.x + sampleCenter.x;
			var offsetY = prng.Next(-100000, 100000) - settings.Offset.y - sampleCenter.y;
			octaveOffsets[i] = new Vector2(offsetX, offsetY);
			maxPossibleHeight += amplitude;
			amplitude *= settings.Persistance;
		}

		float halfWidth = width / 2f, halfHeight = height / 2f;
		float maxLocalNoiseHeight = float.MinValue, minLocalNoiseHeight = float.MaxValue;

		for (var y = 0; y < height; y++)
		{
			for (var x = 0; x < width; x++)
			{
				amplitude = 1;
				frequency = 1;
				float noiseHeight = 0;

				for (var i = 0; i < settings.Octaves; i++)
				{
					var sampleX = (x - halfWidth + octaveOffsets[i].x) / settings.Scale * frequency;
					var sampleY = (y - halfHeight + octaveOffsets[i].y) / settings.Scale * frequency;
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
					noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
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

	public int Seed;
	public Vector2 Offset;
}
