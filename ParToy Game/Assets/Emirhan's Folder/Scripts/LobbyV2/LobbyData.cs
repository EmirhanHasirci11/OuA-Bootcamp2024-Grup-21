using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Services.Lobbies.Models;

namespace Assets.Emirhan_s_Folder.Scripts.LobbyV2
{
    public class LobbyData
    {
        private string _lobbyGameMode;
        private string _relayJoinCode;

        public string LobbyGameMode
        {
            get => _lobbyGameMode; 
            set=>_lobbyGameMode=value;
        }
        public string RelayJoinCode 
        { get=> _relayJoinCode; 
          set=> _relayJoinCode=value; }

        public void Initialize(Dictionary<string, DataObject> lobbyData)
        {
            UpdateState(lobbyData);
        }
        public void Initialize(string gameMode)
        {
            _lobbyGameMode = gameMode;
        }
        public void UpdateState(Dictionary<string, DataObject> lobbyData)
        {
            if (lobbyData.ContainsKey("GameMode"))
            {
                _lobbyGameMode = lobbyData["GameMode"].Value;
            }
            if (lobbyData.ContainsKey("RelayJoinCode"))
            {
                _relayJoinCode = lobbyData["RelayJoinCode"].Value;
            }
        }
        public Dictionary<string, string> Serialize()
        {
            return new Dictionary<string, string>()
            {
                {"GameMode",_lobbyGameMode },
                {"RelayJoinCode",_relayJoinCode }
            };
        }      
    }
}
