using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class ButtonAction : MonoBehaviour
{
    private NetworkManager NetworkManager;
    public TextMeshProUGUI UIText;
    void Start()
    {
        NetworkManager = GetComponentInParent<NetworkManager>();
    }
    public void StartHost()
    {
        NetworkManager.StartHost();
        InitMovementText();
    }
    public void StartClient()
    {
        NetworkManager.StartClient();
        InitMovementText();

    }
    public void SubmitNewPosition()
    {
        var pObject= NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
        var p = pObject.GetComponent<NetcodeMovement>();
        p.Move();
    }
    private void InitMovementText()
    {
        if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
            UIText.text = "MOVE";
        else if (NetworkManager.Singleton.IsClient)
            UIText.text = "REQ MOVE";

    }

}
