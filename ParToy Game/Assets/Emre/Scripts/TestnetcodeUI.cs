using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
public class TestnetcodeUI : MonoBehaviour
{
   [SerializeField] private Button startHostButton;
   [SerializeField] private Button startClientButton;

   private void Awake()
   {
        startHostButton.onClick.AddListener( 
            () =>
            {
                NetworkManager.Singleton.StartHost();
            });



        startClientButton.onClick.AddListener(
             () =>
             {
                 NetworkManager.Singleton.StartClient();
             });
    }
}
