using UnityEngine;

// Stores player health and receives damage from projectiles.
public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int CurrentHealth { get; private set; }

    void Awake()
    {
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0 || CurrentHealth <= 0)
            return;

        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);
        Debug.Log($"Player hit for {damage} damage. Health: {CurrentHealth}/{maxHealth}");

        if (CurrentHealth == 0)
            Debug.Log("Player defeated.");
    }
}
