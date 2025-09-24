using UnityEngine;

public class FollowPlayerBackground : MonoBehaviour
{
    public Transform player; // assign your player here
    private Vector3 offset;

    void Start()
    {
        if (player != null)
        {
            // Only store horizontal offset
            offset = new Vector3(transform.position.x - player.position.x, transform.position.y, transform.position.z);
        }
    }

    void LateUpdate()
    {
        if (player != null)
        {
            // Move background only horizontally
            transform.position = new Vector3(player.position.x + offset.x, transform.position.y, transform.position.z);
        }
    }
}
