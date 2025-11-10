using UnityEngine;
using UnityEngine.UIElements;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip menuBackground;
    public AudioClip levelBackground;
    public AudioClip levelVictory;
    public AudioClip levelDefeat;
    public AudioClip combo;
    public AudioClip cardSelected;
    public AudioClip cardUnselected;

    private void Start()
    {
        musicSource.clip = menuBackground;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
}
