using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using TMPro;
using UnityEngine.UI;

public class GetLobbies : MonoBehaviour
{
    [SerializeField] GameObject buttonPrefab;
    [SerializeField] GameObject buttonsContainer;
   async void Start()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }      
    public async void GetLobbiesTest()
    {
        try
        {
            ClearLobbyButtons();
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
                Debug.Log(message: "Lobby Ýsmi = " + FoundedLobby.Name + "\n" +
                                   "Lobby oluþturulma vakti = " + FoundedLobby.Created+ "\n CODE:"
                                   +FoundedLobby.LobbyCode+"\n ID:"
                                   + FoundedLobby.Id);
                CreateLobbyPrefabs(FoundedLobby);
            }            

        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }

    }

    public void CreateLobbyPrefabs(Lobby lobby)
    {
        var button = Instantiate(buttonPrefab,Vector3.zero, Quaternion.identity);
        button.name = $"{lobby.Name} Button";
        button.GetComponentInChildren<TextMeshProUGUI>().text= lobby.Name;
        var recTransform = button.GetComponent<RectTransform>();
        recTransform.SetParent(buttonsContainer.transform);
        button.GetComponent<Button>().onClick.AddListener(delegate () { LobbyClick(lobby); });
    }
    public void LobbyClick(Lobby lobby)
    {
        Debug.Log("Clicked Lobby: " + lobby.Name);
        GetComponent<JoinLobby>().JoinLobbyById(lobby.Id);

    }
    public void ClearLobbyButtons()
    {
        if(buttonsContainer is not null && buttonsContainer.transform.childCount > 0)
        {
            foreach (Transform item in buttonsContainer.transform)
            {
                Destroy(item.gameObject);
            }
        }
    }

}
