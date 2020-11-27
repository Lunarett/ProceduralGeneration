using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PauseMenu : MonoBehaviour
{
	[SerializeField] private FirstPersonAIO _player;
	[SerializeField] private GameObject _canvas;

	private bool _doOnce = true;

	private void Start()
	{
		_canvas.SetActive(false);
	}

	private void Update()
	{
		if (Input.GetButtonDown("Cancel"))
		{
			if (_doOnce)
			{
				_canvas.SetActive(true);
				_doOnce = false;
			}
			else
			{
				_canvas.SetActive(false);
				_doOnce = true;
			}
		}
	}

	public void Resume()
	{
		_player.ControllerPause();
		_canvas.SetActive(false);
		_doOnce = true;
	}

	public void ResetWorld()
	{
		SceneManager.LoadScene(1);
	}

	public void ExitToMainMenu()
	{
		SceneManager.LoadScene(0);
	}
}
