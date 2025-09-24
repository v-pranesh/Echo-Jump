using UnityEngine;

public class SmoothRepeatBackground : MonoBehaviour
{
    public Transform player;           // assign player here
    public float parallaxFactor = 0.5f; // 0 = static, 1 = exactly follow player
    public Transform[] backgrounds;    // assign all background pieces in order
    public float spriteWidth = 72.8f;  // width of one sprite in Unity units

    private Vector3 lastPlayerPos;

    void Start()
    {
        lastPlayerPos = player.position;
    }

    void LateUpdate()
    {
        // Player movement since last frame
        Vector3 delta = player.position - lastPlayerPos;

        // Move all backgrounds based on player movement
        foreach (Transform bg in backgrounds)
        {
            bg.position += new Vector3(delta.x * parallaxFactor, 0, 0);
        }

        // Reposition backgrounds that moved off-screen
        if (delta.x > 0) // moving right
        {
            Transform first = backgrounds[0];
            Transform last = backgrounds[backgrounds.Length - 1];

            if (first.position.x + spriteWidth / 2 < player.position.x - spriteWidth)
            {
                first.position = last.position + new Vector3(spriteWidth, 0, 0);
                ShiftArrayRight();
            }
        }
        else if (delta.x < 0) // moving left
        {
            Transform first = backgrounds[0];
            Transform last = backgrounds[backgrounds.Length - 1];

            if (last.position.x - spriteWidth / 2 > player.position.x + spriteWidth)
            {
                last.position = first.position - new Vector3(spriteWidth, 0, 0);
                ShiftArrayLeft();
            }
        }

        lastPlayerPos = player.position;
    }

    void ShiftArrayRight()
    {
        Transform temp = backgrounds[0];
        for (int i = 0; i < backgrounds.Length - 1; i++)
            backgrounds[i] = backgrounds[i + 1];
        backgrounds[backgrounds.Length - 1] = temp;
    }

    void ShiftArrayLeft()
    {
        Transform temp = backgrounds[backgrounds.Length - 1];
        for (int i = backgrounds.Length - 1; i > 0; i--)
            backgrounds[i] = backgrounds[i - 1];
        backgrounds[0] = temp;
    }
}
