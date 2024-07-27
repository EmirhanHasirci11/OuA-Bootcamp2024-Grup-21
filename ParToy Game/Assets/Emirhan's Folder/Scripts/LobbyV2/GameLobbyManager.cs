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
    private LobbyData _lobbyData;
    private int _maxNumberOfPlayer = 8;
    string mapName = "";
    private bool inGame=false;
    private Dictionary<int, Color> _colors;
    public bool IsHost => _localLobbyPlayerData.Id == LobbyManager.Instance.GetHostId();
    private void OnEnable()
    {
        _colors = new Dictionary<int, Color>();
        _colors.Add(0, Color.red);
        _colors.Add(1, Color.yellow);
        _colors.Add(2, Color.blue);
        _colors.Add(3, Color.green);
        _colors.Add(4, Color.magenta);
        _colors.Add(5, Color.cyan);
        _colors.Add(6, Color.grey);
        _colors.Add(7, Color.black);
        LobbyEvents.OnLobbyUpdated += OnLobbyUpdated;
    }

    private void OnDisable()
    {
        LobbyEvents.OnLobbyUpdated -= OnLobbyUpdated;
    }   
    public async Task<bool> CreateLobby(string lobbyName,bool isPrivate,int maxPlayer, string gameMode)
    {
        _maxNumberOfPlayer=maxPlayer;
        _localLobbyPlayerData = new LobbyPlayerData();
        _localLobbyPlayerData.Initialize(AuthenticationService.Instance.PlayerId, "HostPlayer");
        _lobbyData = new LobbyData();
        _lobbyData.Initialize(gameMode);
        bool succeeded = await LobbyManager.Instance.CreateLobby(lobbyName,_maxNumberOfPlayer, isPrivate, _localLobbyPlayerData.Serialize(),_lobbyData.Serialize());
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
        int index = 0;
        foreach (Dictionary<string, PlayerDataObject> data in playerData)
        {
            data["Color"]= new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, _colors[index].ToString());
            data["LobbyColor"] = new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, _colors[index++].ToString());
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
            if(index==_colors.Count)
                index = 0;
            _lobbyPlayerDatas.Add(lobbyPlayerData);
        }

        _lobbyData=new LobbyData();
        _lobbyData.Initialize(lobby.Data);

        GameLobbyEvents.OnLobbyUpdated?.Invoke();

        if (readyLobbyPlayerCount == lobby.Players.Count)
        {
            GameLobbyEvents.OnLobbyReady?.Invoke();
        }
        if (_lobbyData.RelayJoinCode != default && !inGame)
        {
            await JoinRelayServer(_lobbyData.RelayJoinCode);
            await SceneManager.LoadSceneAsync("ColorTileConquest");
        }       
    }

    private async Task<bool> JoinRelayServer(string value)
    {
        await RelayManager.Instance.JoinRelay(value);
        inGame= true;
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
        inGame= true;
        string allocationID = RelayManager.Instance.GetAllocationId();
        string connectionData = RelayManager.Instance.GetConnectionData();
        _lobbyData.RelayJoinCode=joinRelayCode;
        await LobbyManager.Instance.UpdateLobbyData(_lobbyData.Serialize());

        await LobbyManager.Instance.UpdatePlayerData(_localLobbyPlayerData.Id, _localLobbyPlayerData.Serialize(), allocationID, connectionData);

        await SceneManager.LoadSceneAsync("ColorTileConquest");
    }

    internal string GetMinigame()
    {
        return _lobbyData.LobbyGameMode;
    }
}
