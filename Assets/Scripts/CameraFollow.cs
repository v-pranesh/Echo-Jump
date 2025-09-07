using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset = new Vector3(0, 2, -10);
    
    [Header("Boundaries")]
    public bool useBoundaries = true;
    public float minX = -10f;
    public float maxX = 10f;
    public float minY = -5f;
    public float maxY = 5f;
    
    [Header("Look Ahead")]
    public float lookAheadDistance = 2f;
    public float lookAheadSpeed = 2f;
    
    private Vector3 velocity = Vector3.zero;
    private float currentLookAhead;
    
    void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                target = player.transform;
        }
    }
    
    void LateUpdate()
    {
        if (target == null) return;
        
        // Calculate look ahead
        PlayerController player = target.GetComponent<PlayerController>();
        if (player != null)
        {
            float targetLookAhead = target.localScale.x > 0 ? lookAheadDistance : -lookAheadDistance;
            currentLookAhead = Mathf.Lerp(currentLookAhead, targetLookAhead, lookAheadSpeed * Time.deltaTime);
        }
        
        // Calculate desired position
        Vector3 desiredPosition = target.position + offset + Vector3.right * currentLookAhead;
        
        // Apply boundaries
        if (useBoundaries)
        {
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, minY, maxY);
        }
        
        // Smooth follow
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothSpeed);
        transform.position = smoothedPosition;
    }
    
    void OnDrawGizmosSelected()
    {
        if (!useBoundaries) return;
        
        Gizmos.color = Color.yellow;
        Vector3 bottomLeft = new Vector3(minX, minY, 0);
        Vector3 topRight = new Vector3(maxX, maxY, 0);
        Vector3 bottomRight = new Vector3(maxX, minY, 0);
        Vector3 topLeft = new Vector3(minX, maxY, 0);
        
        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, bottomLeft);
    }
}