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

	[SerializeField] private States _enemyState;
	[SerializeField] private GameObject _target;

	private bool _inRange;
	private NavMeshAgent _agent;



	private void Awake()
	{
		_agent = GetComponent<NavMeshAgent>();
	}

	private void Update()
	{
		SetSate();

		switch (_enemyState)
		{
			case States.Idle:
				OnIdle();
				break;
			case States.Chase:
				OnChase();
				break;
			default:
				break;
		}
	}

	private void SetSate()
	{
		if(_inRange)
		{
			Debug.Log(_inRange);
			_enemyState = States.Chase;
		}
		else
		{
			Debug.Log(_inRange);
			_enemyState = States.Idle;
		}
	}

	private void OnIdle()
	{
		Debug.Log(_enemyState);
		_agent.SetDestination(transform.position);

		/*
		 * Ai should go from waypoint 1 to waypoint 2 in the building
		 * needs sight detection
		 */
	}

	private void OnChase()
	{
		_agent.SetDestination(_target.transform.position);
		Debug.Log(_agent.destination);


	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
			_inRange = true;
	}
	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
			_inRange = false;
	}
}
