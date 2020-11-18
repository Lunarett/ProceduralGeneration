using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureGenerator
{
	public static Texture2D TextureFromColorMap(Color[] colorMap, int width, int height)
	{
		Texture2D texture = new Texture2D(width, height);
		texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.SetPixels(colorMap);
		texture.Apply();
		return texture;
	}

	public static Texture2D TextureFromHeightMap(float[,] heightMap)
	{
		int w = heightMap.GetLength(0);
		int h = heightMap.GetLength(1);

		Texture2D texture = new Texture2D(w, h);

		Color[] colorMap = new Color[w * h];

		for (int x = 0; x < h; x++)
		{
			for (int y = 0; y < w; y++)
			{
				colorMap[y * w + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
			}
		}

		return TextureFromColorMap(colorMap, w, h);

	}
}
