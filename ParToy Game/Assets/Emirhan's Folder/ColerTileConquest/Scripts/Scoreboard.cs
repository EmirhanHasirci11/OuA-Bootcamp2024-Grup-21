using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using Assets.Emirhan_s_Folder.ColerTileConquest.Scripts;

public class Scoreboard : NetworkBehaviour
{
    [SerializeField] private GameObject playerScoreTextPrefab;
    [SerializeField] private Transform scoreboardPanel;

    private Dictionary<ulong, GameObject> playerScoreEntries = new Dictionary<ulong, GameObject>();

    public struct ScoreboardEntry : INetworkSerializable
    {
        public ulong ClientId;
        public Color PlayerColor;
        public int Score;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref ClientId);
            serializer.SerializeValue(ref PlayerColor);
            serializer.SerializeValue(ref Score);
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;
        }
    }

    private void HandleClientConnected(ulong clientId)
    {
        if (IsServer)
        {
            StartCoroutine(WaitForPlayerObject(clientId));
        }
    }

    private IEnumerator WaitForPlayerObject(ulong clientId)
    {
        yield return new WaitForSeconds(0.5f);

        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out NetworkClient client) && client.PlayerObject != null)
        {
            Player player = client.PlayerObject.GetComponent<Player>();
            CreateScoreEntry(clientId, player.PlayerColor);
            UpdateClientScoreboards();
        }
        else
        {
            Debug.LogWarning($"Client {clientId} için Player objesi bulunamadı.");
        }
    }

    private void HandleClientDisconnected(ulong clientId)
    {
        if (IsServer)
        {
            RemoveScoreEntry(clientId);
            UpdateClientScoreboards();
        }
    }

    public void UpdateScore(ulong clientId, int score)
    {
        if (playerScoreEntries.ContainsKey(clientId))
        {
            var scoreTextComponent = playerScoreEntries[clientId].GetComponent<TextMeshProUGUI>();
            if (scoreTextComponent != null)
            {
                scoreTextComponent.text = $"Player {clientId}: {score}";
            }
        }
    }

    private void CreateScoreEntry(ulong clientId, Color playerColor)
    {
        if (!playerScoreEntries.ContainsKey(clientId))
        {
            GameObject scoreText = Instantiate(playerScoreTextPrefab, scoreboardPanel);
            TextMeshProUGUI textComponent = scoreText.GetComponent<TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.text = $"Player {clientId}: 0";
                textComponent.color = playerColor;

                playerScoreEntries.Add(clientId, scoreText);
                UpdateScoreEntryPositions();
            }
        }
    }

    private void RemoveScoreEntry(ulong clientId)
    {
        if (playerScoreEntries.ContainsKey(clientId))
        {
            Destroy(playerScoreEntries[clientId]);
            playerScoreEntries.Remove(clientId);
            UpdateScoreEntryPositions();
        }
    }

    private void UpdateScoreEntryPositions()
    {
        int index = 0;
        foreach (var entry in playerScoreEntries.Values)
        {
            RectTransform rectTransform = entry.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = new Vector2(0, -index * 30); // Adjust 30 to your desired spacing
                index++;
            }
        }
    }

    private void UpdateClientScoreboards()
    {
        if (IsServer)
        {
            ScoreboardEntry[] scoreboardEntries = new ScoreboardEntry[NetworkManager.Singleton.ConnectedClientsIds.Count];
            int index = 0;
            foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                Player player = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientId).GetComponent<Player>();
                if (player != null)
                {
                    scoreboardEntries[index++] = new ScoreboardEntry
                    {
                        ClientId = clientId,
                        PlayerColor = player.PlayerColor,
                        Score = player.CapturedTiles
                    };
                }
            }
            UpdateScoreboardClientRpc(scoreboardEntries);
        }
    }

    [ClientRpc]
    public void UpdateScoreboardClientRpc(ScoreboardEntry[] entries)
    {
        if (!IsServer)
        {
            ClearScoreboard();

            foreach (ScoreboardEntry entry in entries)
            {
                CreateScoreEntry(entry.ClientId, entry.PlayerColor);
                UpdateScore(entry.ClientId, entry.Score);
            }
        }
    }

    private void ClearScoreboard()
    {
        foreach (Transform child in scoreboardPanel)
        {
            Destroy(child.gameObject);
        }
        playerScoreEntries.Clear();
    }
}
