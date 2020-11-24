using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveObjectsNearby : MonoBehaviour
{
	[SerializeField] private LayerMask _layerMask;

	private void OnTriggerStay(Collider other)
	{

		if(other.CompareTag("Vegetation"))
		{
			Debug.Log("Destroyed: " + other);
			DestroyImmediate(other.gameObject);
		}
		else
		{
			Debug.Log("failed to destroy: " + other);
		}
	}
}
