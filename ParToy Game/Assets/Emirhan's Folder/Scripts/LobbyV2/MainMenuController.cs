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
            bool succeed = await GameLobbyManager.Instance.CreateLobby();
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
