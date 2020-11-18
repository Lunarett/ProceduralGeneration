using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	private CharacterController _controller;

	private Vector3 _moveDirection;

	[SerializeField] private float _jumpSpeed = 30;
	[SerializeField] private float _gravity = 2;
	
	private float _moveSpeed;
	private float _walkSpeed = 4;
	private float _sprintSpeed = 6;

	private void Start()
	{
		_controller = GetComponent<CharacterController>();
	}

	private void Update()
	{
		Move();
	}

	private void Move()
	{
		float moveX = Input.GetAxis("Horizontal");
		float moveZ = Input.GetAxis("Vertical");

		if (_controller.isGrounded)
		{
			_moveDirection = new Vector3(moveX, 0, moveZ);
			_moveDirection = transform.TransformDirection(_moveDirection);

			if (Input.GetKey(KeyCode.LeftShift) && moveZ == 1)
			{
				_moveSpeed = _sprintSpeed;
			}

			else
			{
				_moveSpeed = _walkSpeed;
			}

			_moveDirection *= _moveSpeed;

			if (Input.GetKeyDown(KeyCode.Space))
			{
				_moveDirection.y += _jumpSpeed;
			}
		}

		_moveDirection.y -= _gravity;
		_controller.Move(_moveDirection * Time.deltaTime);
	}
}