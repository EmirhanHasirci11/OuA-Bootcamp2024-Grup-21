using Assets.Emirhan_s_Folder.ColerTileConquest.Scripts;
using Assets.Emirhan_s_Folder.Scripts.LobbyV2;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnManager : NetworkBehaviour
{
    public Transform[] spawnPoints;
    public GameObject[] Players;

    private Dictionary<ulong, GameObject> spawnedPlayers = new Dictionary<ulong, GameObject>();

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;
        }
    }

    public void HandleClientConnected(ulong clientId)
    {
        if (IsServer)
        {
            // Delay player spawning to override NetworkManager's automatic spawning
            StartCoroutine(DelayedSpawnPlayer(clientId));
        }
    }

    public void HandleClientDisconnected(ulong clientId)
    {
        if (IsServer)
        {
            if (spawnedPlayers.TryGetValue(clientId, out GameObject player))
            {
                if (player != null && player.GetComponent<NetworkObject>().IsSpawned)
                {
                    player.GetComponent<NetworkObject>().Despawn();
                }
                Destroy(player);
                spawnedPlayers.Remove(clientId);
            }
        }
    }

    private IEnumerator DelayedSpawnPlayer(ulong clientId)
    {
        yield return new WaitForSeconds(0.1f); 

        if (!spawnedPlayers.ContainsKey(clientId))
        {
            LobbyPlayerData playerData = LobbyManager.Instance.GetPlayerDataFromLobby();

            int spawnIndex = (int)(NetworkManager.Singleton.ConnectedClientsList.Count % spawnPoints.Length);
            int spawnPrefab = (int)(NetworkManager.Singleton.ConnectedClientsList.Count % Players.Length);
            GameObject playerPrefab = Players[spawnPrefab];
            Transform spawnPoint = spawnPoints[spawnIndex];
            GameObject playerInstance = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
            playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);

            Player playerScript = playerInstance.GetComponent<Player>();
            if (playerScript != null)
            {
                playerScript.InitializePlayer(playerData);
            }
            else
            {
                Debug.LogError("Player script not found!");
            }

            // Add the spawned player to the dictionary
            spawnedPlayers[clientId] = playerInstance;
        }
    }
}
