using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement; 
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
    public AudioClip gameOver;      
    public AudioClip combo;       
    public AudioClip cardSelected;
    public AudioClip cardUnselected;

    private const string MasterKey = "MasterVolume_L";
    private const string MusicKey = "MusicVolume_L";
    private const string SfxKey = "SFXVolume_L";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Transform music = transform.Find("Music");
        if (music != null)
            musicSource = music.GetComponent<AudioSource>();

        Transform sfx = transform.Find("SFX");
        if (sfx != null)
            sfxSource = sfx.GetComponent<AudioSource>();

        LoadInitialVolumeSettings();

        SceneManager.sceneLoaded += OnSceneLoaded;
    } 

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void LoadInitialVolumeSettings()
    {
        float masterVolume = PlayerPrefs.GetFloat(MasterKey, 1f);
        float musicVolume = PlayerPrefs.GetFloat(MusicKey, 1f);
        float sfxVolume = PlayerPrefs.GetFloat(SfxKey, 1f);

        audioMixer.SetFloat("MasterVolume", Mathf.Log10(masterVolume) * 20);
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(musicVolume) * 20);
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(sfxVolume) * 20);
    }
    
    public void InitializeVolumeSliders(VolumeSliderInitializer initializer)
    {
        initializer.masterSlider.minValue = 0.001f;
        initializer.masterSlider.maxValue = 1f;
        initializer.musicSlider.minValue = 0.001f;
        initializer.musicSlider.maxValue = 1f;
        initializer.sfxSlider.minValue = 0.001f;
        initializer.sfxSlider.maxValue = 1f;
        
        initializer.masterSlider.onValueChanged.RemoveAllListeners();
        initializer.musicSlider.onValueChanged.RemoveAllListeners();
        initializer.sfxSlider.onValueChanged.RemoveAllListeners();

        initializer.masterSlider.onValueChanged.AddListener(SetMasterVolume);
        initializer.musicSlider.onValueChanged.AddListener(SetMusicVolume);
        initializer.sfxSlider.onValueChanged.AddListener(SetSfxVolume);
        
        initializer.masterSlider.value = PlayerPrefs.GetFloat(MasterKey, 1f);
        initializer.musicSlider.value = PlayerPrefs.GetFloat(MusicKey, 1f);
        initializer.sfxSlider.value = PlayerPrefs.GetFloat(SfxKey, 1f);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu") 
        {
            PlayMusic(menuBackground);
        }
        else 
        {
            PlayMusic(levelBackground);
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        if (musicSource != null && clip != null && musicSource.clip != clip)
        {
            musicSource.Stop();
            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void PlayGameOverMusic() 
    {
        if (musicSource != null && gameOver != null && musicSource.clip != gameOver)
        {
            musicSource.Stop();
            musicSource.clip = gameOver;
            musicSource.loop = false; 
            musicSource.Play();
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
            sfxSource.PlayOneShot(clip);
    }

    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat(MasterKey, volume); 
        PlayerPrefs.Save();
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat(MusicKey, volume);
        PlayerPrefs.Save();
    }
    
    public void SetSfxVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat(SfxKey, volume);
        PlayerPrefs.Save();
    }
}