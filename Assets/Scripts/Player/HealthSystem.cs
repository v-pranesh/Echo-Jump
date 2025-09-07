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
            GameManager.Instance?.GameOver();
        }
        else
        {
            // Enemy death
            Destroy(gameObject, 1f); // Delay to allow death animation
        }
    }
    
    public bool IsAlive()
    {
        return currentHealth > 0;
    }
    
    public float GetHealthPercentage()
    {
        return (float)currentHealth / maxHealth;
    }
}