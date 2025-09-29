using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    [Header("Music Tracks")]
    public AudioClip menuMusic;
    public AudioClip level1Music;
    
    [Header("SFX Sounds")]
    public AudioClip jumpSFX;
    public AudioClip attackSFX;
    public AudioClip playerHurtSFX;
    public AudioClip playerDeathSFX;
    
    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene loaded: " + scene.name);
        
        if (scene.name == "MainMenu")
        {
            PlayMenuMusic();
        }
        else if (scene.name == "Level1")
        {
            PlayLevel1Music();
        }
    }
    
    public void PlayMenuMusic()
    {
        if (menuMusic != null && musicSource.clip != menuMusic)
        {
            musicSource.clip = menuMusic;
            musicSource.Play();
            Debug.Log("Now playing Menu music");
        }
    }
    
    public void PlayLevel1Music()
    {
        if (level1Music != null && musicSource.clip != level1Music)
        {
            musicSource.clip = level1Music;
            musicSource.Play();
            Debug.Log("Now playing Level1 music");
        }
    }
    
    public void PlaySFX(string soundName)
    {
        switch (soundName)
        {
            case "Jump":
                if (jumpSFX != null) sfxSource.PlayOneShot(jumpSFX);
                break;
            case "Attack":
                if (attackSFX != null) sfxSource.PlayOneShot(attackSFX);
                break;
            case "PlayerHurt":
                if (playerHurtSFX != null) sfxSource.PlayOneShot(playerHurtSFX);
                break;
            case "PlayerDeath":
                if (playerDeathSFX != null) sfxSource.PlayOneShot(playerDeathSFX);
                break;
        }
    }
}