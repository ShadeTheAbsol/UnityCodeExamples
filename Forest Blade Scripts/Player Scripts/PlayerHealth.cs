using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    private int maxHealth = 100;
    private int currentHealth;
    private bool hurtInvincibilityActive;
    private float invincibilityWindowTimer;

    public Slider healthSlider;
    public float invincibilityWindowTime = 1f;
    public event Action PlayerHurt = delegate { };
    public static event Action PlayerDeath = delegate { };
    public event Action<Vector2> PlayerKnockBack = delegate { };


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Sets Player Max Health
        SetMaxHealth(maxHealth);

        invincibilityWindowTimer = invincibilityWindowTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (hurtInvincibilityActive)
        {
            invincibilityWindowTimer -= Time.deltaTime;

            if (invincibilityWindowTimer <= 0)
            {
                hurtInvincibilityActive = false;
                invincibilityWindowTimer = invincibilityWindowTime;
            }
        }

    }

    //Updates Player Max Health
    private void SetMaxHealth(int maxHealth)
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;
        currentHealth = maxHealth;
    }

    //Prevent Player Health Dropping Below 0 When Taking Damage And Update Health
    //Health Updated When Player Takes Damage Or Is Healed
    private void UpdateHealth(int healthValue)
    {
        if (currentHealth == 0)
            return;

        currentHealth += healthValue;
        healthSlider.value = currentHealth;
        //Debug.Log("Current Health: " + currentHealth);
        //Sends PlayerDeath Message If Health Reaches 0
        if (currentHealth <= 0)
            PlayerDeath();
    }

    //If Player Attacked By Enemy, Take Damage
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (hurtInvincibilityActive)
                return;

            hurtInvincibilityActive = true;
            PlayerHurt();
            UpdateHealth(-10);
        }
    }

    //If Player Touches Enemy, Take Damage And Add Knockback Effect To Player By Sending Message With Knockback Direction
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy") && LayerMask.LayerToName(collision.gameObject.layer) == "Enemy")
        {
            Vector2 playerPos = transform.position;
            Vector2 enemyPos = collision.transform.position;
            Vector2 collisionDirection = enemyPos - playerPos;
            PlayerHurt();
            PlayerKnockBack(collisionDirection.normalized);

            if (hurtInvincibilityActive)
                return;

            hurtInvincibilityActive = true;
            UpdateHealth(-10);
        }
    }
}
