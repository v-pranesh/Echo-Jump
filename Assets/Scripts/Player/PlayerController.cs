//PlayerController.cs

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
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        healthSystem = GetComponent<HealthSystem>();
    }
    
void Update()
{
    CheckGrounded();
    
    // Always show ground status
    Debug.Log("Is Grounded: " + isGrounded);
    
    if (Input.GetKeyDown(KeyCode.Space))
    {
        Debug.Log("Space pressed while grounded: " + isGrounded);
    }
    
    HandleInput();
    UpdateAnimations();
}

// ADD THIS METHOD to see the ground check circle
void OnDrawGizmos()
{
    if (groundCheck != null)
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
    
    void FixedUpdate()
    {
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
        healthSystem.TakeDamage(damage);
        animator.SetTrigger(hurtHash);
        AudioManager.Instance?.PlaySFX("PlayerHurt");
    }
    
    // Called by animation event
    public void OnAttackHit()
    {
        // Additional attack logic can be added here
    }
    
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}