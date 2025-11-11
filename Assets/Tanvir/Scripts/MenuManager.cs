using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("UI panels")]
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;

    [Header("Scene Name")]
    public string gameSceneName = "GameScene";

    public void OpenSettingsPanel()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettingsPanel()
    {
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void StartGame()
    {
        AudioManager.Instance.PlayMusic(AudioManager.Instance.levelBackground);

        SceneManager.LoadScene(gameSceneName);
    }
}
