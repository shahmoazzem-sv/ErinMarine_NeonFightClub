using Fusion;
using UnityEngine;

public class GameNetworkSpawner : SimulationBehaviour, IPlayerJoined
{
    [SerializeField] private NetworkPrefabRef playerPrefab;

    public void PlayerJoined(PlayerRef player)
    {
        if (Runner.IsServer)
        {
            Vector3 spawnPos = new Vector3(player.RawEncoded * 2f, 0f, 0f);
            Runner.Spawn(playerPrefab, spawnPos, Quaternion.identity, player);
        }
    }
}
