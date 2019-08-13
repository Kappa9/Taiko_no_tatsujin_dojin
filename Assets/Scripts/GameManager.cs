using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static GameManager instance = null;
	public enum GameState
	{
		Title,Selection,Gameplay,Result
	}
	public static GameState state = GameState.Title;
	public bool fading = false;

	public AudioClip[] taikoSound;

	AudioSource au;

	void Awake()
    {
		DontDestroyOnLoad(gameObject.transform.parent.gameObject);
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject.transform.parent.gameObject);
	}

	void OnEnable()
	{
		au = GetComponent<AudioSource>();
	}

	void Update()
    {
		Control();
    }

	void Control()
	{
		if (!fading)
		{
			if (state == GameState.Selection || state == GameState.Gameplay)
			{
				if (Input.GetButtonDown("DonL1") || Input.GetButtonDown("DonR1")) au.PlayOneShot(taikoSound[0]);
				if (Input.GetButtonDown("KaL1") || Input.GetButtonDown("KaR1")) au.PlayOneShot(taikoSound[1]);
			}
			if (Input.GetButtonDown("Start") && state != GameState.Gameplay)
				au.PlayOneShot(taikoSound[0]);
		}
	}
}
