using TMPro;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class _GameManager : MonoBehaviour
{
    private bool isGamePaused = false;

    [Header("UI Elements")]
    public Button submitButton;

    [Header("UI Panels")]
    public GameObject settingsPanel;
    public GameObject pauseMenuPanel;
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverText;

    [Header("Scene Management")]
    public string menuSceneName = "MainMenu";

    private void Start()
    {
        Time.timeScale = 1f;

        settingsPanel.SetActive(false);
        pauseMenuPanel.SetActive(false);
        gameOverPanel.SetActive(false);

        if (submitButton != null)
            submitButton.onClick.AddListener(TriggerGameOver);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isGamePaused)
            {
                if (!settingsPanel.activeSelf)
                    ResumeGame();

                else
                    CloseSettings();
            }

            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        if (gameOverPanel.activeSelf) return;

        isGamePaused = true;
        Time.timeScale = 0f;
        pauseMenuPanel.SetActive(true);
    }

    public void ResumeGame()
    {
        isGamePaused = false;
        Time.timeScale = 1f;
        pauseMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
    }

    public void OpenSettings()
    {
        pauseMenuPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);

        if (isGamePaused)
            pauseMenuPanel.SetActive(true);

        else if (gameOverPanel.activeSelf)
            gameOverPanel.SetActive(true);

        else
            ResumeGame();
    }

    public void TriggerGameOver()
    {
        Time.timeScale = 0f;
        ShowGameOverScreen("Game Over!");
    }

    private void ShowGameOverScreen(string message)
    {
        gameOverText.text = message;
        gameOverPanel.SetActive(true);
        pauseMenuPanel.SetActive(false);
        isGamePaused = false;
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
