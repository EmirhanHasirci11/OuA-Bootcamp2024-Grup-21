using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Emirhan_s_Folder.Scripts.LobbyV2.Game
{
    public static class GameLobbyEvents
    {
        public delegate void LobbyUpdated();
        public static LobbyUpdated OnLobbyUpdated;

        public delegate void LobbyReady();
        public static LobbyReady OnLobbyReady;
    }
}
