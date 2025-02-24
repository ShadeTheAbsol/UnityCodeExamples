using UnityEngine;
using System.Collections;

public class Lava : MonoBehaviour 
{
	public Transform gameManager;
	public bool setNewSpawnPoint;
	public Vector2 newSpawnPoint;

	void OnTriggerEnter2D (Collider2D col)
	{
		if (col.tag == "Player" || col.tag == "SpiritRushOrb") 
		{
			if(setNewSpawnPoint)
				gameManager.SendMessage ("ChangeSpawnPoint", newSpawnPoint, SendMessageOptions.DontRequireReceiver);
			
			gameManager.SendMessage ("KillPlayer", SendMessageOptions.DontRequireReceiver);
		}
	}
}
