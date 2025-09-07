//HealthSystem.cs

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
    }
    
public void TakeDamage(int damage)
{
    if (isDead) return;
    
    currentHealth -= damage;
    currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    
    // ADD THIS DEBUG LINE:
    Debug.Log("Player took " + damage + " damage! Health: " + currentHealth + "/" + maxHealth);
    
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
        
        OnHealthChanged?.Invoke(currentHealth);
    }
    
    void Die()
    {
        isDead = true;
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