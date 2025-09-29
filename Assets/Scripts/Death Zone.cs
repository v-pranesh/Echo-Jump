using UnityEngine;

public class DeathZone : MonoBehaviour
{
    [Header("Death Zone Settings")]
    public bool killInstantly = true;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null && !player.isDead)
            {
                Debug.Log("Player entered death zone!");
                if (killInstantly)
                {
                    player.Die(); // Direct death for falling
                }
                else
                {
                    player.TakeDamage(100); // Lethal damage
                }
            }
        }
    }
}