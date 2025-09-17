using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]

public class PlayerControllerInputSystem : MonoBehaviour
{
    private Animator playerAnimController;
    private Rigidbody2D rb;
    //Input Actions
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction attackAction;
    private bool isPlayerFacingRight = true;
    private bool jump;
    private int knockbackDirection;
    private bool knockback;
    private int comboCounter;
    private float previousAttackTime;
    private bool attackStarted;


    public float speed = 1;
    public float jumpStrength = 1;
    public float knockbackStrength = 1;
    public float comboResetTime = 0.5f;
    public float attackCoolDownTime = 1f;

    private void OnEnable()
    {
        GetComponent<PlayerHealth>().PlayerKnockBack += KnockPlayerBack;
        GetComponent<PlayerHealth>().PlayerHurt += PlayerHurt;
        PlayerHealth.PlayerDeath += PlayerDead;
    }

    private void OnDisable()
    {
        GetComponent<PlayerHealth>().PlayerKnockBack -= KnockPlayerBack;
        GetComponent<PlayerHealth>().PlayerHurt -= PlayerHurt;
        PlayerHealth.PlayerDeath -= PlayerDead;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerAnimController = GetComponent<Animator>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Setup Player Input Actions
        SetupPlayerInputInitialization();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Grounded: " + isPlayerGrounded());
        Vector2 playerMoveValue = moveAction.ReadValue<Vector2>();

        //Play Player Walk Animation If MoveAxis Is Not Equal To 0 And Player Is Grounded
        if (playerMoveValue.x != 0)
        {
            playerAnimController.SetBool("Walk", true);
        }
        else
        {
            playerAnimController.SetBool("Walk", false);
        }

        //Tell FixedUpdate Logic That Player Is Attempting A Jump
        if (jumpAction.WasPressedThisFrame() && isPlayerGrounded())
        {
            jump = true;
        }

        //If Player Is Grounded And Attack Attempted, Start Attack Combo
        if (attackAction.WasPressedThisFrame() && isPlayerGrounded())
        {
            if (attackStarted)
                return;

            if (comboCounter == 0 || Time.time - previousAttackTime <= comboResetTime)
            {
                attackStarted = true;
                comboCounter++;

                //Reset Attack Combo If Combo Counter Goes Higher Than Max Combo
                if (comboCounter > 3)
                {
                    attackStarted = false;
                    comboCounter = 0;
                }

                playerAnimController.SetInteger("Attack State", comboCounter);
                previousAttackTime = Time.time;
            }
            else
            {
                //Reset Attack Combo Back To 0 If Combo Fails
                attackStarted = true;
                comboCounter = 0;
                playerAnimController.SetInteger("Attack State", comboCounter);
                previousAttackTime = Time.time;
            }
        }

        //Reset Combo And End Attack Immediately If No Attack Input Given For A While Mid Combo
        if (comboCounter != 0 && Time.time - previousAttackTime > comboResetTime)
        {
            comboCounter = 0;
            playerAnimController.SetInteger("Attack State", comboCounter);
            attackStarted = false;
        }

        //Activate Player Jump Animation When Player Is Determined As No Longer Grounded
        if (!isPlayerGrounded())
        {
            jump = false;
            playerAnimController.SetBool("Jump", true);
        }
            
        //Disable Player Jump Animation If Player Is Grounded And Player Jump Animation Is Still Active
        if (playerAnimController.GetBool("Jump") && isPlayerGrounded())
        {
            playerAnimController.SetBool("Jump", false);
        }

        //Flip Player Transform Scale Based On Which Direction Player Is Attempting To Move
        if (playerMoveValue.x < 0 && isPlayerFacingRight)
        {
            isPlayerFacingRight = false;
            FlipPlayer();
        }
        else if (playerMoveValue.x > 0 && isPlayerFacingRight == false)
        {
            isPlayerFacingRight = true;
            FlipPlayer();
        }
    }

    private void FixedUpdate()
    {
        Vector2 playerMoveValue = moveAction.ReadValue<Vector2>();

        //Applies Player Movement Based On PlayerMoveValue Read At Time And Prevents Movement During Attacks
        if (playerAnimController.GetInteger("Attack State") == 0)
        {
            rb.linearVelocity = new Vector2(playerMoveValue.x * speed, rb.linearVelocityY);
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocityY);
        }

        //Briefly Applies Upward Velocity When Jump Is Triggered While Maintaining Current Horizontal Move Speed
        if (jump && playerAnimController.GetInteger("Attack State") == 0)
        {
            rb.linearVelocity = new Vector2(playerMoveValue.x * speed, 0 + jumpStrength);
        }

        //Applies Knockback Force To Player When Knocked Back
        if (knockback)
        {
            Vector2 knockbackForce = new Vector2(knockbackStrength * knockbackDirection, 0);
            rb.AddForce(knockbackForce, ForceMode2D.Impulse);
            
            knockback = false;
        }
    }

    //Flips Player Transform Scale On X Axis When Called
    private void FlipPlayer()
    {
        Vector2 playerScale = transform.localScale;
        playerScale.x *= -1;

        transform.localScale = playerScale;
    }

    //Setup Player Input Actions When Called
    private void SetupPlayerInputInitialization()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        attackAction = InputSystem.actions.FindAction("Attack");
    }

    //Checks If Player Is Touching Ground Based On Current Y Axis Linear Velocity
    private bool isPlayerGrounded()
    {
        return rb.linearVelocityY == 0;
    }

    //Calls Knockback Effect With Appropriate Force Direction Based On Provided Direction When Called
    private void KnockPlayerBack(Vector2 forceDirection)
    {
        if (forceDirection.x > 0)
        {
            knockbackDirection = -1;
        }
        else if(forceDirection.x < 0)
        {
            knockbackDirection = 1;
        }

        knockback = true;
    }

    //Ends Player Attack To Determine When Next Combo Attack Can Occur And Ends Attack Entirely After Reaching Max Attack Combo
    private void EndAttack()
    {
        attackStarted = false;

        if (comboCounter == 3)
        {
            comboCounter = 0;
            playerAnimController.SetInteger("Attack State", comboCounter);
        }
    }

    private void PlayerHurt()
    {
        playerAnimController.SetTrigger("Hurt");
    }

    private void PlayerDead()
    {
        playerAnimController.SetBool("Dead", true);
    }

    private void HidePlayer()
    {
        gameObject.SetActive(false);
    }
}
