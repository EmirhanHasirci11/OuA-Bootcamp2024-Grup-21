using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;

public class LobbyPlayerData : MonoBehaviour
{
    private string _id;
    private string _gamertag;
    private bool _isReady;
    private Color _color;

    public string Id => _id;
    public string Gamertag => _gamertag;
    public Color Color => _color;
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
            string Body_Color = playerData["Color"].Value;
            string[] rgba = Body_Color.Substring(5, Body_Color.Length - 6).Split(", ");
            Color color = new Color(float.Parse(rgba[0]), float.Parse(rgba[1]), float.Parse(rgba[2]), float.Parse(rgba[3]));
            _color = color;
            Debug.Log(_color.ToString());
        }
    }

    public Dictionary<string, string> Serialize()
    {
        return new Dictionary<string, string>()
        {
            { "Id", _id },
            { "Gamertag", _gamertag },
            { "IsReady", _isReady.ToString() },
            {"Color",_color.ToString() }
        };
    }
}
