using Assets.Emirhan_s_Folder.Scripts.LobbyV2;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Emirhan_s_Folder.ColerTileConquest.Scripts
{
    public class Player : NetworkBehaviour
    {
        [SerializeField] private Renderer playerRenderer;
        private LobbyPlayerData _lobbyPlayerData;

        private NetworkVariable<Color> _playerColor = new NetworkVariable<Color>();
        private NetworkVariable<int> _capturedTiles = new NetworkVariable<int>(0);

        public Color PlayerColor
        {
            get => _playerColor.Value;
            private set => _playerColor.Value = value;
        }

        public int CapturedTiles
        {
            get => _capturedTiles.Value;
            private set => _capturedTiles.Value = value;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            _lobbyPlayerData = LobbyManager.Instance.GetPlayerDataFromLobby();

            if (IsServer)
            {
                InitializePlayer(_lobbyPlayerData);
                NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            }
            else if (IsOwner)
            {
                InitializePlayer(_lobbyPlayerData);
            }

            _playerColor.OnValueChanged += OnPlayerColorChanged;
        }

        private void OnPlayerColorChanged(Color previousColor, Color newColor)
        {
            playerRenderer.material.color = newColor;
        }

        public override void OnNetworkDespawn()
        {
            if (IsServer)
            {
                NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            }
            base.OnNetworkDespawn();

            _playerColor.OnValueChanged -= OnPlayerColorChanged;
        }

        private void OnClientConnected(ulong clientId)
        {
            // Kullanılmıyor
        }

        public void InitializePlayer(LobbyPlayerData playerData)
        {
            if (IsServer || IsOwner)
            {
                _lobbyPlayerData = playerData;
                gameObject.name = _lobbyPlayerData.Gamertag;

                PlayerColor = playerRenderer.material.color;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void CaptureTileServerRpc(bool isAdding, Color oldTileColor)
        {
            if (isAdding)
            {
                CapturedTiles++;
                if (oldTileColor != Color.white)
                {
                    GetPlayerByColor(oldTileColor)?.GetComponent<Player>().RemoveCapturedTileServerRpc();
                }
            }
            else
            {
                CapturedTiles--;
            }

            Scoreboard scoreboard = FindObjectOfType<Scoreboard>();
            scoreboard.UpdateScore(OwnerClientId, CapturedTiles);

            // UpdateClientScoreboards fonksiyonunu çağırmadan önce scoreboardEntry dizisini oluşturun
            Scoreboard.ScoreboardEntry[] scoreboardEntries = new Scoreboard.ScoreboardEntry[NetworkManager.Singleton.ConnectedClientsIds.Count];
            int index = 0;
            foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                Player player = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientId).GetComponent<Player>();
                scoreboardEntries[index++] = new Scoreboard.ScoreboardEntry
                {
                    ClientId = clientId,
                    PlayerColor = player.PlayerColor,
                    Score = player.CapturedTiles
                };
            }
            scoreboard.UpdateScoreboardClientRpc(scoreboardEntries);
        }

        [ServerRpc(RequireOwnership = false)]
        public void RemoveCapturedTileServerRpc()
        {
            CapturedTiles--;
            Scoreboard scoreboard = FindObjectOfType<Scoreboard>();
            scoreboard.UpdateScore(OwnerClientId, CapturedTiles);

            Scoreboard.ScoreboardEntry[] scoreboardEntries = new Scoreboard.ScoreboardEntry[NetworkManager.Singleton.ConnectedClientsIds.Count];
            int index = 0;
            foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                Player player = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientId).GetComponent<Player>();
                scoreboardEntries[index++] = new Scoreboard.ScoreboardEntry
                {
                    ClientId = clientId,
                    PlayerColor = player.PlayerColor,
                    Score = player.CapturedTiles
                };
            }
            scoreboard.UpdateScoreboardClientRpc(scoreboardEntries);
        }

        private GameObject GetPlayerByColor(Color color)
        {
            foreach (var player in FindObjectsOfType<Player>())
            {
                if (player.PlayerColor == color)
                {
                    return player.gameObject;
                }
            }
            return null;
        }
    }
}