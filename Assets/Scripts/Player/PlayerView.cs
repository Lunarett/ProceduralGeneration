using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
	[SerializeField] private Transform _player;
	[SerializeField] private Transform _playerArms;

	[SerializeField] private float _mouseSensitivity;

	private float _xAxisClamp = 0;

	private void Awake()
	{
		Cursor.lockState = CursorLockMode.Locked;
	}

	private void Update()
	{
		RotateCamera();
	}

	private void RotateCamera()
	{
		float mouseX = Input.GetAxis("Mouse X");
		float mouseY = Input.GetAxis("Mouse Y");

		float rotAmountX = mouseX * _mouseSensitivity;
		float rotAmountY = mouseY * _mouseSensitivity;

		_xAxisClamp -= rotAmountY;

		Vector3 rotPlayerArms = _playerArms.transform.rotation.eulerAngles;
		Vector3 rotPlayer = _player.transform.rotation.eulerAngles;

		rotPlayerArms.x -= rotAmountY;
		rotPlayerArms.z = 0;
		rotPlayer.y += rotAmountX;

		if (_xAxisClamp > 90)
		{
			_xAxisClamp = 90;
			rotPlayerArms.x = 90;
		}

		else if (_xAxisClamp < -90)
		{
			_xAxisClamp = -90;
			rotPlayerArms.x = 270;
		}

		_playerArms.rotation = Quaternion.Euler(rotPlayerArms);
		_player.rotation = Quaternion.Euler(rotPlayer);
	}
}
