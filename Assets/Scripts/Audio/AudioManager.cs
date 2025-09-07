//AudioManager.cs
using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        public float volume = 1f;
        public float pitch = 1f;
        public bool loop = false;
    }
    
    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    
    [Header("Sounds")]
    public Sound[] sounds;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void PlaySFX(string soundName)
    {
        Sound sound = System.Array.Find(sounds, s => s.name == soundName);
        if (sound != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(sound.clip, sound.volume);
        }
    }
    
    public void PlayMusic(string soundName)
    {
        Sound sound = System.Array.Find(sounds, s => s.name == soundName);
        if (sound != null && musicSource != null)
        {
            musicSource.clip = sound.clip;
            musicSource.volume = sound.volume;
            musicSource.pitch = sound.pitch;
            musicSource.loop = sound.loop;
            musicSource.Play();
        }
    }
    
    public void StopMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }
    
    public void SetMusicVolume(float volume)
    {
        if (musicSource != null)
            musicSource.volume = volume;
    }
    
    public void SetSFXVolume(float volume)
    {
        if (sfxSource != null)
            sfxSource.volume = volume;
    }
}