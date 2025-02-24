using UnityEngine;
using System.Collections;

public class SpiritSwap : MonoBehaviour 
{
	public Sprite normalStatue;
	public Sprite activeStatue;
	private Transform player;
	private Vector2 statuePos;
	private Vector2 playerPos;
	private bool inactive;

	void Start ()
	{
		player = GameObject.Find ("Player Wolf").transform;
		statuePos = transform.position;
	}

	void OnTriggerEnter2D (Collider2D col)
	{
		if (col.tag == "DeathObstacle") 
		{
			inactive = true;
			transform.GetComponent<SpriteRenderer> ().sprite = normalStatue;
		}
	}

	void OnTriggerStay2D (Collider2D col)
	{
		if (col.tag == "Player" && inactive == false) 
		{
			transform.GetComponent<SpriteRenderer> ().sprite = activeStatue;
		}

		if (col.tag == "Player" && inactive == false && Input.GetKeyDown(KeyCode.E)) 
		{
			statuePos = transform.position;
			playerPos = player.position;
			player.position = statuePos;
			transform.parent.position = playerPos;
		}
	}

	void OnTriggerExit2D (Collider2D col)
	{
		if (col.tag == "Player" && inactive == false) 
		{
			transform.GetComponent<SpriteRenderer> ().sprite = normalStatue;
		}

		if (col.tag == "DeathObstacle") 
		{
			inactive = false;
			transform.GetComponent<SpriteRenderer> ().sprite = normalStatue;
		}
	}
		
}
