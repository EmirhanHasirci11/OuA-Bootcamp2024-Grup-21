using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Threading.Tasks;
using Assets.Emirhan_s_Folder.Scripts.LobbyV2.Core;
using Assets.Emirhan_s_Folder.Scripts.LobbyV2;
using Assets.Emirhan_s_Folder.Scripts.LobbyV2.Game;
using UnityEngine.SceneManagement;

public class GameLobbyManager : Singleton<GameLobbyManager>
{
    private List<LobbyPlayerData> _lobbyPlayerDatas = new List<LobbyPlayerData>();
    private LobbyPlayerData _localLobbyPlayerData;
    private string relayCode = "";
    private int _maxNumberOfPlayer = 8;
    string mapName = "";
    public bool IsHost => _localLobbyPlayerData.Id == LobbyManager.Instance.GetHostId();
    private void OnEnable()
    {
        LobbyEvents.OnLobbyUpdated += OnLobbyUpdated;
    }

    private void OnDisable()
    {
        LobbyEvents.OnLobbyUpdated -= OnLobbyUpdated;
    }
    public async Task<bool> CreateLobby()
    {
        _localLobbyPlayerData = new LobbyPlayerData();
        _localLobbyPlayerData.Initialize(AuthenticationService.Instance.PlayerId, "HostPlayer");
        bool succeeded = await LobbyManager.Instance.CreateLobby(_maxNumberOfPlayer, false, _localLobbyPlayerData.Serialize());
        return succeeded;
    }
    public string GetLobbyCode()
    {
        return LobbyManager.Instance.GetLobbyCode();
    }

    public async Task<bool> JoinLobby(string code)
    {
        _localLobbyPlayerData = new LobbyPlayerData();
        _localLobbyPlayerData.Initialize(AuthenticationService.Instance.PlayerId, "JoinPlayer");
        bool succeeded = await LobbyManager.Instance.JoinLobby(code, _localLobbyPlayerData.Serialize());
        return succeeded;
    }
    private async void OnLobbyUpdated(Lobby lobby)
    {
        List<Dictionary<string, PlayerDataObject>> playerData = LobbyManager.Instance.GetPlayerData();
        _lobbyPlayerDatas.Clear();

        int readyLobbyPlayerCount = 0;

        foreach (Dictionary<string, PlayerDataObject> data in playerData)
        {
            LobbyPlayerData lobbyPlayerData = new LobbyPlayerData();
            lobbyPlayerData.Initialize(data);

            if (lobbyPlayerData.Id == AuthenticationService.Instance.PlayerId)
            {
                _localLobbyPlayerData = lobbyPlayerData;
            }
            if (lobbyPlayerData.IsReady)
            {
                readyLobbyPlayerCount++;
            }

            _lobbyPlayerDatas.Add(lobbyPlayerData);
        }
        GameLobbyEvents.OnLobbyUpdated?.Invoke();

        if (readyLobbyPlayerCount == lobby.Players.Count)
        {
            GameLobbyEvents.OnLobbyReady?.Invoke();
        }
        if (relayCode != "")
        {
            await JoinRelayServer(relayCode);
            if (mapName!= "ColorTileConquest")
            {
                mapName = "ColorTileConquest";
                SceneManager.LoadSceneAsync("ColorTileConquest");
            }
        }
    }

    private async Task<bool> JoinRelayServer(string value)
    {
        await RelayManager.Instance.JoinRelay(value);
        string allocationID = RelayManager.Instance.GetAllocationId();
        string connectionData = RelayManager.Instance.GetConnectionData();

        await LobbyManager.Instance.UpdatePlayerData(_localLobbyPlayerData.Id, _localLobbyPlayerData.Serialize(), allocationID, connectionData);
        return true;
    }

    public List<LobbyPlayerData> GetPlayers()
    {
        return _lobbyPlayerDatas;
    }

    public async Task<bool> SetPlayerReady()
    {
        _localLobbyPlayerData.IsReady = true;
        return await LobbyManager.Instance.UpdatePlayerData(_localLobbyPlayerData.Id, _localLobbyPlayerData.Serialize());
    }

    internal async Task StartGame()
    {
        string joinRelayCode = await RelayManager.Instance.CreateRelay(_maxNumberOfPlayer);
        Lobby loby = LobbyManager.Instance.GetLobby();
        loby.Data["RelayJoinCode"] = (new DataObject(DataObject.VisibilityOptions.Member, joinRelayCode));
        relayCode = joinRelayCode;

        string allocationID = RelayManager.Instance.GetAllocationId();
        string connectionData = RelayManager.Instance.GetConnectionData();

        await LobbyManager.Instance.UpdatePlayerData(_localLobbyPlayerData.Id, _localLobbyPlayerData.Serialize(), allocationID, connectionData);

        await SceneManager.LoadSceneAsync("ColorTileConquest");
    }
}
