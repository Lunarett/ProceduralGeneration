using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
	public enum DrawMode { NoiseMap, ColorMap, Mesh };
	public DrawMode drawMode;
	[SerializeField] private bool _useTerrainFlatShading;

	[SerializeField] private int _chunkSize = 96;
	[Range(0, 6)]
	[SerializeField] private int _LevelOfDetail;
	[SerializeField] private float _noiseScale = 30.0f;
	[SerializeField] private float _meshHeightMultiplier;
	[SerializeField] private AnimationCurve _meshHeightCurve;
	[Space]
	[SerializeField] private int _octaves = 4;
	[Range(0, 1)]
	[SerializeField] private float _persistance = 0.5f;
	[SerializeField] private float _lacunarity = 2.0f;
	[Range(0, 145)]
	[SerializeField] private float _radius;
	[Space]
	[SerializeField] private int _seed;
	[SerializeField] private Vector2 _offset;

	[Header("Object Spawner")]
	[SerializeField] private int _densitiy;
	[SerializeField] private GameObject _player;
	[SerializeField] private Element[] _vegetaion;

	public bool AutoUpdate = true;
	public bool RandomizeOnStart = false;

	public TerrainType[] regions;

	private void Start()
	{
		if(RandomizeOnStart)
			_seed = Random.Range(1, int.MaxValue);

		GenerateWorld();
	}

	public void GenerateWorld()
	{
		float[,] noiseMap = Noise.GenerateNoiseMap(_chunkSize + 2, _chunkSize + 2, _seed, _noiseScale, _octaves, _persistance, _lacunarity, _offset, _radius);

		//Color
		Color[] colorMap = new Color[_chunkSize * _chunkSize];
		for (int x = 0; x < _chunkSize; x++)
		{
			for (int y = 0; y < _chunkSize; y++)
			{
				float currentHeight = noiseMap[x, y];

				for (int i = 0; i < regions.Length; i++)
				{
					if (currentHeight <= regions[i].height)
					{
						colorMap[y * _chunkSize + x] = regions[i].color;
						break;
					}
				}
			}
		}

		MapDisplay display = FindObjectOfType<MapDisplay>();

		if (drawMode == DrawMode.NoiseMap)
			display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
		else if (drawMode == DrawMode.ColorMap)
			display.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, _chunkSize, _chunkSize));
		else if (drawMode == DrawMode.Mesh)
			display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, _meshHeightMultiplier, _meshHeightCurve, _LevelOfDetail, _useTerrainFlatShading), TextureGenerator.TextureFromColorMap(colorMap, _chunkSize, _chunkSize));


		SpawnVegetation(noiseMap, _densitiy);
	}

	private void OnValidate()
	{
		if (_lacunarity < 1)
			_lacunarity = 1;

		if (_octaves < 0)
			_octaves = 0;
	}

	private void SpawnVegetation(float[,] noiseMap, int treeAmount)
	{
		DestroyAllChildren();
		bool doOnce = true;

		for (int i = 0; i < treeAmount; i++)
		{
			int rndX = Random.Range(0, _chunkSize);
			int rndZ = Random.Range(0, _chunkSize);

			Vector3 pos = MeshGenerator.ChunkToWorldPos(rndX, rndZ, _chunkSize, noiseMap, _meshHeightMultiplier, _meshHeightCurve);
			int rndVeg = Random.Range(0, _vegetaion.Length);

			//Spawn Player
			if (doOnce && pos.y > 6.5f && pos.y < 10)
			{
				Instantiate(_player, Vector3.Scale(pos, transform.lossyScale), Quaternion.Euler(0, Random.Range(0, 360), 0), transform);
				doOnce = false;
			}

			if (pos.y > _vegetaion[rndVeg].MinHeight && pos.y < _vegetaion[rndVeg].MaxHeight)
			{
				Quaternion rndRot = Quaternion.Euler(0, Random.Range(0, 360), 0);

				GameObject obj = Instantiate(_vegetaion[rndVeg].Prefab, Vector3.Scale(pos, transform.lossyScale), rndRot, transform);
				obj.transform.localScale = new Vector3(1 / transform.lossyScale.x * Random.Range(_vegetaion[rndVeg].MinScale, _vegetaion[rndVeg].MaxScale), 1 / transform.lossyScale.y * Random.Range(_vegetaion[rndVeg].MinScale, _vegetaion[rndVeg].MaxScale), 1 / transform.lossyScale.z * Random.Range(_vegetaion[rndVeg].MinScale, _vegetaion[rndVeg].MaxScale));
			}
		}

	}

	public void DestroyAllChildren()
	{
		for (int i = transform.childCount - 1; i >= 0; i--)
		{
			DestroyImmediate(transform.GetChild(i).gameObject);
		}
	}
}


[System.Serializable]
public struct TerrainType
{
	public string name;
	public float height;
	public Color color;
}

[System.Serializable]
public class Element
{
	public string Name;
	public GameObject Prefab;
	public float MinHeight;
	public float MaxHeight;

	public float MinScale;
	public float MaxScale;
}
