using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class _GameManager : MonoBehaviour
{
    private bool isGamePaused = false;
    private InputAction pauseAction;

    [Header("UI Elements")]
    public Button submitButton;

    [Header("UI Panels")]
    public GameObject settingsPanel;
    public GameObject pauseMenuPanel;
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverText;

    [Header("Scene Management")]
    public string menuSceneName = "MainMenu";

    private void Awake()
    {
        pauseAction = new InputAction(binding: "<Keyboard>/escape");
        pauseAction.performed += context => TogglePause();
    }

    private void Start()
    {
        Time.timeScale = 1f;

        settingsPanel.SetActive(false);
        pauseMenuPanel.SetActive(false);
        gameOverPanel.SetActive(false);

        if (submitButton != null)
            submitButton.onClick.AddListener(TriggerGameOver);
    }

    private void OnEnable()
    {
        pauseAction.Enable();
    }

    private void OnDisable()
    {
        pauseAction.Disable();
        pauseAction.performed -= context => TogglePause();
    }

    private void TogglePause()
    {
        if (gameOverPanel.activeSelf) return;

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
        settingsPanel.SetActive(true);

        if (isGamePaused)
            pauseMenuPanel.SetActive(false);

        else if (gameOverPanel.activeSelf)
            gameOverPanel.SetActive(false);

        else
            PauseGame();
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
        ShowGameOverScreen("GameOver!");
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
