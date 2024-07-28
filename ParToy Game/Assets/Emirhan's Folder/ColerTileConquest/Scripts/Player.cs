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
              
            }
        }

        public void InitializePlayer(LobbyPlayerData playerData)
        {
            if (IsServer || IsOwner)
            {
                _lobbyPlayerData = playerData;
                
                gameObject.name = _lobbyPlayerData.Gamertag;
            }
        }

       
}
}
