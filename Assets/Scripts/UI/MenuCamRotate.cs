using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCamRotate : MonoBehaviour
{
	[SerializeField] private float _speed;

	private Quaternion _newRotation;
	private float _newY;


	private void Update()
	{
		_newY += _speed * Time.deltaTime;
		_newRotation.eulerAngles = new Vector3(transform.rotation.x, _newY, transform.rotation.z);

		transform.rotation = _newRotation;
	}
}
