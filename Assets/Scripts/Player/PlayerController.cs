using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 12f;
    
    [Header("Combat")]
    public float attackRange = 1.5f;
    public int attackDamage = 1;
    public float attackCooldown = 0.5f;
    public Transform attackPoint;
    public LayerMask enemyLayers;
    
    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayerMask;
    
    [Header("Death Settings")]
    public float deathDelay = 2f;
    public bool isDead = false;
    public float deathYThreshold = -10f;
    
    // Components
    private Rigidbody2D rb;
    private Animator animator;
    private HealthSystem healthSystem;
    
    // State
    private bool isGrounded;
    private bool facingRight = true;
    private float horizontal;
    private float lastAttackTime;
    
    // Animation parameters
    private readonly int speedHash = Animator.StringToHash("Speed");
    private readonly int isGroundedHash = Animator.StringToHash("IsGrounded");
    private readonly int attackHash = Animator.StringToHash("Attack");
    private readonly int hurtHash = Animator.StringToHash("Hurt");
    private readonly int deathHash = Animator.StringToHash("Die");
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        healthSystem = GetComponent<HealthSystem>();
        
        // Subscribe to health system events
        if (healthSystem != null)
        {
            healthSystem.OnDeath.AddListener(OnDeath);
        }
    }
    
    void Update()
    {
        if (isDead) return;
        
        CheckGrounded();
        CheckFallDeath();
        HandleInput();
        UpdateAnimations();
    }
    
    void FixedUpdate()
    {
        if (isDead) return;
        
        HandleMovement();
    }
    
    void HandleInput()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        
        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }
        
        // Attack
        if (Input.GetButtonDown("Fire1") && Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
        }
    }
    
    void HandleMovement()
    {
        // Move horizontally
        rb.linearVelocity = new Vector2(horizontal * moveSpeed, rb.linearVelocity.y);
        
        // Flip sprite based on direction
        if (horizontal > 0 && !facingRight)
        {
            Flip();
        }
        else if (horizontal < 0 && facingRight)
        {
            Flip();
        }
    }
    
    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        AudioManager.Instance?.PlaySFX("Jump");
    }
    
    void Attack()
    {
        lastAttackTime = Time.time;
        animator.SetTrigger(attackHash);
        
        // Check for enemies in attack range
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        
        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                enemyAI.TakeDamage(attackDamage);
            }
        }
        
        AudioManager.Instance?.PlaySFX("Attack");
    }
    
    void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayerMask);
    }
    
    void CheckFallDeath()
    {
        // Check if player has fallen below death threshold
        if (transform.position.y < deathYThreshold && !isDead)
        {
            Debug.Log("Player fell below death threshold! Y: " + transform.position.y);
            Die();
        }
    }
    
    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }
    
    void UpdateAnimations()
    {
        animator.SetFloat(speedHash, Mathf.Abs(horizontal));
        animator.SetBool(isGroundedHash, isGrounded);
    }
    
    public void TakeDamage(int damage)
    {
        if (isDead) return;
        
        healthSystem.TakeDamage(damage);
        animator.SetTrigger(hurtHash);
        AudioManager.Instance?.PlaySFX("PlayerHurt");
    }
    
    // Called by HealthSystem when player dies
    void OnDeath()
    {
        if (isDead) return;
        
        Die();
    }
    
    // Direct death method (for falling, etc.)
    public void Die()
    {
        if (isDead) return;
        
        isDead = true;
        
        // Stop all movement immediately
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false; // This stops all physics interactions
        
        // Disable the player's collider to prevent further collisions
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
            collider.enabled = false;
        
        // Play death animation
        animator.SetTrigger(deathHash);
        
        // Play death sound
        AudioManager.Instance?.PlaySFX("PlayerDeath");
        
        Debug.Log("Player died! Going to GameOver in " + deathDelay + " seconds...");
        
        // Stop all coroutines and invokes to prevent conflicts
        CancelInvoke();
        
        // Go to GameOver screen after delay
        Invoke(nameof(GoToGameOver), deathDelay);
    }
    
    void GoToGameOver()
    {
        Debug.Log("Loading GameOver scene...");
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
        else
        {
            Debug.LogError("GameManager instance not found! Loading scene directly...");
            // Fallback: Load GameOver scene directly
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameOver");
        }
    }
    
    // Called by animation event
    public void OnAttackHit()
    {
        // Additional attack logic can be added here
    }
    
    // Visual debugging
    void OnDrawGizmos()
    {
        // Always draw ground check
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
        
        // Draw death threshold line
        Gizmos.color = Color.red;
        Vector3 deathLineStart = new Vector3(-10f, deathYThreshold, 0f);
        Vector3 deathLineEnd = new Vector3(10f, deathYThreshold, 0f);
        Gizmos.DrawLine(deathLineStart, deathLineEnd);
    }
    
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        
        // Draw attack range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}