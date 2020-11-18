using UnityEngine;

public class MapDisplay : MonoBehaviour
{
	[SerializeField] private Renderer _textureRenderer;
	[SerializeField] private MeshFilter _meshFilter;
	[SerializeField] private MeshRenderer _meshRenderer;
	[SerializeField] private GameObject _terrainMesh;

	private MeshCollider _meshCollider;


	public void DrawTexture(Texture2D texture)
	{
		_textureRenderer.sharedMaterial.mainTexture = texture;
		_textureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height);
	}

	public void DrawMesh(MeshData meshData, Texture2D texture)
	{
		Mesh mesh = meshData.CreateMesh();

		_meshFilter.sharedMesh = mesh;
		_meshRenderer.sharedMaterial.mainTexture = texture;

		if (_terrainMesh.GetComponent<MeshCollider>())
		{
			DestroyImmediate(_terrainMesh.GetComponent<MeshCollider>());
			_meshCollider = _terrainMesh.AddComponent<MeshCollider>();
		}
		else
		{
			_meshCollider = _terrainMesh.AddComponent<MeshCollider>();
		}

			_meshCollider.sharedMesh = mesh;
	}
}
