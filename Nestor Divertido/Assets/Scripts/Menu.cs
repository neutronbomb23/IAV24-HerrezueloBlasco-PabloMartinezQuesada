using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{

	public GameObject mainMenuHolder;


	void Start()
    {
	
	}


	public void Play()
    {
		SceneManager.LoadScene ("Game");
	}

	public void Quit()
    {
		Application.Quit ();
	}

	
	public void MainMenu()
    {
		mainMenuHolder.SetActive (true);
		
	}
}
