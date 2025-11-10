using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class GameManager : SimulationBehaviour, IPlayerJoined, IPlayerLeft
{

    public static GameManager Instance;

    public NetworkObject playerPrefab;

    Dictionary<PlayerRef, NetworkObject> playerObjects = new();

    void Awake()
    {
        Instance = this;
    }




    public void PlayerJoined(PlayerRef player)
    {
        Debug.Log($"Player joined: {player.PlayerId}");

        Vector3 spawnPos = player.PlayerId == 1 ? new Vector3(0, 0, -2) : new Vector3(0, 0, 2);
        Quaternion rotation = player.PlayerId == 1 ? Quaternion.identity : Quaternion.Euler(0, 180, 0);

        NetworkObject playerObj = Runner.Spawn(playerPrefab, spawnPos, rotation, player);
        playerObjects[player] = playerObj;
    }

    public void PlayerLeft(PlayerRef player)
    {
        if (playerObjects.TryGetValue(player, out var obj))
        {
            Runner.Despawn(obj);
            playerObjects.Remove(player);
        }
    }
}
