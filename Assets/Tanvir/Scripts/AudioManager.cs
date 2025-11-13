using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Source")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip menuBackground;
    public AudioClip levelBackground;
    public AudioClip levelVictory;
    public AudioClip levelDefeat;
    public AudioClip combo;
    public AudioClip cardSelected;
    public AudioClip cardUnselected;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }

        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            Transform music = transform.Find("Music");
            if (music != null)
                musicSource = music.GetComponent<AudioSource>();

            Transform sfx = transform.Find("SFX");

            if (sfx != null)
                sfxSource = sfx.GetComponent<AudioSource>();
        }
    }

    private void Start()
    {
        if (musicSource != null && menuBackground != null)
        {
            musicSource.clip = menuBackground;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
    }
    
    public void SetSfxVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (musicSource != null && clip != null)
        {
            musicSource.Stop();
            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.Play();
        }
    }
    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
            sfxSource.PlayOneShot(clip);
    }
}
