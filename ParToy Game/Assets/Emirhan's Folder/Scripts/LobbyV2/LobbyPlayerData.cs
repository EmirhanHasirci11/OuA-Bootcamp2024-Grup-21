using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyPlayerData : NetworkBehaviour
{
    private string _id;
    private string _gamertag;
    private bool _isReady;
    private NetworkVariable<Color> _color = new NetworkVariable<Color>();
    private Dictionary<string, PlayerDataObject> playerDataRet;
    public Dictionary<string, PlayerDataObject> PlayerDataRet => playerDataRet;
    public string Id => _id;
    public string Gamertag => _gamertag;
    public NetworkVariable<Color> Color => _color;
    private Color _lobbyColor;
    public Color LobbyColor => _lobbyColor;
    public bool IsReady
    {
        get => _isReady;
        set => _isReady = value;
    }

    public void Initialize(string id, string gamertag)
    {
        _id = id;
        _gamertag = gamertag;
    }

    public void Initialize(Dictionary<string, PlayerDataObject> playerData)
    {
        playerDataRet = playerData;
        UpdateState(playerData);
    }

    public void UpdateState(Dictionary<string, PlayerDataObject> playerData)
    {
        if (playerData.ContainsKey("Id"))
        {
            _id = playerData["Id"].Value;
        }
        if (playerData.ContainsKey("Gamertag"))
        {
            _gamertag = playerData["Gamertag"].Value;
        }
        if (playerData.ContainsKey("IsReady"))
        {
            _isReady = playerData["IsReady"].Value == "True";
        }
        if (playerData.ContainsKey("Color"))
        {
            string bodyColor = playerData["Color"].Value;
            string[] rgba = bodyColor.Substring(5, bodyColor.Length - 6).Split(", ");
            Color color = new Color(float.Parse(rgba[0]), float.Parse(rgba[1]), float.Parse(rgba[2]), float.Parse(rgba[3]));
            _color.Value = color;
        }
        if (playerData.ContainsKey("LobbyColor"))
        {
            string bodyColor = playerData["LobbyColor"].Value;
            string[] rgba = bodyColor.Substring(5, bodyColor.Length - 6).Split(", ");
            Color color = new Color(float.Parse(rgba[0]), float.Parse(rgba[1]), float.Parse(rgba[2]), float.Parse(rgba[3]));
            _lobbyColor = color;
        }
        playerDataRet = playerData;
    }

    public Dictionary<string, string> Serialize()
    {
        return new Dictionary<string, string>()
        {
            { "Id", _id },
            { "Gamertag", _gamertag },
            { "IsReady", _isReady.ToString() },
            { "Color", _color.ToString() },
            { "LobbyColor", _lobbyColor.ToString() },
        };
    }
}
