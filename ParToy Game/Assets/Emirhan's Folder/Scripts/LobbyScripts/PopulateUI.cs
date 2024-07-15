using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class PopulateUI : MonoBehaviour
{
    private CurrentLobby _currentLobby;
    [SerializeField] TextMeshProUGUI lobbyName;
    [SerializeField] TextMeshProUGUI lobbyCode;
    [SerializeField] TextMeshProUGUI gameMode;
    [SerializeField] GameObject playerContainer;
    [SerializeField] GameObject playerInfoPrefab;
    private string lobbyID;

    public void PopulateUIElement()
    {
        ClearContainer();
        lobbyName.text=_currentLobby.currentLobby.Name;
        lobbyCode.text=_currentLobby.currentLobby.LobbyCode;
        gameMode.text = _currentLobby.currentLobby.Data["GameMode"].Value;
        foreach (Player player in _currentLobby.currentLobby.Players)
        {
            InstantiatePlayerInfo(player);   
        }
    }
    void Start()
    {
        _currentLobby = GameObject.Find("LobbyManager").GetComponent<CurrentLobby>();   
        PopulateUIElement();
        lobbyID = _currentLobby.currentLobby.Id;
        InvokeRepeating(nameof(PollforLobbyUpdate), 1.1f, 2f);
    }

    void InstantiatePlayerInfo(Player player)
    {
        var text = Instantiate(playerInfoPrefab, Vector3.zero, Quaternion.identity);
        text.name = player.Joined.ToShortTimeString();
        text.GetComponent<TextMeshProUGUI>().text = player.Id + " : " + player.Data["TotalScore"].Value;
        var rectTransform = text.GetComponent<RectTransform>();
        rectTransform.SetParent(playerContainer.transform);
    }
    async void PollforLobbyUpdate()
    {
        _currentLobby.currentLobby= await LobbyService.Instance.GetLobbyAsync(lobbyID);
        PopulateUIElement();
    }
    private void ClearContainer()
    {
        if(playerContainer is not null && playerContainer.transform.childCount>0) 
        {
            foreach (Transform item in playerContainer.transform)
            {
                Destroy(item.gameObject);
            }
        }
    }
}
