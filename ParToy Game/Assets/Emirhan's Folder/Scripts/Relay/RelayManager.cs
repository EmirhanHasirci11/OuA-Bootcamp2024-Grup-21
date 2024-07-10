using Assets.Emirhan_s_Folder.Scripts.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class RelayManager : MonoBehaviour
{
    // Start is called before the first frame update
    private string PID;
    [SerializeField] TextMeshProUGUI IdTXT;
    [SerializeField] TextMeshProUGUI JoinCodeTXT;
    [SerializeField] TMP_Dropdown playerCountDD;
    [SerializeField] TMP_InputField HostID;
    private RelayHostData _hostData;
    private RelayJoinData _joinData;
    async void Start()
    {
        await UnityServices.InitializeAsync();
        SignIn();

    }
    async void SignIn()
    {
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        PID = AuthenticationService.Instance.PlayerId;
        IdTXT.text = PID;

    }
   public async void OnHostClick()
    {
        int maxPlayer = Convert.ToInt32(playerCountDD.options[playerCountDD.value].text);
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayer);
        _hostData = new RelayHostData()
        {
            IPv4Address = allocation.RelayServer.IpV4,
            Port = (ushort)allocation.RelayServer.Port,
            AllocationID = allocation.AllocationId,
            AllocationIdBytes = allocation.AllocationIdBytes,
            ConnectionData = allocation.ConnectionData,
            Key = allocation.Key
        };
        _hostData.JoinCode = await RelayService.Instance.GetJoinCodeAsync(_hostData.AllocationID);
        JoinCodeTXT.text = _hostData.JoinCode;
        Debug.Log("Allocate Complete : " + _hostData.AllocationID);
        Debug.Log("Join Code : " + _hostData.JoinCode);
    }
    public async void OnJoinClick()
    {
        JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(HostID.text);

        _joinData = new RelayJoinData()
        {
            IPv4Address = allocation.RelayServer.IpV4,
            Port = (ushort)allocation.RelayServer.Port,
            AllocationID = allocation.AllocationId,
            AllocationIDBytes = allocation.AllocationIdBytes,
            ConnectionData = allocation.ConnectionData,
            Key = allocation.Key
        };
        Debug.Log("Join Success:" + _joinData.AllocationID);
    }
    
    void Update()
    {

    }
}
