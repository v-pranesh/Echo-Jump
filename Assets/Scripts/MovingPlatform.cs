using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Movement")]
    public Transform[] waypoints;
    public float moveSpeed = 2f;
    public float waitTime = 1f;
    
    [Header("Settings")]
    public bool startMovingImmediately = true;
    public bool loop = true;
    
    private int currentWaypoint = 0;
    private float waitTimer = 0f;
    private bool isWaiting = false;
    private bool movingForward = true;
    
    void Start()
    {
        if (waypoints.Length == 0)
        {
            Debug.LogWarning("MovingPlatform has no waypoints!");
            enabled = false;
            return;
        }
        
        transform.position = waypoints[0].position;
    }
    
    void Update()
    {
        if (waypoints.Length < 2) return;
        
        if (isWaiting)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTime)
            {
                isWaiting = false;
                waitTimer = 0f;
            }
            return;
        }
        
        MovePlatform();
    }
    
    void MovePlatform()
    {
        Vector3 targetPosition = waypoints[currentWaypoint].position;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            // Reached waypoint
            isWaiting = true;
            
            if (loop)
            {
                // Loop through waypoints
                if (movingForward)
                {
                    currentWaypoint++;
                    if (currentWaypoint >= waypoints.Length)
                    {
                        currentWaypoint = waypoints.Length - 2;
                        movingForward = false;
                    }
                }
                else
                {
                    currentWaypoint--;
                    if (currentWaypoint < 0)
                    {
                        currentWaypoint = 1;
                        movingForward = true;
                    }
                }
            }
            else
            {
                // One-way movement
                currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
            }
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(transform);
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(null);
        }
    }
    
    void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length < 2) return;
        
        Gizmos.color = Color.green;
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] == null) continue;
            
            Gizmos.DrawWireCube(waypoints[i].position, Vector3.one * 0.5f);
            
            if (i < waypoints.Length - 1 && waypoints[i + 1] != null)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            }
        }
        
        if (loop && waypoints[waypoints.Length - 1] != null && waypoints[0] != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(waypoints[waypoints.Length - 1].position, waypoints[0].position);
        }
    }
}