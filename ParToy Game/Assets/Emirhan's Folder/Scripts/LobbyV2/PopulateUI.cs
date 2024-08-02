using Assets.Emirhan_s_Folder.Scripts.LobbyV2;
using Assets.Emirhan_s_Folder.Scripts.LobbyV2.Core;
using Assets.Emirhan_s_Folder.Scripts.LobbyV2.Game;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class PopulateUI : MonoBehaviour
{
    private CurrentLobby _currentLobby;
    [SerializeField] TextMeshProUGUI lobbyName;
    [SerializeField] TextMeshProUGUI lobbyCode;
    [SerializeField] TextMeshProUGUI gameMode;
    [SerializeField] TextMeshProUGUI playerCount;
    [SerializeField] TextMeshProUGUI privateState;
    [SerializeField] GameObject playerContainer;
    [SerializeField] GameObject playerInfoPrefab;
    [SerializeField] private Button _readyButton;
    [SerializeField] private Button _startButton;
    private string _gameMode;
    private string lobbyID;

    private void OnEnable()
    {
        _readyButton.onClick.AddListener(OnReadyPressed);
        if (GameLobbyManager.Instance.IsHost)
        {
            GameLobbyEvents.OnLobbyReady += OnLobbyReady;
            _startButton.onClick.AddListener(OnStartButtonClicked);
        }
        GameLobbyEvents.OnLobbyUpdated += OnLobbyUpdated;
    }

    private async void OnStartButtonClicked()
    {
        await GameLobbyManager.Instance.StartGame();
    }

    private void OnLobbyReady()
    {
        _startButton.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        _readyButton.onClick.RemoveListener(OnReadyPressed);
        _startButton.onClick.RemoveListener(OnStartButtonClicked);
        GameLobbyEvents.OnLobbyReady -= OnLobbyReady;
        GameLobbyEvents.OnLobbyUpdated -= OnLobbyUpdated;
    }

    private void OnLobbyUpdated()
    {
       _gameMode= GameLobbyManager.Instance.GetMinigame();
        UpdateMinigame();

    }
    private void UpdateMinigame()
    {
        gameMode.text = _gameMode;
    }

    private async void OnReadyPressed()
    {
        bool succeed = await GameLobbyManager.Instance.SetPlayerReady();
        if (succeed)
        {
            _readyButton.gameObject.SetActive(false);
        }
    }
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
        lobbyCode.text = $"Lobby Code: {GameLobbyManager.Instance.GetLobbyCode()}";
        lobbyName.text = $"{LobbyManager.Instance.GetLobby().Name}";
        playerCount.text = $"Max Player: :{LobbyManager.Instance.GetLobby().MaxPlayers}";
        privateState.text = $"Private Lobby: {LobbyManager.Instance.GetLobby().IsPrivate}";
       
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
