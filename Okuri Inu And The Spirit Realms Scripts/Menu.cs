using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour 
{

	public void OnStartClick ()
	{
		SceneManager.LoadScene ("Okuri Inu And The Spirit Realm Trials Prototype Stage");
	}

	public void OnCreditsClick ()
	{
		SceneManager.LoadScene ("Credits");
	}

	public void OnQuitClick ()
	{
		print ("Game Quit");
		Application.Quit();
	}

	public void OnReturnToMenuClick ()
	{
		SceneManager.LoadScene ("Main Menu");
	}

	public void OnRetryClick ()
	{
		SceneManager.LoadScene ("Okuri Inu And The Spirit Realm Trials Prototype Stage");
	}
}
