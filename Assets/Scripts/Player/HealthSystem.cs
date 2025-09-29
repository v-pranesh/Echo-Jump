using UnityEngine;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 3;
    public int currentHealth;
    
    [Header("Events")]
    public UnityEvent<int> OnHealthChanged;
    public UnityEvent OnDeath;
    public UnityEvent OnRespawn;
    
    private bool isDead = false;
    
    void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth);
        
        // Auto-connect to UIManager
        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            OnHealthChanged.AddListener(uiManager.UpdateHealthUI);
            Debug.Log("HealthSystem: Connected to UIManager automatically!");
        }
        else
        {
            Debug.LogError("HealthSystem: UIManager not found in scene!");
        }
    }
    
    public void TakeDamage(int damage)
    {
        if (isDead) return;
        
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        Debug.Log("HealthSystem: Took " + damage + " damage! Health: " + currentHealth + "/" + maxHealth);
        OnHealthChanged?.Invoke(currentHealth);
        
        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }
    
    public void Heal(int healAmount)
    {
        if (isDead) return;
        
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        Debug.Log("HealthSystem: Healed " + healAmount + " health! Health: " + currentHealth + "/" + maxHealth);
        OnHealthChanged?.Invoke(currentHealth);
    }
    
    void Die()
    {
        isDead = true;
        Debug.Log("HealthSystem: Died!");
        OnDeath?.Invoke();
        
        if (gameObject.CompareTag("Player"))
        {
            // Let PlayerController handle the death and respawn process
            PlayerController playerController = GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.Die();
            }
            else
            {
                // Fallback to GameOver if no PlayerController found
                GameManager.Instance?.GameOver();
            }
        }
        else
        {
            // Enemy death
            Destroy(gameObject, 1f); // Delay to allow death animation
        }
    }
    
    public void Respawn()
    {
        isDead = false;
        currentHealth = maxHealth;
        
        Debug.Log("HealthSystem: Respawned with full health!");
        OnHealthChanged?.Invoke(currentHealth);
        OnRespawn?.Invoke();
    }
    
    public void InstantKill()
    {
        if (isDead) return;
        
        currentHealth = 0;
        OnHealthChanged?.Invoke(currentHealth);
        Die();
    }
    
    public void FullHeal()
    {
        if (isDead) return;
        
        currentHealth = maxHealth;
        Debug.Log("HealthSystem: Fully healed! Health: " + currentHealth + "/" + maxHealth);
        OnHealthChanged?.Invoke(currentHealth);
    }
    
    public bool IsAlive()
    {
        return currentHealth > 0 && !isDead;
    }
    
    public bool IsDead()
    {
        return isDead;
    }
    
    public float GetHealthPercentage()
    {
        return (float)currentHealth / maxHealth;
    }
    
    public void SetMaxHealth(int newMaxHealth, bool healToFull = false)
    {
        maxHealth = newMaxHealth;
        if (healToFull)
        {
            currentHealth = maxHealth;
            OnHealthChanged?.Invoke(currentHealth);
        }
        else
        {
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            OnHealthChanged?.Invoke(currentHealth);
        }
    }
    
    // Call this when player falls to their death
    public void HandleFallDeath()
    {
        if (isDead) return;
        
        Debug.Log("HealthSystem: Player fell to death!");
        InstantKill();
    }
}