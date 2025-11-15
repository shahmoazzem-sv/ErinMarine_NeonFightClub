using TMPro;
using UnityEditor.VersionControl;
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

    private CanvasGroup pauseMenuCanvasGroup;
    private CanvasGroup gameOverCanvasGroup;

    [Header("Scene Management")]
    public string menuSceneName = "MainMenu";

    private void Awake()
    {
        pauseMenuCanvasGroup = pauseMenuPanel.GetComponent<CanvasGroup>();
        gameOverCanvasGroup = gameOverPanel.GetComponent<CanvasGroup>();

        pauseAction = new InputAction(binding: "<Keyboard/escape");
        pauseAction.performed += context => TogglePause();

        // if (pauseMenuCanvasGroup != null || gameOverCanvasGroup !=null)
        // {
        //     Debug.LogError("CanvasGroup component is missing.");
        // }
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

    private void SetPanelInteractive(GameObject panel, bool isInteractive)
    {
        panel.SetActive(isInteractive); 

        CanvasGroup group = panel.GetComponent<CanvasGroup>();
        if (group != null)
        {
            group.interactable = isInteractive;

            group.blocksRaycasts = isInteractive;
        }
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

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayGameOverMusic();

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
