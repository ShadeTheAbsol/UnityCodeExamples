using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour 
{
	public Vector2 spawnPoint;
	public Vector2 spiritTrailSpawnPoint;
	public bool spawnOnSpiritTrail;
	private Transform player;
	private Quaternion playerdefaultRotaion;
	private Transform wolfBreath;
	public static int activeBreath;
	private Transform wolfBreathSprite;
	private Vector2 moveDirection;
	private Vector2 breathLocation;
	private Vector2 breathStartSize;
	private Rigidbody2D playerRigid;
	public float movementSpeed;
	public float crawlSpeed;
	private float movementSpeedTmp;
	public float maxSpeed;
	public float spiritRushMaxSpeed;
	private float maxSpeedTmp;
	public float jumpPower;
	public float breathDistanceLimit;
	public float breathHitSpeed;
	private bool groundHit;
	public float jumpTimer;
	private float tempJumptimer;
	private bool startJumpTimer;
	private bool crouchMode;
	private bool ceilingAbove;
	private int facingDirection;
	private SpriteRenderer wolfSprite;
	public Sprite fireBreath;
	public Sprite iceBreath;
	private Animator anim;

	private RaycastHit2D spiritTrailRay;
	public float spiritTrailStickForce;
	private bool onSpiritTrail;
	public float spiritTrailRayLength;
	public float moveDirectionRayLength;
	private Vector2 averageNormal;
	private Transform spiritRayleftObject;
	private Transform spiritRayRightObject;
	private bool spiritTrailCastExecuting;
	public float rayCheckTimer;

	private RaycastHit2D wallCheckRay;
	private bool changingWalls;
	public bool jumpedToCeiling;
	private bool rotating;
	private float rotateTimer = 0.4f;
	private float tempRotateTimer = 0.4f;

	private bool spiritRushChoosingDirection;
	private bool spiritRushing;
	private int spiritRushChosenDirection;
	private bool leftRightRushChosen;


	// Use this for initialization

	void Start ()
	{
		player = GameObject.FindGameObjectWithTag ("Player").transform;
		playerdefaultRotaion = player.rotation;
		wolfBreath = GameObject.Find ("Wolf Breath").transform;
		wolfBreathSprite = GameObject.Find ("Breath Sprite").transform;
		breathLocation = wolfBreath.localPosition;
		breathStartSize = wolfBreath.GetComponent<BoxCollider2D> ().size;
		spiritRayleftObject = player.FindChild ("SpiritRayLeft");
		spiritRayRightObject = player.FindChild ("SpiritRayRight");
		playerRigid = GetComponent<Rigidbody2D> ();
		facingDirection = 1;
		wolfSprite = GetComponent<SpriteRenderer> ();
		anim = GetComponent<Animator> ();
		playerRigid.velocity = Vector2.zero;
		wolfBreathSprite.gameObject.SetActive (false);
		activeBreath = 0;
		tempJumptimer = jumpTimer;

		if (spawnOnSpiritTrail != true) 
		{
			player.position = spawnPoint;
		} 
		else 
		{
			player.position = spiritTrailSpawnPoint;
		}

	}
	
	// Update is called once per frame
	void Update () 
	{
		if (startJumpTimer == true) 
		{
			wallCheckRay = Physics2D.Raycast (player.position, Quaternion.Euler(0,0,-30) * player.up, 5);
			Debug.DrawRay (player.position, Quaternion.Euler(0,0,-30) * player.up * 5, Color.blue);
		}

		if (rotating) 
		{
			rotateTimer -= Time.deltaTime;

			if (rotateTimer <= 0) 
			{
				rotating = false;
				rotateTimer = tempRotateTimer;
			}
		}
			

		if (player.rotation != playerdefaultRotaion && changingWalls == false && groundHit != true && spiritTrailRay.collider == null) 
		{
			Quaternion wolfChangeDirection = Quaternion.RotateTowards (player.rotation, playerdefaultRotaion, 90);
			player.rotation = Quaternion.Lerp (player.rotation, wolfChangeDirection, Time.deltaTime * 2);
		}

		if (startJumpTimer) 
		{
			jumpTimer -= Time.deltaTime;

			if (jumpTimer <= 1 && wallCheckRay.collider == null && onSpiritTrail == false) 
			{
				changingWalls = false;
				startJumpTimer = false;
			}

			if (jumpTimer <= 0)
			{
				changingWalls = false;
				startJumpTimer = false;
			}
		}
			
		
		spiritTrailRay = Physics2D.Raycast (player.position, -player.up, spiritTrailRayLength);


		if (facingDirection == 1) 
		{
			moveDirection = transform.right;//Quaternion.Euler (0, 0,-90) * spiritTrailRay.normal;
		} 
		else 
		{
			moveDirection = Quaternion.Euler (0, 0,90) * spiritTrailRay.normal;
		}
			

		Debug.DrawRay (player.position, -player.up * spiritTrailRayLength, Color.red);


		if (onSpiritTrail == false && spiritTrailRay.collider != null && spiritTrailRay.collider.gameObject.layer == 11) 
		{
			onSpiritTrail = true;
		} 

		if (onSpiritTrail == true && spiritTrailRay.collider == null || spiritTrailRay.collider != null && spiritTrailRay.collider.gameObject.layer != 11) 
		{
			onSpiritTrail = false;
		}
			

		if (onSpiritTrail) 
		{
			RaycastHit2D moveDirectionRay = Physics2D.Raycast (player.position, moveDirection, moveDirectionRayLength);
			Debug.DrawRay (player.position, moveDirection * moveDirectionRayLength, Color.yellow);

			if (spiritTrailCastExecuting == false && spiritTrailRay.collider) 
			{
				StartCoroutine("SpiritTrailChecker");
			}

			playerRigid.AddForce (-player.up * spiritTrailStickForce);
			player.GetComponent<BoxCollider2D> ().size = new Vector2 (1.43f, 2.21f);
		
			if (moveDirectionRay.collider != null && moveDirectionRay.collider.gameObject.layer == 11 && groundHit == true && wallCheckRay.collider == null) 
			{
				Quaternion wolfChangeDirection = Quaternion.FromToRotation (Vector2.up, moveDirectionRay.normal);
				player.rotation = wolfChangeDirection;
			}
			else if(groundHit == true && spiritTrailRay.collider != null && onSpiritTrail == true && changingWalls == false && startJumpTimer == false && jumpedToCeiling == false)
			{
				Quaternion wolfGroundRotation = Quaternion.FromToRotation (Vector2.up, averageNormal);
				if(rotating == false)
				player.rotation = Quaternion.Lerp (player.rotation, wolfGroundRotation, Time.deltaTime * maxSpeed * 30); 
				//Causes Wall Jump Error!! FIX
			}
				//rotate wolf according to obstacle angle under it
		} 


		if (groundHit || crouchMode == true) 
		{
			anim.SetBool ("Grounded", true);
		} 

		if(groundHit != true && crouchMode == false) 
		{
			anim.SetBool ("Grounded", false);
		}

		if (Input.GetAxisRaw ("Horizontal") > 0 || Input.GetAxis ("Horizontal") < 0) 
		{
			anim.SetBool ("Running", true);
			if(crouchMode == false && onSpiritTrail != true && spiritRushChoosingDirection == false && spiritRushing == false)
			player.GetComponent<BoxCollider2D> ().size = new Vector2 (3.2f, 2.21f);
		}

		if (Input.GetAxisRaw ("Horizontal") == 0) 
		{
			anim.SetBool ("Running", false);
			if(crouchMode == false && spiritRushChoosingDirection == false && spiritRushing == false)
			player.GetComponent<BoxCollider2D> ().size = new Vector2 (1.43f, 2.21f);
		}

		if (Input.GetKeyDown (KeyCode.C) && groundHit && onSpiritTrail != true || Input.GetKeyDown (KeyCode.S) && groundHit && onSpiritTrail != true) 
		{
			if (ceilingAbove == false) 
			{
				crouchMode = !crouchMode;
				Crawl();
			}
		}

		if (Input.GetKeyDown (KeyCode.Z) && crouchMode == false && onSpiritTrail == false) 
		{
			//print ("Entering / Exiting Spirit Rush Mode!");
			spiritRushChoosingDirection = !spiritRushChoosingDirection;
			if (spiritRushChoosingDirection) 
			{
				anim.SetBool ("SpiritDashing", true);
				player.tag = "SpiritRushOrb";
				playerRigid.gravityScale = 0;
				playerRigid.velocity = Vector2.zero;
				player.GetComponent<BoxCollider2D> ().size = new Vector2 (1.5f, 1.5f);
			}

			if(spiritRushChoosingDirection == false || spiritRushing == true)
			{
				spiritRushChoosingDirection = false;
				spiritRushing = false;
				anim.SetBool ("SpiritDashing", false);
				player.tag = "Player";
				playerRigid.velocity = Vector2.zero;
				playerRigid.gravityScale = 1;
			}
		
		}

		if (spiritRushChoosingDirection) 
		{
			playerRigid.velocity = Vector2.zero;

			if(Input.GetKeyDown(KeyCode.LeftArrow))
				{
					if (facingDirection == 1)
					Flip ();

					spiritRushChosenDirection = -1;
					leftRightRushChosen = true;
					spiritRushChoosingDirection = false;
					spiritRushing = true;
				}

			if(Input.GetKeyDown(KeyCode.RightArrow))
			{
				if (facingDirection == -1)
					Flip ();
				
				spiritRushChosenDirection = 1;
				leftRightRushChosen = true;
				spiritRushChoosingDirection = false;
				spiritRushing = true;
			}

			if(Input.GetKeyDown(KeyCode.UpArrow))
			{
				anim.SetBool ("StartGame", false);
				spiritRushChosenDirection = 1;
				leftRightRushChosen = false;
				spiritRushChoosingDirection = false;
				spiritRushing = true;
			}

			if(Input.GetKeyDown(KeyCode.DownArrow))
			{
				spiritRushChosenDirection = -1;
				leftRightRushChosen = false;
				spiritRushChoosingDirection = false;
				spiritRushing = true;
			}
		}


			
			

		if (player.InverseTransformDirection (playerRigid.velocity).x > maxSpeed && spiritRushing == false) 
		{
			Vector2 playerVel = player.InverseTransformDirection (playerRigid.velocity);
			playerVel.x = maxSpeed;
			playerRigid.velocity = player.TransformDirection (playerVel);	
		}
			//playerRigid.velocity = new Vector2(maxSpeed,playerRigid.velocity.y);

		if (player.InverseTransformDirection (playerRigid.velocity).x < -maxSpeed && spiritRushing == false) 
		{
			Vector2 playerVel = player.InverseTransformDirection (playerRigid.velocity);
			playerVel.x = -maxSpeed;
			playerRigid.velocity = player.TransformDirection (playerVel);	
		}

		if (player.InverseTransformDirection (playerRigid.velocity).x > spiritRushMaxSpeed && spiritRushing == true) 
		{
			Vector2 playerVel = player.InverseTransformDirection (playerRigid.velocity);
			playerVel.x = spiritRushMaxSpeed;
			playerRigid.velocity = player.TransformDirection (playerVel);	
		}
		//playerRigid.velocity = new Vector2(maxSpeed,playerRigid.velocity.y);

		if (player.InverseTransformDirection (playerRigid.velocity).x < -spiritRushMaxSpeed && spiritRushing == true) 
		{
			Vector2 playerVel = player.InverseTransformDirection (playerRigid.velocity);
			playerVel.x = -spiritRushMaxSpeed;
			playerRigid.velocity = player.TransformDirection (playerVel);	
		}

		if (player.InverseTransformDirection (playerRigid.velocity).y > spiritRushMaxSpeed && spiritRushing == true) 
		{
			Vector2 playerVel = player.InverseTransformDirection (playerRigid.velocity);
			playerVel.y = spiritRushMaxSpeed;
			playerRigid.velocity = player.TransformDirection (playerVel);	
		}
		//playerRigid.velocity = new Vector2(maxSpeed,playerRigid.velocity.y);

		if (player.InverseTransformDirection (playerRigid.velocity).y < -spiritRushMaxSpeed && spiritRushing == true) 
		{
			Vector2 playerVel = player.InverseTransformDirection (playerRigid.velocity);
			playerVel.y = -spiritRushMaxSpeed;
			playerRigid.velocity = player.TransformDirection (playerVel);	
		}
			//playerRigid.velocity = new Vector2(-maxSpeed,playerRigid.velocity.y);

		if (Input.GetKeyDown (KeyCode.Space) && groundHit && crouchMode == false && spiritRushChoosingDirection == false && spiritRushing == false || Input.GetKeyDown (KeyCode.W) && groundHit && crouchMode == false && spiritRushChoosingDirection == false && spiritRushing == false) 
		{
			if(onSpiritTrail == true)
			changingWalls = true;
			
			startJumpTimer = true;
			jumpTimer = tempJumptimer;
			anim.SetBool ("StartGame", false);

			if (player.right.y >= 0.99f || player.right.y <= -0.99f) 
			{
				playerRigid.AddRelativeForce (new Vector2 (jumpPower * facingDirection * 2, jumpPower), ForceMode2D.Impulse);
			}
			else if (onSpiritTrail == false)
			{
				RaycastHit2D groundRay = Physics2D.Raycast (player.position, -player.up, 1.5f);

				if(groundRay)
				playerRigid.AddRelativeForce (new Vector2 (0f, jumpPower), ForceMode2D.Impulse);
			}
		}

		if (Input.GetKeyDown (KeyCode.Alpha1) && !Input.GetKey (KeyCode.X)) 
		{
			activeBreath = 1;
			wolfBreathSprite.GetComponent<SpriteRenderer> ().sprite = fireBreath;
		}
		else if (Input.GetKeyDown (KeyCode.Alpha2) && !Input.GetKey (KeyCode.X)) 
		{
			activeBreath = 2;
			wolfBreathSprite.GetComponent<SpriteRenderer> ().sprite = iceBreath;
		}
			

		if (Input.GetKey (KeyCode.X) && activeBreath != 0 && crouchMode != true && spiritRushChoosingDirection == false && spiritRushing == false) 
		{
			if(playerRigid.IsSleeping())
			playerRigid.WakeUp ();
	
			if (facingDirection == 1 && wolfBreath.localPosition.x < breathDistanceLimit) 
			{
				wolfBreath.Translate (Vector2.right * Time.deltaTime * breathHitSpeed/2, player);
				wolfBreath.GetComponent<BoxCollider2D> ().size += new Vector2(Time.deltaTime * breathHitSpeed,0);
				wolfBreathSprite.gameObject.SetActive (true);
			} 
			else if (facingDirection == -1 && wolfBreath.localPosition.x > breathDistanceLimit) 
			{
				wolfBreath.Translate (Vector2.left * Time.deltaTime * breathHitSpeed/2, player);
				wolfBreath.GetComponent<BoxCollider2D> ().size += new Vector2(Time.deltaTime * breathHitSpeed,0);
				wolfBreathSprite.gameObject.SetActive (true);
			}
		}

		if (Input.GetKeyUp (KeyCode.X)) 
		{
			wolfBreath.localPosition = breathLocation;
			wolfBreath.GetComponent<BoxCollider2D> ().size = breathStartSize;
			wolfBreathSprite.gameObject.SetActive (false);
		}

		if(Input.GetKeyUp(KeyCode.RightArrow) && anim.GetBool("Crawling") == true || Input.GetKeyUp(KeyCode.LeftArrow) && anim.GetBool("Crawling") == true)
			anim.SetBool ("Crawling", false);
	}

	void FixedUpdate ()
	{
		if (Input.GetAxis ("Horizontal") < 0.1f && Input.GetAxis ("Horizontal") > -0.1f) 
		{
			if (onSpiritTrail == false && groundHit && spiritTrailRay.collider != null && spiritTrailRay.collider.gameObject.layer != 11 && spiritRushing == false) 
			{
				playerRigid.velocity = new Vector2 (0, playerRigid.velocity.y);
			}

			if (startJumpTimer == false && onSpiritTrail)
			{
				Vector2 playerVel = player.InverseTransformDirection (playerRigid.velocity);
				playerVel.x = 0;
				playerRigid.velocity = player.TransformDirection (playerVel);	
			}

	
			if (startJumpTimer == false && spiritRushing == false) 
			{			
					Vector2 playerVel = player.InverseTransformDirection (playerRigid.velocity);
					playerVel.x = 0;
					playerRigid.velocity = player.TransformDirection (playerVel);	//Resolve This Issue. Commented Code Was Original Code Here.
					//playerRigid.velocity = new Vector2 (0, playerRigid.velocity.y);
			} 

		}
			
		

		if (Input.GetKey (KeyCode.RightArrow) && spiritRushChoosingDirection == false && spiritRushing == false || Input.GetKey (KeyCode.D) && spiritRushChoosingDirection == false && spiritRushing == false) 
		{
			if (facingDirection == -1)
				Flip ();

			if(crouchMode && anim.GetBool("Crawling") != true)
				anim.SetBool ("Crawling", true);
		
				playerRigid.AddRelativeForce (new Vector2 (Input.GetAxis ("Horizontal") * movementSpeed, 0f), ForceMode2D.Force);

		
		}



		if (Input.GetKey (KeyCode.LeftArrow) && spiritRushChoosingDirection == false && spiritRushing == false || Input.GetKey (KeyCode.A) && spiritRushChoosingDirection == false && spiritRushing == false) 
		{
			if (facingDirection == 1)
				Flip ();

			if(crouchMode && anim.GetBool("Crawling") != true)
				anim.SetBool ("Crawling", true);

				playerRigid.AddRelativeForce (new Vector2 (Input.GetAxis ("Horizontal") * movementSpeed, 0f), ForceMode2D.Force);
		}

		if (spiritRushing) 
		{

			if (spiritRushChosenDirection == 1)
			{
				if (leftRightRushChosen) 
				{
					playerRigid.AddRelativeForce (new Vector2 (spiritRushChosenDirection * movementSpeed, 0f), ForceMode2D.Force);
				} 
				else
				{
					playerRigid.AddRelativeForce (new Vector2 (0f, spiritRushChosenDirection * movementSpeed), ForceMode2D.Force);
				}
			}
			else 
			{
				if (leftRightRushChosen) 
				{
					playerRigid.AddRelativeForce (new Vector2 (spiritRushChosenDirection * movementSpeed, 0f), ForceMode2D.Force);
				} 
				else
				{
					playerRigid.AddRelativeForce (new Vector2 (0f, spiritRushChosenDirection * movementSpeed), ForceMode2D.Force);
				}
			}
		}
			
	}

	IEnumerator SpiritTrailChecker()
	{
		if(spiritTrailCastExecuting == true)
			yield break;

		spiritTrailCastExecuting = true;
		yield return new WaitForSeconds (rayCheckTimer);
		RaycastHit2D spiritTrailRayLeft = Physics2D.Raycast (spiritRayleftObject.position, -player.up, spiritTrailRayLength);
		RaycastHit2D spiritTrailRayRight = Physics2D.Raycast (spiritRayRightObject.position, -player.up, spiritTrailRayLength);
		Debug.DrawRay (spiritRayleftObject.position, -player.up * spiritTrailRayLength, Color.red);
		Debug.DrawRay (spiritRayRightObject.position, -player.up * spiritTrailRayLength, Color.red);

		if (spiritTrailRayLeft.collider != null && spiritTrailRayRight.collider != null && spiritTrailRayLeft.normal.y != 1 && spiritTrailRayRight.normal.y != 1) 
		{
			averageNormal = (spiritTrailRayLeft.normal + spiritTrailRayRight.normal) / 2;
		} 
		else
		{
			averageNormal = spiritTrailRay.normal;
		} 

		spiritTrailCastExecuting = false;
	}

	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		if (facingDirection == 1) 
		{
			facingDirection = -1;
			wolfSprite.flipX = true;
			wolfBreathSprite.GetComponent<SpriteRenderer> ().flipX = true;
			breathLocation.x *= -1;
			breathDistanceLimit *= -1;
			wolfBreath.localPosition = new Vector2(wolfBreath.localPosition.x * -1, wolfBreath.localPosition.y);
			wolfBreathSprite.localPosition = new Vector2(wolfBreathSprite.localPosition.x * -1, wolfBreathSprite.localPosition.y);
		}
		else if (facingDirection == -1) 
		{
			facingDirection = 1;
			wolfSprite.flipX = false;
			wolfBreathSprite.GetComponent<SpriteRenderer> ().flipX = false;
			breathLocation.x *= -1;
			breathDistanceLimit *= -1;
			wolfBreath.localPosition = new Vector2(wolfBreath.localPosition.x * -1, wolfBreath.localPosition.y);
			wolfBreathSprite.localPosition = new Vector2(wolfBreathSprite.localPosition.x * -1, wolfBreathSprite.localPosition.y);
		}
	}
	void OnCollisionStay2D (Collision2D col)
	{
		if (col.gameObject.CompareTag ("ground"))
		{
			groundHit = true;
			if(player.rotation != playerdefaultRotaion && onSpiritTrail == false && startJumpTimer == false || onSpiritTrail == false && groundHit == true)
				player.rotation = playerdefaultRotaion;

			if(onSpiritTrail)
			playerRigid.gravityScale = 0;
		}

		if(col.gameObject.layer != 12 && spiritRushing == true)
		{
			spiritRushChoosingDirection = false;
			spiritRushing = false;
			anim.SetBool ("SpiritDashing", false);
			player.tag = "Player";
			playerRigid.velocity = Vector2.zero;
			playerRigid.gravityScale = 1;
		}
			
	}

	void OnCollisionEnter2D (Collision2D col)
	{
		if (col.gameObject.layer == 11 && col.transform.CompareTag ("ground") && spiritTrailRay.collider == null)
		{
			player.SetParent (col.transform);
			playerRigid.velocity = Vector2.zero;
			RotatePlayer ();
			//Invoke("RotatePlayer",0.0002f);
			changingWalls = false;
			startJumpTimer = false;
			jumpTimer = tempJumptimer;
		}

		if(col.gameObject.layer != 12 && spiritRushing == true)
		{
			spiritRushChoosingDirection = false;
			spiritRushing = false;
			anim.SetBool ("SpiritDashing", false);
			player.tag = "Player";
			playerRigid.velocity = Vector2.zero;
			playerRigid.gravityScale = 1;
		}

	}

	private void RotatePlayer()
	{
		rotating = true;
		if (jumpedToCeiling == false)
		{
			//print ("Rotated 180");
			player.RotateAround(player.position, Vector3.forward, 180f);
			Flip ();
			player.SetParent(null);
		} 
		else
		{
			//print ("Rotated 90");
			player.RotateAround(player.position, Vector3.forward, -90f);
			//Flip ();
			player.SetParent(null);
		}
	}

	void OnCollisionExit2D (Collision2D col)
	{
		if (col.transform.CompareTag ("ground"))
		{
			groundHit = false;
			if(player.CompareTag("Player"))
			playerRigid.gravityScale = 1;
		}
			
	}

	private void MoveToCeiling ()
	{
		jumpedToCeiling = !jumpedToCeiling;
	}

	private void GravitySafetyDeactivate ()
	{
		groundHit = false;
		playerRigid.gravityScale = 1;
	}

	private void Crawl ()
	{
		if (crouchMode)
		{
			//print ("Currently Crouching");
			player.GetComponent<BoxCollider2D> ().size = new Vector2 (1.43f, 1.34f);
			anim.SetBool ("Crouching", true);
			movementSpeedTmp = movementSpeed;
			movementSpeed = crawlSpeed;
			maxSpeedTmp = maxSpeed;
			maxSpeed = 5;
		} 
		else 
		{
			//print ("Stopped Crouching");
			anim.SetBool ("Crouching", false);
			anim.SetBool ("Crawling", false);
			movementSpeed = movementSpeedTmp;
			maxSpeed = maxSpeedTmp;
		}
	}

	void OnTriggerStay2D (Collider2D col)
	{
		if (col.gameObject.layer == 10 && col.IsTouching(player.GetComponent<Collider2D>()))
		{
			ceilingAbove = true;
		}
	}

	void OnTriggerExit2D (Collider2D col)
	{
		if (col.gameObject.layer == 10)
		{
			ceilingAbove = false;
		}
	}
		
}
