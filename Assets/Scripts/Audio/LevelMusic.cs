using UnityEngine;

public class LevelMusic : MonoBehaviour
{
    void Start()
    {
        AudioManager.Instance.PlayLevel1Music();
    }
}