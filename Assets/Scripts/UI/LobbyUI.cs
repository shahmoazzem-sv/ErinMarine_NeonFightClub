using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;
using System.Threading.Tasks;

public class LobbyUI : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField roomNameInput;
    public TMP_InputField roomPassInput;
    public Button hostButton;
    public Button joinButton;
    public TMP_Text statusText;

    private NetworkRunner _runner;
    private NetworkSceneManagerDefault _sceneManager;

    void Start()
    {
        hostButton.onClick.AddListener(() => StartGameAsync(true).Forget());
        joinButton.onClick.AddListener(() => StartGameAsync(false).Forget());
    }

    private async Task StartGameAsync(bool isHost)
    {
        string roomName = roomNameInput.text.Trim();

        if (string.IsNullOrEmpty(roomName))
        {
            statusText.text = "Enter a room name!";
            return;
        }

        // Create NetworkRunner dynamically
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;

        // Attach default scene manager
        _sceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>();

        statusText.text = isHost ? "Creating room..." : "Joining room...";

        var startArgs = new StartGameArgs()
        {
            GameMode = isHost ? GameMode.Host : GameMode.Client,
            SessionName = roomName,
            SceneManager = _sceneManager
        };

        var result = await _runner.StartGame(startArgs);

        if (!result.Ok)
        {
            statusText.text = $"Failed: {result.ShutdownReason}";
            Debug.LogError($"Failed to start Fusion: {result.ShutdownReason}");
            return;
        }

        Debug.Log($"Joined Room: {roomName}");
        statusText.text = $"Joined Room: {roomName}";

        // Corrected: Check for scene authority using the official property
        if (_runner.IsSceneAuthority)
        {
            statusText.text = "Loading Game Scene...";
            // Make sure the scene at build index 1 is in the Build Settings
            await _runner.LoadScene(SceneRef.FromIndex(1));
        }
    }
}

// Helper to silence async void warnings
public static class TaskExtensions
{
    public static void Forget(this Task task) { }
}