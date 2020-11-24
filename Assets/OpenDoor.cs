using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
	[SerializeField] private Transform _door;
	[SerializeField] private bool _door2;
	[SerializeField] private GameObject _openDoorUI;

	private bool _inRange;
	private bool _flipFlop;

	private void Start()
	{
		if (_openDoorUI != null)
			_openDoorUI.SetActive(false);
	}

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.E) && _door2 && _inRange)
		{
			if(_flipFlop)
			{
				_door.eulerAngles = new Vector3(_door.eulerAngles.x, _door.eulerAngles.y + 90, _door.eulerAngles.z);
				_flipFlop = false;
			}
			else
			{
				_door.eulerAngles = new Vector3(_door.eulerAngles.x, _door.eulerAngles.y - 90, _door.eulerAngles.z);
				_flipFlop = true;
			}

		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player"))
		{
			_inRange = true;

			if (_openDoorUI != null)
				_openDoorUI.SetActive(true);

			if(!_door2)
				_door.eulerAngles = new Vector3(_door.eulerAngles.x, _door.eulerAngles.y - 90, _door.eulerAngles.z);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			_inRange = false;

			if (_openDoorUI != null)
				_openDoorUI.SetActive(false);

			if (!_door2)
				_door.eulerAngles = new Vector3(_door.eulerAngles.x, _door.eulerAngles.y + 90, _door.eulerAngles.z);
		}
	}
}
