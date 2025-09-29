using UnityEngine;

public class MenuMusic : MonoBehaviour
{
    void Start()
    {
        AudioManager.Instance.PlayMenuMusic();
    }
}