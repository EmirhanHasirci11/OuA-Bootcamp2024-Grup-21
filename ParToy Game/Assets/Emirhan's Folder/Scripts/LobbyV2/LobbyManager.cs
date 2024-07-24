using Assets.Emirhan_s_Folder.Scripts.LobbyV2.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Assets.Emirhan_s_Folder.Scripts.LobbyV2
{
    public class LobbyManager : Singleton<LobbyManager>
    {
        private static Lobby _lobby;
        private Coroutine _hearthbeatCoroutine;
        private Coroutine _refreshLobbyCoroutine;
        public async Task<bool> CreateLobby(string lobbyName, int maxPlayer, bool isPrivate, Dictionary<string, string> data, Dictionary<string, string> lobbyData)
        {
            Dictionary<string, PlayerDataObject> playerData = SerializePlayerData(data);
            Player player = new Player(AuthenticationService.Instance.PlayerId, connectionInfo: null, playerData);
            CreateLobbyOptions options = new CreateLobbyOptions()
            {
                IsPrivate = isPrivate,
                Player = player,
                Data = SerializeLobbyData(lobbyData)
            };

            try
            {
                _lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayer, options);

            }
            catch (Exception)
            {

                return false;
            }

            _hearthbeatCoroutine = StartCoroutine(HearthbeatLobbyCoroutine(_lobby.Id, 6f));
            PeriodicallyRefreshLobby();

            Debug.Log($"Lobby creation is Succeed! ID: {_lobby.Id}");
            return true;
        }

        private Dictionary<string, DataObject> SerializeLobbyData(Dictionary<string, string> lobbyData)
        {
            Dictionary<string, DataObject> lobbyDataV = new Dictionary<string, DataObject>();
            foreach (var (key, value) in lobbyData)
            {
                lobbyDataV.Add(key, new DataObject(visibility: DataObject.VisibilityOptions.Member, value));
            }
            return lobbyDataV;
        }

        private IEnumerator HearthbeatLobbyCoroutine(string lobbyId, float waitTimeSeconds)
        {
            while (true)
            {
                Debug.Log("Heartbeat");
                LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
                yield return new WaitForSecondsRealtime(waitTimeSeconds);
            }
        }
        private static CancellationTokenSource _updateLobbySource;
        private static async void PeriodicallyRefreshLobby()
        {
            _updateLobbySource = new CancellationTokenSource();
            await Task.Delay(2 * 1000);
            while (!_updateLobbySource.IsCancellationRequested && _lobby != null)
            {
                _lobby = await Lobbies.Instance.GetLobbyAsync(_lobby.Id);
                LobbyEvents.OnLobbyUpdated?.Invoke(_lobby);
                await Task.Delay(1 * 1000);
            }
        }
        private IEnumerator RefreshLobbyCoroutine(string lobbyId, float waitTimeSeconds)
        {
            while (true)
            {
                Task<Lobby> task = LobbyService.Instance.GetLobbyAsync(lobbyId);
                yield return new WaitUntil(() => task.IsCompleted);
                Lobby newLobby = task.Result;
                if (newLobby.LastUpdated > _lobby.LastUpdated)
                {
                    _lobby = newLobby;
                    LobbyEvents.OnLobbyUpdated?.Invoke(_lobby);
                }
                yield return new WaitForSecondsRealtime(waitTimeSeconds);
            }
        }

        private Dictionary<string, PlayerDataObject> SerializePlayerData(Dictionary<string, string> data)
        {
            Dictionary<string, PlayerDataObject> playerData = new Dictionary<string, PlayerDataObject>();
            foreach (var (key, value) in data)
            {
                playerData.Add(key, new PlayerDataObject(visibility: PlayerDataObject.VisibilityOptions.Member, value));
            }
            return playerData;
        }
        public void OnApplicationQuit()
        {
            if (_lobby != null && _lobby.HostId == AuthenticationService.Instance.PlayerId)
            {
                LobbyService.Instance.DeleteLobbyAsync(_lobby.Id);
            }
        }

        public string GetLobbyCode()
        {
            return _lobby?.LobbyCode;
        }

        public async Task<bool> JoinLobby(string code, Dictionary<string, string> playerData)
        {
            JoinLobbyByCodeOptions options = new JoinLobbyByCodeOptions();
            Player player = new Player(AuthenticationService.Instance.PlayerId, connectionInfo: null, SerializePlayerData(playerData));
            options.Player = player;

            try
            {
                _lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(code, options);
            }
            catch (System.Exception)
            {
                return false;
            }

            PeriodicallyRefreshLobby();

            return true;
        }

        public List<Dictionary<string, PlayerDataObject>> GetPlayerData()
        {
            List<Dictionary<string, PlayerDataObject>> data = new List<Dictionary<string, PlayerDataObject>>();

            foreach (Player player in _lobby.Players)
            {
                data.Add(player.Data);
            }

            return data;
        }

        public async Task<bool> UpdatePlayerData(string playerId, Dictionary<string, string> data, string allocationID = "", string connectionData = "")
        {
            Dictionary<string, PlayerDataObject> playerData = SerializePlayerData(data);
            UpdatePlayerOptions options = new UpdatePlayerOptions()
            {
                Data = playerData,
                AllocationId = allocationID,
                ConnectionInfo = connectionData
            };

            try
            {
                _lobby = await LobbyService.Instance.UpdatePlayerAsync(_lobby.Id, playerId, options);
            }
            catch (System.Exception)
            {
                return false;
            }

            LobbyEvents.OnLobbyUpdated(_lobby);
            return true;
        }

        public string GetHostId()
        {

            return _lobby.HostId;
        }

        public Lobby GetLobby()
        {
            return _lobby;
        }
        public async Task<bool> UpdateLobbyData(Dictionary<string, string> data)
        {
            Dictionary<string, DataObject> lobbyData = SerializeLobbyData(data);

            UpdateLobbyOptions options = new UpdateLobbyOptions()
            {
                Data = lobbyData
            };

            try
            {
                _lobby = await LobbyService.Instance.UpdateLobbyAsync(_lobby.Id, options);

            }
            catch (System.Exception)
            {
                return false;
            }

            LobbyEvents.OnLobbyUpdated(_lobby);

            return true;
        }

        internal async Task<bool> GetLobbies(GameObject prefab, GameObject container, Dictionary<string, string> playerData)
        {
            GetLobbiesTest(prefab, container, playerData);
            return true;
        }
        public async void GetLobbiesTest(GameObject prefab, GameObject container, Dictionary<string, string> playerData)
        {
            try
            {
                ClearLobbyButtons(container);
                QueryLobbiesOptions options = new();
                Debug.Log(message: "QueryLobbiesTest");
                options.Count = 25;

                options.Filters = new List<QueryFilter>()
            {
                new QueryFilter(field:QueryFilter.FieldOptions.AvailableSlots,op:QueryFilter.OpOptions.GT,value:"0")
            };

                options.Order = new List<QueryOrder>()
            {
                new QueryOrder(asc:false,field:QueryOrder.FieldOptions.Created)
            };


                QueryResponse lobbies = await Lobbies.Instance.QueryLobbiesAsync(options);
                Debug.Log(message: "Get Lobbies Done COUNT: " + lobbies.Results.Count);
                foreach (Lobby FoundedLobby in lobbies.Results)
                {
                    Debug.Log(message: "Lobby İsmi = " + FoundedLobby.Name + "\n" +
                                       "Lobby oluşturulma vakti = " + FoundedLobby.Created + "\n CODE:"
                                       + FoundedLobby.LobbyCode + "\n ID:"
                                       + FoundedLobby.Id);
                    CreateLobbyPrefabs(FoundedLobby, prefab, container, playerData);
                }

            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }

        }

        public void CreateLobbyPrefabs(Lobby lobby, GameObject prefab, GameObject container, Dictionary<string, string> playerData)
        {
            var button = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            button.name = $"{lobby.Name} Button";
            button.GetComponentInChildren<TextMeshProUGUI>().text = lobby.Name;
            var recTransform = button.GetComponent<RectTransform>();
            recTransform.SetParent(container.transform);
            button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate () { LobbyClick(lobby, playerData); });
        }
        public async void LobbyClick(Lobby lobby, Dictionary<string, string> playerData)
        {
            Debug.Log("Clicked Lobby: " + lobby.Name);
            await LobbyManager.Instance.JoinLobby(lobby.LobbyCode, playerData);
            SceneManager.LoadScene("CurrentLobby");

        }
        public void ClearLobbyButtons(GameObject container)
        {
            if (container is not null && container.transform.childCount > 0)
            {
                foreach (Transform item in container.transform)
                {
                    Destroy(item.gameObject);
                }
            }
        }
    }
}
