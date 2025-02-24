using UnityEngine;
using System.Collections;

public class Freezable : MonoBehaviour 
{

	public BoxCollider2D ledge;
	public BoxCollider2D player;
	public Sprite normalWater;
	public Sprite frozenWater;
	public float frozenTimer;
	private float originalTimer;
	private bool frozen;
	private bool playerInWay;

	void Start()
	{
		originalTimer = frozenTimer;
	}

	void OnTriggerStay2D (Collider2D col)
	{
		if (col.tag == "Breath" && PlayerMovement.activeBreath == 2 && Input.GetKey(KeyCode.X) && playerInWay == false) 
		{
			if (frozen == true)
				frozenTimer = originalTimer;
			ledge.enabled = true;
			transform.GetComponent<SpriteRenderer> ().sprite = frozenWater;
			frozen = true;
		}

		if (col.tag == "Breath" && PlayerMovement.activeBreath == 1 && Input.GetKey(KeyCode.X) && frozen == true) 
		{
			frozen = false;
			frozenTimer = originalTimer;
			UnfreezeWater ();
		}
	}

	void UnfreezeWater ()
	{
		frozen = false;
		frozenTimer = originalTimer;
		if (ledge.IsTouching(player)) 
		{
			player.SendMessage ("GravitySafetyDeactivate",SendMessageOptions.DontRequireReceiver);
		}
		ledge.enabled = false;
		transform.GetComponent<SpriteRenderer> ().sprite = normalWater;
	}

	void FreezePossibleCheck ()
	{
		playerInWay = !playerInWay;
	}
		

	void Update ()
	{
		if (frozen) 
		{
			frozenTimer -= Time.deltaTime;

			if (frozenTimer <= 0) 
			{
				frozen = false;
				UnfreezeWater ();
			}
		}
	}
}
