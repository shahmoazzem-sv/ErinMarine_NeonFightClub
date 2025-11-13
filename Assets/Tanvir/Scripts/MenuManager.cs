using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("UI panels")]
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;
    public GameObject quitPanel;

    [Header("Scene Name")]
    public string gameSceneName = "GameScene";

    public void OpenSettingsPanel()
    {
            mainMenuPanel.SetActive(false);
            settingsPanel.SetActive(true);
            quitPanel.SetActive(false);
    }

    public void CloseSettingsPanel()
    {
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        quitPanel.SetActive(false);
    }

    public void OpenQuitPanel()
    {
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(false);
        quitPanel.SetActive(true);            
    }

    public void CloseQuitPanel()
    {
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        quitPanel.SetActive(false);
    }

    public void StartGame()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMusic(AudioManager.Instance.levelBackground);

        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
