//EnemyAI.cs

using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Enemy Settings")]
    public EnemyType enemyType;
    public int maxHealth;
    public int damage;
    public float moveSpeed;
    public float detectionRange;
    public float attackRange;
    public float attackCooldown;
    
    [Header("Patrol")]
    public float patrolDistance = 3f;
    public LayerMask groundLayerMask;
    public LayerMask wallLayerMask;
    
    [Header("References")]
    public Transform groundCheck;
    public Transform wallCheck;
    
    // Components
    private Rigidbody2D rb;
    private Animator animator;
    private HealthSystem healthSystem;
    
    // State
    private Transform player;
    private Vector2 startPosition;
    private bool movingRight = true;
    private bool isChasing = false;
    private float lastAttackTime;
    private bool isDead = false;
    
    // Animation parameters
    private readonly int speedHash = Animator.StringToHash("Speed");
    private readonly int attackHash = Animator.StringToHash("Attack");
    private readonly int hurtHash = Animator.StringToHash("Hurt");
    private readonly int deathHash = Animator.StringToHash("Death");
    
    public enum EnemyType
    {
        Small,   // 1 HP, 1 damage, fast
        Medium,  // 2 HP, 1 damage, medium speed
        Hard     // 3 HP, 2 damage, slow but tanky
    }
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        healthSystem = GetComponent<HealthSystem>();
        
        startPosition = transform.position;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        
        if (healthSystem != null)
        {
            healthSystem.OnDeath.AddListener(OnDeath);
            Debug.Log("Enemy health events connected!");
        }

        SetupEnemyStats();
        SetupHealthSystem();
    }
    
    void SetupEnemyStats()
    {
        switch (enemyType)
        {
            case EnemyType.Small:
                maxHealth = 1;
                damage = 1;
                moveSpeed = 3f;
                break;
            case EnemyType.Medium:
                maxHealth = 2;
                damage = 1;
                moveSpeed = 2f;
                break;
            case EnemyType.Hard:
                maxHealth = 3;
                damage = 2;
                moveSpeed = 1.5f;
                break;
        }
    }
    
    void SetupHealthSystem()
    {
        if (healthSystem != null)
        {
            healthSystem.maxHealth = maxHealth;
            healthSystem.currentHealth = maxHealth;
            healthSystem.OnDeath.AddListener(OnDeath);
        }
    }
    
    void Update()
    {
        if (isDead || player == null) return;
        
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        if (distanceToPlayer <= detectionRange)
        {
            isChasing = true;
            ChasePlayer();
        }
        else
        {
            isChasing = false;
            Patrol();
        }
        
        // Attack if player is in range
        if (distanceToPlayer <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            AttackPlayer();
        }
        
        UpdateAnimations();
    }
    
    void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);
        
        // Flip sprite based on direction
        if (direction.x > 0 && !movingRight)
        {
            Flip();
        }
        else if (direction.x < 0 && movingRight)
        {
            Flip();
        }
    }
    
    void Patrol()
    {
        // Check if we should turn around
        bool shouldTurn = false;
        
        // Check for ground
        if (groundCheck != null)
        {
            bool hasGround = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayerMask);
            if (!hasGround) shouldTurn = true;
        }
        
        // Check for walls
        if (wallCheck != null)
        {
            bool hitWall = Physics2D.OverlapCircle(wallCheck.position, 0.1f, wallLayerMask);
            if (hitWall) shouldTurn = true;
        }
        
        // Check patrol distance
        float distanceFromStart = Vector2.Distance(transform.position, startPosition);
        if (distanceFromStart > patrolDistance) shouldTurn = true;
        
        if (shouldTurn)
        {
            Flip();
        }
        
        // Move
        float direction = movingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(direction * moveSpeed * 0.5f, rb.linearVelocity.y); // Slower patrol speed
    }
    
    void AttackPlayer()
    {
        Debug.Log("Small enemy attacking! Damage: " + damage);
        lastAttackTime = Time.time;
        animator.SetTrigger(attackHash);
        
        // Deal damage to player
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.TakeDamage(damage);
        }
        
        AudioManager.Instance?.PlaySFX("EnemyAttack");
    }
    
    void Flip()
    {
        movingRight = !movingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }
    
    void UpdateAnimations()
    {
        if (animator != null)
        {
            animator.SetFloat(speedHash, Mathf.Abs(rb.linearVelocity.x));
        }
    }
    
    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;
        
        healthSystem?.TakeDamage(damageAmount);
        animator?.SetTrigger(hurtHash);
        AudioManager.Instance?.PlaySFX("EnemyHurt");
    }
    
    void OnDeath()
    {
        isDead = true;
        animator?.SetTrigger(deathHash);
        rb.linearVelocity = Vector2.zero;
        
        // Disable colliders
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D col in colliders)
        {
            col.enabled = false;
        }
        
        AudioManager.Instance?.PlaySFX("EnemyDeath");
        
        // Add score or other rewards here
        GameManager.Instance?.AddScore(GetScoreValue());
    }
    
    int GetScoreValue()
    {
        switch (enemyType)
        {
            case EnemyType.Small: return 10;
            case EnemyType.Medium: return 20;
            case EnemyType.Hard: return 50;
            default: return 10;
        }
    }
    
    void OnDrawGizmosSelected()
    {
        // Detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        // Patrol area
        Gizmos.color = Color.blue;
        Vector2 startPos = Application.isPlaying ? startPosition : transform.position;
        Gizmos.DrawWireSphere(startPos, patrolDistance);
    }
}