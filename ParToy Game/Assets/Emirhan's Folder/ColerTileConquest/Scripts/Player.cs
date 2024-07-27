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

        // Initialize with a default color 
        public NetworkVariable<Color> Color = new NetworkVariable<Color>(UnityEngine.Color.white);

        public static class ColorTracker
        {
            public static NetworkVariable<Dictionary<Color, int>> ColorCounts =
                new NetworkVariable<Dictionary<Color, int>>(new Dictionary<Color, int>(),
                NetworkVariableReadPermission.Everyone,
                NetworkVariableWritePermission.Server);

            // Static constructor to ensure initialization
            static ColorTracker()
            {
                if (ColorCounts.Value == null)
                {
                    ColorCounts.Value = new Dictionary<Color, int>();
                }
            }
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            _lobbyPlayerData = LobbyManager.Instance.GetPlayerDataFromLobby();

            if (IsServer)
            {
                InitializePlayer(_lobbyPlayerData);

                // Server: Reset ColorCounts on scene load
                if (ColorTracker.ColorCounts.Value == null)
                {
                    ColorTracker.ColorCounts.Value = new Dictionary<Color, int>();
                }

                NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            }
            else if (IsOwner)
            {
                InitializePlayer(_lobbyPlayerData);
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsServer)
            {
                NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            }
            base.OnNetworkDespawn();
        }

        private void OnClientConnected(ulong clientId)
        {
            if (IsServer)
            {
                UpdateColorCountsServerRpc(_lobbyPlayerData.LobbyColor, 1);
            }
        }

        public void InitializePlayer(LobbyPlayerData playerData)
        {
            if (IsServer || IsOwner)
            {
                _lobbyPlayerData = playerData;
                Color.Value = _lobbyPlayerData.LobbyColor;
                gameObject.name = _lobbyPlayerData.Gamertag;
            }
        }

        public void ColorTile(Tile tileToColor, Color newColor)
        {
            if (!IsOwner) return;
            tileToColor.ColorTileServerRpc(newColor, NetworkObjectId);
        }

        [ServerRpc]
        public void UpdateColorCountsServerRpc(Color color, int change)
        {
            var currentCounts = ColorTracker.ColorCounts.Value;

            if (currentCounts == null)
            {
                currentCounts = new Dictionary<Color, int>();
            }

            if (currentCounts.ContainsKey(color))
            {
                currentCounts[color] += change;
                Debug.Log(currentCounts[color]);
            }
            else
            {
                currentCounts.Add(color, change);
            }

            ColorTracker.ColorCounts.Value = currentCounts;
        }

        public static int GetColorCount(Color color)
        {
            if (ColorTracker.ColorCounts.Value != null &&
                ColorTracker.ColorCounts.Value.ContainsKey(color))
            {
                return ColorTracker.ColorCounts.Value[color];
            }
            return 0;
        }
    }
}
