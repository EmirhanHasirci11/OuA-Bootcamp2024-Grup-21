using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public BarManager healthBar;

    // unique player id
    public int playerId = 0;

    // health variables
    public int maxHealth = 100;
    public int currentHealth;

    void Start()
    {
        // initialize currentHealth
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        healthBar.SetTimeBar(currentHealth / maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // ...
    }
}
