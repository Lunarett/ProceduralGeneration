using System.Collections;
using UnityEngine;

public class Noise
{
	public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, float radius)
	{
		float[,] noiseMap = new float[mapWidth, mapHeight];
		Vector2 mapSize = new Vector2(mapWidth, mapHeight);

		System.Random prng = new System.Random(seed);
		Vector2[] octaveOffsets = new Vector2[octaves];

		for (int i = 0; i < octaves; i++)
		{
			float offsetX = prng.Next(-100000, 100000) + offset.x;
			float offsetY = prng.Next(-100000, 100000) + offset.y;
			octaveOffsets[i] = new Vector2(offsetX, offsetY);
		}

		if (scale <= 0)
			scale = 0.0001f;

		float maxNoiseHeight = float.MinValue;
		float minNoiseHeight = float.MaxValue;

		float halfWidth = mapWidth / 2f;
		float halfHeight = mapHeight / 2f;

		for (int x = 0; x < noiseMap.GetLength(0); x++)
		{
			for (int y = 0; y < noiseMap.GetLength(1); y++)
			{

				float amplitude = 1;
				float frequency = 1;
				float noiseHeight = 0;

				for (int i = 0; i < octaves; i++)
				{
					float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x;
					float sampleY = (y - halfHeight) / scale * frequency + octaveOffsets[i].y;
					
					float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
					noiseHeight += perlinValue * amplitude;

					amplitude *= persistance;
					frequency *= lacunarity;
				}
				
				if (noiseHeight > maxNoiseHeight)
					maxNoiseHeight = noiseHeight;
				else if (noiseHeight < minNoiseHeight)
					minNoiseHeight = noiseHeight;

				mapSize = new Vector2(noiseMap.GetLength(0), noiseMap.GetLength(1));


				noiseMap[x, y] = noiseHeight;
			}
		}

		for (int x = 0; x < noiseMap.GetLength(0); x++)
		{
			for (int y = 0; y < noiseMap.GetLength(1); y++)
			{
				float radialEffect = Mathf.Clamp01(2 - (Vector2.Distance(new Vector2(x, y), mapSize / 2)) / radius);
				noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]) * radialEffect;
			}
		}

		return noiseMap;
	}
}
