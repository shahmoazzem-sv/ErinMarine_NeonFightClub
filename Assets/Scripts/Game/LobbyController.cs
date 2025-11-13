using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;
using System.Threading.Tasks;

/// <summary>
/// Handles creating or joining a local multiplayer lobby using
/// a persistent NetworkRunnerHandler prefab.
/// </summary>
public class LobbyController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField roomNameInput;
    [SerializeField] private TMP_InputField roomPassInput; // optional
    [SerializeField] private Button hostButton;
    [SerializeField] private Button joinButton;
    [SerializeField] private TMP_Text statusText;

    [Header("Network Runner Handler Prefab")]
    [SerializeField] private NetworkRunnerHandler runnerHandlerPrefab;

    private NetworkRunner _runner;
    private NetworkSceneManagerDefault _sceneManager;

    private const int GAME_SCENE_BUILD_INDEX = 1;

    private void Start()
    {
        hostButton.onClick.AddListener(() => _ = StartGame(true));
        joinButton.onClick.AddListener(() => _ = StartGame(false));
    }

    private async Task StartGame(bool isHost)
    {
        string roomName = roomNameInput.text.Trim();

        if (string.IsNullOrEmpty(roomName))
        {
            statusText.text = "Enter a room name!";
            return;
        }

        // Try to find any existing NetworkRunnerHandler in scene or persistent hierarchy
        var existingHandler = Object.FindFirstObjectByType<NetworkRunnerHandler>();

        if (existingHandler == null)
        {
            // Spawn the prefab (it contains NetworkRunner + SceneManager + DontDestroyOnLoad)
            existingHandler = Instantiate(runnerHandlerPrefab);
        }

        _runner = existingHandler.Runner;
        _sceneManager = existingHandler.SceneManager;

        _runner.ProvideInput = true;
        statusText.text = isHost ? "Creating room..." : "Joining room...";

        var result = await _runner.StartGame(new StartGameArgs
        {
            GameMode = isHost ? GameMode.Host : GameMode.Client,
            SessionName = roomName,
            SceneManager = _sceneManager
        });

        if (!result.Ok)
        {
            statusText.text = $"Failed: {result.ShutdownReason}";
            Debug.LogError($"Fusion failed: {result.ShutdownReason}");
            Destroy(existingHandler.gameObject);
            return;
        }

        statusText.text = "Room joined successfully!";
        Debug.Log($"[{(isHost ? "Host" : "Client")}] Joined Room: {roomName}");

        if (_runner.IsSceneAuthority)
        {
            statusText.text = "Loading Game Scene...";
            await _runner.LoadScene(SceneRef.FromIndex(GAME_SCENE_BUILD_INDEX));
        }

        // Hide the lobby UI after transition
        gameObject.SetActive(false);
    }
}
