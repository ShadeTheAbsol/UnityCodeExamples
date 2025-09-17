using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    private int maxHealth = 100;
    private int currentHealth;

    public Slider healthSlider;
    public event Action PlayerHurt = delegate { };
    public static event Action PlayerDeath = delegate { };
    public event Action<Vector2> PlayerKnockBack = delegate { };


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Sets Player Max Health
        SetMaxHealth(maxHealth);
    }

    // Update is called once per frame
    void Update()
    {

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
            PlayerHurt();
            UpdateHealth(-10);
        }
    }

    //If Player Touches Enemy, Take Damage And Add Knockback Effect To Player By Sending Message With Knockback Direction
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy"))
        {
            Vector2 playerPos = transform.position;
            Vector2 enemyPos = collision.transform.position;
            Vector2 collisionDirection = enemyPos - playerPos;
            PlayerHurt();
            UpdateHealth(-10);
            PlayerKnockBack(collisionDirection.normalized);
        }
    }
}
