using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class CreateLobby : MonoBehaviour
{
    [SerializeField] TMP_InputField lobbyName;
    [SerializeField] TMP_Dropdown maxPlayers;
    [SerializeField] TMP_Dropdown gameMode;
    [SerializeField] Toggle isLobbyPrivate;
    [SerializeField] TextMeshProUGUI LobbyCodeView;

    void Start()
    {

    }
    public async void CreateLobbyFunc()
    {
        DontDestroyOnLoad(this);
        string lobbyNameText = lobbyName.text;
        int maxPlayerCount = Convert.ToInt32(maxPlayers.options[maxPlayers.value].text);
        CreateLobbyOptions opts = new CreateLobbyOptions();
        opts.IsPrivate = isLobbyPrivate.isOn;
        opts.Player = new Player(AuthenticationService.Instance.PlayerId);
        opts.Player.Data = new Dictionary<string, PlayerDataObject>()
            {
                {"TotalScore",new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member,"0") },
                {"Id",new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member,opts.Player.Id) },
                {"GamerTag",new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member,"HostPlayer") },
                {"isReady",new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member,"False") }
            };

        opts.Data = new Dictionary<string, DataObject>()
        {
            {
                "GameMode",new DataObject(DataObject.VisibilityOptions.Public,gameMode.options[gameMode.value].text,DataObject.IndexOptions.S1)
            }
        };


        Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyNameText, maxPlayerCount, opts);
        GetComponent<CurrentLobby>().currentLobby = lobby;
        Debug.Log("Lobby creation is succedd!");
        LobbyCodeView.text = lobby.LobbyCode;

        StartCoroutine(HeartbeatLobbyCoroutine(lobby.Id, 15f));
        LobbyStatic.LoadLobbyRoom();
    }

    IEnumerator HeartbeatLobbyCoroutine(string lobbyId, float waitTimeSeconds)
    {
        var delay = new WaitForSeconds(waitTimeSeconds);
        while (true)
        {
            LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return delay; ;
        }

    }
    void LogPlayers(Lobby lby)
    {
        foreach (var player in lby.Players)
        {
            Debug.Log($"Player ID= {player.Id}");
        }
    }
}
