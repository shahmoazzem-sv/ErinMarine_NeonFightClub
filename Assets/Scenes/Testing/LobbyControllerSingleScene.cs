using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Fusion.Sockets;
using UnityEngine.SceneManagement;
// using Fusion.SceneManagement; // Corrected using directive for scene management
using Fusion.Photon.Realtime; // Needed for NetAddress, NetConnectFailedReason (often bundled with Fusion)

// Removed IHostMigrationEvents as it often causes conflicts unless fully implemented
// Also removed unnecessary IHostMigrationEvents interface definition at the bottom
public class LobbyControllerSingleScene : MonoBehaviour, INetworkRunnerCallbacks
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField roomNameInput;
    [SerializeField] private TMP_InputField roomPassInput;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button joinButton;
    [SerializeField] private TMP_Text statusText;

    [Header("Network Runner Handler Prefab")]
    [SerializeField] private NetworkRunnerHandler runnerHandlerPrefab;

    [Header("Network Player Prefab")]
    [SerializeField] private NetworkObject playerPrefab; // Assign your Networked Player Prefab here

    private NetworkRunner _runner;
    private const int PLAYER_LIMIT = 2; // Maximum players allowed
    private string roomPassword = null;
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

    private void Start()
    {
        if (playerPrefab == null)
        {
            Debug.LogError("PlayerPrefab is not assigned in the inspector!");
            return;
        }

        hostButton.onClick.AddListener(() => _ = StartGame(GameMode.Host));
        joinButton.onClick.AddListener(() => _ = StartGame(GameMode.Client));
    }

    private async Task StartGame(GameMode mode)
    {
        string roomName = roomNameInput.text.Trim();

        if (string.IsNullOrEmpty(roomName))
        {
            statusText.text = "Enter a room name!";
            return;
        }

        // Only set the password if hosting AND a password was actually entered
        if (mode == GameMode.Host)
        {
            string enteredPass = roomPassInput.text.Trim();
            roomPassword = string.IsNullOrEmpty(enteredPass) ? null : enteredPass;
        }

        // Try to find any existing NetworkRunnerHandler
        var existingHandler = Object.FindFirstObjectByType<NetworkRunnerHandler>();

        if (existingHandler == null)
        {
            existingHandler = Instantiate(runnerHandlerPrefab);
        }

        _runner = existingHandler.Runner;
        _runner.ProvideInput = true;

        // Ensure to remove old callbacks before adding new ones
        _runner.RemoveCallbacks(this);
        _runner.AddCallbacks(this);

        statusText.text = (mode == GameMode.Host) ? "Creating room..." : "Joining room...";

        // ⚠️ IMPORTANT: Get the INetworkSceneManager component
        var sceneManager = existingHandler.GetComponent<INetworkSceneManager>();
        if (sceneManager == null)
        {
            Debug.LogError("NetworkRunnerHandler must have a component that implements INetworkSceneManager (e.g., NetworkSceneManagerDefault).");
            return;
        }

        var startArgs = new StartGameArgs
        {
            GameMode = mode,
            SessionName = roomName,
            // 💡 FIX: Use INetworkSceneManager interface
            SceneManager = sceneManager
        };

        // When joining as a client, we need to pass the password via ConnectionData.
        if (mode == GameMode.Client && roomPassInput != null)
        {
            // Pass the password as part of the connection token
            string clientPassword = roomPassInput.text.Trim();
            if (!string.IsNullOrEmpty(clientPassword))
            {
                // Simple byte array encoding for the token (max 512 bytes)
                startArgs.ConnectionToken = System.Text.Encoding.UTF8.GetBytes(clientPassword);
            }
        }

        var result = await _runner.StartGame(startArgs);

        if (!result.Ok)
        {
            statusText.text = $"Failed: {result.ShutdownReason}";
            Debug.LogError($"Fusion failed: {result.ShutdownReason}");
            Destroy(existingHandler.gameObject);
            return;
        }

        statusText.text = "Runner started successfully. Waiting for players...";
        gameObject.SetActive(false); // UI hides here on success.
    }

    // --- Fusion Callbacks ---

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            // Calculate spawn position
            // Use runner.SessionInfo.PlayerCount for the current count, as ActivePlayers might be stale
            Vector3 spawnPosition = new Vector3(runner.SessionInfo.PlayerCount % 2 == 0 ? 2f : -2f, 0f, 0f);

            // Spawn the player object via the NetworkRunner with the correct input authority
            NetworkObject networkPlayerObject = runner.Spawn(playerPrefab, spawnPosition, Quaternion.identity, player);

            _spawnedCharacters.Add(player, networkPlayerObject);

            Debug.Log($"[Host] Player {player.PlayerId} joined and spawned.");
        }
        else
        {
            Debug.Log($"[Client] Successfully joined the room. Waiting for Host to spawn character.");
        }
    }

    // 💡 FIX: OnConnectRequest is the new place for client rejection (player limit and password check)
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        if (runner.IsServer)
        {
            // 1. Player Limit Check (Use CurrentPlayers property from SessionInfo)
            if (runner.SessionInfo.PlayerCount >= PLAYER_LIMIT)
            {
                Debug.LogWarning($"Rejecting connection. Player limit reached ({PLAYER_LIMIT}).");
                // Do not call request.Accept()
                return;
            }

            // 2. Password Check (only if a password was set by the host)
            if (roomPassword != null)
            {
                if (token == null || token.Length == 0)
                {
                    Debug.LogWarning("Rejecting connection. Password required but none provided.");
                    // Do not call request.Accept()
                    return;
                }

                // Decode the password token sent by the client
                string clientPassword = System.Text.Encoding.UTF8.GetString(token).Trim();

                if (clientPassword != roomPassword)
                {
                    Debug.LogWarning("Rejecting connection. Incorrect password.");
                    // Do not call request.Accept()
                    return;
                }
            }
        }

        // If the code reaches here, accept the connection
        request.Accept();
        Debug.Log($"Accepted connection from {request.RemoteAddress}");
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            _spawnedCharacters.Remove(player);
            Debug.Log($"Player {player.PlayerId} left. Despawning their character.");
        }
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        gameObject.SetActive(true);
        statusText.text = $"Disconnected. Reason: {shutdownReason}";
        Debug.Log($"Runner shut down: {shutdownReason}");

        // Remove callbacks when shutting down to prevent null reference issues
        runner.RemoveCallbacks(this);
    }

    // 💡 FIX: Updated OnConnectFailed signature
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        statusText.text = $"Connection failed: {reason}";
        Debug.LogError($"Connection failed: {reason}");
    }

    // 💡 FIX: Updated OnDisconnectedFromServer signature
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        Debug.Log($"Disconnected from Server: {reason}");
        // Optional: Call OnShutdown manually if needed, but Fusion usually handles this.
    }

    // Placeholder implementations (Some signatures corrected to match Fusion 2 API)
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, System.ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }

    // Implementations for newer required callbacks (AOI, etc.)
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, System.ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
}