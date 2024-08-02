using Assets.Emirhan_s_Folder.Scripts.LobbyV2.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbySpawner : MonoBehaviour
{
    [SerializeField] private List<LobbyPlayer> _players;

    private void OnEnable()
    {
        GameLobbyEvents.OnLobbyUpdated += OnLobbyUpdated;
    }

    private void OnDisable()
    {
        GameLobbyEvents.OnLobbyUpdated -= OnLobbyUpdated;
    }

    private void OnLobbyUpdated()
    {
        List<LobbyPlayerData> playerDatas = GameLobbyManager.Instance.GetPlayers();

        for (int i = 0; i < playerDatas.Count; i++)
        {
            LobbyPlayerData data = playerDatas[i];
            _players[i].SetData(data);
        }
    }
}
