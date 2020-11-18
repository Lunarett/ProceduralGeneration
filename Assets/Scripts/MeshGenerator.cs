using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
	public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve heightCurve, int levelOfDetail, bool useTerrainFlatShading)
	{
		//AnimationCurve heightCurve = new AnimationCurve(_heightCurve.keys);

		int meshSimplificationIncrement = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;

		int borderedSize = heightMap.GetLength(0);
		int meshSize = borderedSize - 2 * meshSimplificationIncrement;
		int meshSizeUnsimplified = borderedSize - 2;

		float topLeftX = (meshSizeUnsimplified - 1) / -2f;
		float topLeftZ = (meshSizeUnsimplified - 1) / 2f;

		// new Vector3(topLeftX + percent.x * meshSizeUnsimplified, height, topLeftZ - percent.y * meshSizeUnsimplified);


		int verticesPerLine = (meshSize - 1) / meshSimplificationIncrement + 1;

		MeshData meshData = new MeshData(verticesPerLine, useTerrainFlatShading);

		int[,] vertexIndicesMap = new int[borderedSize, borderedSize];
		int meshVertexIndex = 0;
		int borderVertexIndex = -1;

		for (int y = 0; y < borderedSize; y += meshSimplificationIncrement)
		{
			for (int x = 0; x < borderedSize; x += meshSimplificationIncrement)
			{
				bool isBorderVertex = y == 0 || y == borderedSize - 1 || x == 0 || x == borderedSize - 1;

				if (isBorderVertex)
				{
					vertexIndicesMap[x, y] = borderVertexIndex;
					borderVertexIndex--;
				}
				else
				{
					vertexIndicesMap[x, y] = meshVertexIndex;
					meshVertexIndex++;
				}
			}
		}

		for (int y = 0; y < borderedSize; y += meshSimplificationIncrement)
		{
			for (int x = 0; x < borderedSize; x += meshSimplificationIncrement)
			{
				int vertexIndex = vertexIndicesMap[x, y];
				Vector2 percent = new Vector2((x - meshSimplificationIncrement) / (float)meshSize, (y - meshSimplificationIncrement) / (float)meshSize);

				Vector3 vertexPosition = ChunkToWorldPos(x, y, borderedSize, heightMap, heightMultiplier, heightCurve);

				meshData.AddVertex(vertexPosition, percent, vertexIndex);

				if (x < borderedSize - 1 && y < borderedSize - 1)
				{
					int a = vertexIndicesMap[x, y];
					int b = vertexIndicesMap[x + meshSimplificationIncrement, y];
					int c = vertexIndicesMap[x, y + meshSimplificationIncrement];
					int d = vertexIndicesMap[x + meshSimplificationIncrement, y + meshSimplificationIncrement];
					meshData.AddTriangles(a, d, c);
					meshData.AddTriangles(d, a, b);

				}

				vertexIndex++;
			}
		}
		meshData.Finalize();

		return meshData;

	}

	public static float GetHeightAt(float[,] heightMap, float heightMultiplier, AnimationCurve heightCurve, int y, int x)
	{
		return heightCurve.Evaluate(heightMap[x, y]) * heightMultiplier;
	}

	public static float GetHeightFromWorld(Vector3 worldPos, float[,] heightMap, float heightMultiplier, AnimationCurve heightCurve, Vector3 scale, Vector3 offset)
	{
		int x = (int)((worldPos.x - offset.x) / scale.x + 1 - ((heightMap.GetLength(0) - 3) / -2));
		int y = (int)((worldPos.z - offset.z) / scale.z - 1 - ((heightMap.GetLength(0) - 3) / 2));

		y = -y;

		if (x < 0 || x > heightMap.GetLength(0) || y < 0 || y > heightMap.GetLength(0))
			return 0;


		return GetHeightAt(heightMap, heightMultiplier, heightCurve, y, x) * scale.y;
	}

	public static Vector3 ChunkToWorldPos(int x, int y, int chunkSize, float[,] heightMap, float heightMultiplier, AnimationCurve heightCurve)
	{
		int meshSimplificationIncrement = 1;
		int borderedSize = heightMap.GetLength(0);
		int meshSize = borderedSize - 2 * meshSimplificationIncrement;
		int meshSizeUnsimplified = borderedSize - 2;

		float topLeftX = (meshSizeUnsimplified - 1) / -2f;
		float topLeftZ = (meshSizeUnsimplified - 1) / 2f;

		Vector2 percent = new Vector2((x - meshSimplificationIncrement) / (float)meshSize, (y - meshSimplificationIncrement) / (float)meshSize);
		float height = GetHeightAt(heightMap, heightMultiplier, heightCurve, y, x);
		Vector3 vertexPosition = new Vector3(topLeftX + percent.x * meshSizeUnsimplified, height, topLeftZ - percent.y * meshSizeUnsimplified);

		return vertexPosition;
	}
}

