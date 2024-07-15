using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class JoinLobby : MonoBehaviour
{
    [SerializeField] TMP_InputField lobbyCodeField;
    public async void JoinLobbyByCode()
    {
        var lobbyCode = lobbyCodeField.text;
        try
        {
            DontDestroyOnLoad(this);

            JoinLobbyByCodeOptions options = new JoinLobbyByCodeOptions();
            options.Player = new Player(AuthenticationService.Instance.PlayerId);
            options.Player.Data = new Dictionary<string, PlayerDataObject>()
            {
                {"TotalScore",new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member,"0") }
            };
            Lobby lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, options);
            GetComponent<CurrentLobby>().currentLobby = lobby;
            Debug.Log("Joined lobby by code! " + lobbyCode);
            LobbyStatic.LoadLobbyRoom();
        }
        catch (LobbyServiceException e)
        {

            Debug.LogError(e);
        }
    }
    public async void JoinLobbyById(string lobbyID)
    {

        try
        {
            DontDestroyOnLoad(this);
            JoinLobbyByIdOptions options = new JoinLobbyByIdOptions();
            options.Player = new Player(AuthenticationService.Instance.PlayerId);
            options.Player.Data = new Dictionary<string, PlayerDataObject>()
            {
                {"TotalScore",new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member,"0") }
            };
            Lobby lobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyID, options);
            GetComponent<CurrentLobby>().currentLobby = lobby;
            Debug.Log("Joined lobby by ID! " + lobbyID);
            Debug.Log("Joined LobbyCode is : " + lobby.LobbyCode);
            LobbyStatic.LoadLobbyRoom();
        }
        catch (LobbyServiceException e)
        {

            Debug.LogError(e);
        }
    }
    public async void QuickJoinFunc()
    {
        try
        {
            DontDestroyOnLoad(this);
            Lobby lobby = await LobbyService.Instance.QuickJoinLobbyAsync();
            GetComponent<CurrentLobby>().currentLobby = lobby;
            Debug.Log("Joined lobby by QuickJoin " + lobby.Id);
            Debug.Log("Joined LobbyCode is : " + lobby.LobbyCode);
            LobbyStatic.LoadLobbyRoom();
        }

        catch (LobbyServiceException e)
        {

            throw;
        }
    }
}
