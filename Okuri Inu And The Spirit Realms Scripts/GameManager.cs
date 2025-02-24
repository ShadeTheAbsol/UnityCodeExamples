using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour 
{
	private Transform player;
	private Vector2 playerPos;
	public Vector2 spawnPoint;
	private Vector2 spawnPos;

	public Transform[] wolfStatues;
	public Vector2[] wolfStatuePositions;
	private int statueArrayLength;

	void Start ()
	{
		player = GameObject.Find ("Player Wolf").transform;
		spawnPos = spawnPoint;
		statueArrayLength = GameObject.FindGameObjectsWithTag("WolfStatue").Length;
		wolfStatues = new Transform[statueArrayLength];
		wolfStatuePositions = new Vector2[statueArrayLength];

		for (int i = 0; i < statueArrayLength; i++) 
		{
			wolfStatues [i] = GameObject.FindGameObjectsWithTag("WolfStatue")[i].transform;
			wolfStatuePositions [i] = wolfStatues [i].position; 
		}
	}

	
	private void KillPlayer()
	{
		player.tag = "Player";
		player.position = spawnPos;

		for (int i = 0; i < statueArrayLength; i++) 
		{
			wolfStatues [i].position = wolfStatuePositions [i];
		}

	}

	private void ChangeSpawnPoint (Vector2 newSpawnPoint)
	{
		spawnPos = newSpawnPoint;
	}
		
}
