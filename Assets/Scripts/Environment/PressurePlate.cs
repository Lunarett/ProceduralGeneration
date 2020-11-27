using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
	[SerializeField] private Transform _newTransform;
	[SerializeField] private AudioSource _soundOn;
	[SerializeField] private AudioSource _soundOff;

	private Vector3 _initialPos;

	private void Start()
	{
		_initialPos = transform.position;
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player"))
		{
			Debug.Log("enter");
			_soundOn.Play();
			transform.position = _newTransform.position;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			Debug.Log(_initialPos);
			_soundOff.Play();
			transform.position = _initialPos;
		}
	}
}
