using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    public BarManager healthBar;

    // unique player id
    public NetworkVariable<int> playerId = new NetworkVariable<int>();

    // health variables
    public int maxHealth = 100; // --> float'a çevirmeyi unutma
    public NetworkVariable<int> currentHealth = new NetworkVariable<int>();

    void Start()
    {
        // initialize currentHealth
        if (IsOwner)
        {
            currentHealth.Value = maxHealth;
        }
    }

    [ServerRpc]
    public void TakeDamageServerRpc(int damage)
    {
        currentHealth.Value -= damage;

        healthBar.SetTimeBar((float)currentHealth.Value / maxHealth);

        if (currentHealth.Value <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // ...
    }
}