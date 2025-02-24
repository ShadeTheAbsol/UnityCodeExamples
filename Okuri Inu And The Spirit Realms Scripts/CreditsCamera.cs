using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class CreditsCamera : MonoBehaviour 
{
	public float endCreditsPosition;
	public float speed;

	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () 
	{
		StartCoroutine(RollCredits ());
	}

	private IEnumerator RollCredits ()
	{
		if (transform.position.y >= endCreditsPosition) 
		{
			transform.Translate(0, -Time.deltaTime * speed, 0);
		}

		if (transform.position.y <= endCreditsPosition) 
		{
			yield return new WaitForSeconds (2);
			SceneManager.LoadScene ("Main Menu");
		}
	}
}
