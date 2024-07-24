using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Emirhan_s_Folder.Scripts.LobbyV2
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _joinLobbyCode;
        [SerializeField] private Button _HostButton;
        [SerializeField] private Button _JoinButton;
        [SerializeField] TMP_InputField lobbyName;
        [SerializeField] TMP_Dropdown maxPlayers;
        [SerializeField] TMP_Dropdown gameMode;
        [SerializeField] Toggle isLobbyPrivate;
        private void OnEnable()
        {
            _HostButton.onClick.AddListener(OnHostClicked);
            _JoinButton.onClick.AddListener(OnJoinClicked);
        }
        private void OnDisable()
        {
            _HostButton.onClick.RemoveListener(OnHostClicked);
            _JoinButton.onClick.RemoveListener(OnJoinClicked);
        }

        private async void OnHostClicked()
        {
            int maxPlayerCount = Convert.ToInt32(maxPlayers.options[maxPlayers.value].text);
            bool succeed = await GameLobbyManager.Instance.CreateLobby(lobbyName.text,isLobbyPrivate.isOn,maxPlayerCount,gameMode.options[gameMode.value].text);
            if (succeed)
            {
                SceneManager.LoadScene("CurrentLobby");
            }
        }
        private async void OnJoinClicked()
        {
            bool succeed = await GameLobbyManager.Instance.JoinLobby(_joinLobbyCode.text.Trim());

            if (succeed)
            {
                SceneManager.LoadScene("CurrentLobby");
            }
        }

    }
}
