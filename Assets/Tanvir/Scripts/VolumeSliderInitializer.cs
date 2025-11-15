using UnityEngine;
using UnityEngine.UI;

public class VolumeSliderInitializer : MonoBehaviour
{
    [Header("Slider References")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    private void Start()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.InitializeVolumeSliders(this);
        }
        else
        {
            Debug.LogError("AudioManager is not active or hasn't loaded yet. Check execution order.");
        }
    }
}