public class MeshData
{
	Vector3[] Vertices;
	int[] Triangles;
	Vector2[] uvs;
	Vector3[] bakedNormals;

	Vector3[] borderVertices;
	int[] borderTriangles;

	int triangleIndex;
	int borderTriangleIndex;

	bool useFlatShading;

	public MeshData(int verticesPerLine, bool useFlatShading)
	{
		this.useFlatShading = useFlatShading;

		Vertices = new Vector3[verticesPerLine * verticesPerLine];
		uvs = new Vector2[verticesPerLine * verticesPerLine];
		Triangles = new int[(verticesPerLine - 1) * (verticesPerLine - 1) * 6];
		borderTriangles = new int[24 * verticesPerLine];

		borderVertices = new Vector3[verticesPerLine * 4 + 4];
	}

	public void AddVertex(Vector3 vertexPosition, Vector2 uv, int vertexIndex)
	{
		if (vertexIndex < 0)
		{
			borderVertices[-vertexIndex - 1] = vertexPosition;
		}
		else
		{
			Vertices[vertexIndex] = vertexPosition;
			uvs[vertexIndex] = uv;
		}
	}

	public void AddTriangles(int a, int b, int c)
	{
		if (a < 0 || b < 0 || c < 0)
		{
			borderTriangles[borderTriangleIndex] = a;
			borderTriangles[borderTriangleIndex + 1] = b;
			borderTriangles[borderTriangleIndex + 2] = c;
			borderTriangleIndex += 3;
		}
		else
		{
			Triangles[triangleIndex] = a;
			Triangles[triangleIndex + 1] = b;
			Triangles[triangleIndex + 2] = c;
			triangleIndex += 3;
		}
	}

	Vector3[] CalculateNormals()
	{

		Vector3[] vertexNormals = new Vector3[Vertices.Length];
		int triangleCount = Triangles.Length / 3;
		for (int i = 0; i < triangleCount; i++)
		{
			int normalTriangleIndex = i * 3;
			int vertexIndexA = Triangles[normalTriangleIndex];
			int vertexIndexB = Triangles[normalTriangleIndex + 1];
			int vertexIndexC = Triangles[normalTriangleIndex + 2];

			Vector3 triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
			vertexNormals[vertexIndexA] += triangleNormal;
			vertexNormals[vertexIndexB] += triangleNormal;
			vertexNormals[vertexIndexC] += triangleNormal;
		}

		int borderTriangleCount = borderTriangles.Length / 3;
		for (int i = 0; i < borderTriangleCount; i++)
		{
			int normalTriangleIndex = i * 3;
			int vertexIndexA = borderTriangles[normalTriangleIndex];
			int vertexIndexB = borderTriangles[normalTriangleIndex + 1];
			int vertexIndexC = borderTriangles[normalTriangleIndex + 2];

			Vector3 triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
			if (vertexIndexA >= 0)
			{
				vertexNormals[vertexIndexA] += triangleNormal;
			}
			if (vertexIndexB >= 0)
			{
				vertexNormals[vertexIndexB] += triangleNormal;
			}
			if (vertexIndexC >= 0)
			{
				vertexNormals[vertexIndexC] += triangleNormal;
			}
		}


		for (int i = 0; i < vertexNormals.Length; i++)
		{
			vertexNormals[i].Normalize();
		}

		return vertexNormals;

	}

	Vector3 SurfaceNormalFromIndices(int iA, int iB, int iC)
	{
		Vector3 pointA = (iA < 0) ? borderVertices[-iA - 1] : Vertices[iA];
		Vector3 pointB = (iB < 0) ? borderVertices[-iB - 1] : Vertices[iB];
		Vector3 pointC = (iC < 0) ? borderVertices[-iC - 1] : Vertices[iC];

		Vector3 sideAB = pointB - pointA;
		Vector3 sideAC = pointC - pointA;

		return Vector3.Cross(sideAB, sideAC).normalized;
	}

	public void Finalize()
	{
		if (useFlatShading)
			FlatShading();
		else
			BakeNormals();
	}

	private void BakeNormals()
	{
		bakedNormals = CalculateNormals();
	}

	void FlatShading()
	{
		Vector3[] flatShadedVertices = new Vector3[Triangles.Length];
		Vector2[] flatShadedUvs = new Vector2[Triangles.Length];

		for (int i = 0; i < Triangles.Length; i++)
		{
			flatShadedVertices[i] = Vertices[Triangles[i]];
			flatShadedUvs[i] = uvs[Triangles[i]];
			Triangles[i] = i;
		}

		Vertices = flatShadedVertices;
		uvs = flatShadedUvs;
	}

	public Mesh CreateMesh()
	{
		Mesh mesh = new Mesh();
		mesh.vertices = Vertices;
		mesh.triangles = Triangles;
		mesh.uv = uvs;

		if (useFlatShading)
			mesh.RecalculateNormals();
		else
			mesh.normals = CalculateNormals();

		return mesh;
	}
}

