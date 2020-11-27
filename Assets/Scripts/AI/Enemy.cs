using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
	public enum States
	{
		Idle,
		Chase
	};

	
	[SerializeField] private Transform[] _waypoints;

	private GameObject _target;
	private bool _inRange;
	private Transform _currentWaypoint;
	private NavMeshAgent _agent;
	private States _enemyState;
	private Animator _animator;
	private int index = 0;



	private void Awake()
	{
		_agent = GetComponent<NavMeshAgent>();
		_animator = GetComponent<Animator>();
	}

	private void Update()
	{
		SetSate();

		switch (_enemyState)
		{
			case States.Idle:
				OnPatrol();
				break;
			case States.Chase:
				OnChase();
				break;
			default:
				break;
		}

		float y = Vector3.Dot(transform.forward, _agent.velocity);

		_animator.SetFloat("speed", y / 4);
	}

	private void SetSate()
	{
		if(_inRange)
		{
			_enemyState = States.Chase;
		}
		else
		{
			_enemyState = States.Idle;
		}
	}

	private void OnPatrol()
	{
		if (_agent.pathPending)
			return;

		if(_agent.remainingDistance < 1)
		{
			index++;

			if (index >= _waypoints.Length)
			{
				index = 0;
			}
		}

		_agent.SetDestination(_waypoints[index].position);
	}

	private void OnChase()
	{
		_agent.SetDestination(_target.transform.position);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			_target = other.gameObject;
			_inRange = true;
		}
	}
	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			_target = other.gameObject;
			_inRange = false;
		}
	}
}
