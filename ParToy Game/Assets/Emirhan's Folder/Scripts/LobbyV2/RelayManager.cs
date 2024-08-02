using Assets.Emirhan_s_Folder.Scripts.LobbyV2.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using UnityEngine.InputSystem;

namespace Assets.Emirhan_s_Folder.Scripts.LobbyV2
{
    public class RelayManager: Singleton<RelayManager>
    {
        private bool _isHost;
        private string _joinCode;
        private string _ip;
        private int _port;
        private byte[] _connectionData;
        private byte[] _key;
        private byte[] _hostConnectionData;
        private byte[] _allocationIdBytes;
        private System.Guid _allocationId;
        public bool IsHost
        {
            get{ return _isHost; }
        }

        public async Task<string> CreateRelay(int maxConnection)
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnection);
            _joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            RelayServerEndpoint dtlsEndpoint = allocation.ServerEndpoints.First(conn => conn.ConnectionType == "dtls");
            _ip = dtlsEndpoint.Host;
            _port = dtlsEndpoint.Port;

            _allocationId = allocation.AllocationId;
            _connectionData = allocation.ConnectionData;
            _key = allocation.Key;
            _allocationIdBytes= allocation.AllocationIdBytes;

            _isHost = true;
            return _joinCode;
        }
        public async Task<bool> JoinRelay(string joinCode)
        {
            _joinCode = joinCode;
            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            RelayServerEndpoint dtlsEndpoint = allocation.ServerEndpoints.First(conn => conn.ConnectionType == "dtls");
            _ip = dtlsEndpoint.Host;
            _port = dtlsEndpoint.Port;
            _allocationId = allocation.AllocationId;
            _connectionData = allocation.ConnectionData;

            _key = allocation.Key;
            _allocationIdBytes = allocation.AllocationIdBytes;
            _hostConnectionData= allocation.HostConnectionData;

            return true;
        }

        public string GetAllocationId()
        {
            return _allocationId.ToString();
        }

        public string GetConnectionData()
        {
            return _connectionData.ToString();
        }
        public (byte[] AllocationId, byte[] Key, byte[] ConnectionData, string _dtlsAddress, int _dtlsPort) GetHostConnectionInfo()
        {
            return (_allocationIdBytes, _key, _connectionData, _ip, _port);
        }

        public (byte[] AllocationId, byte[] Key, byte[] ConnectionData, byte[] HostConnectionData, string _dtlsAddress, int _dtlsPort) GetClientConnectionInfo()
        {
            return (_allocationIdBytes, _key, _connectionData, _hostConnectionData, _ip, _port);
        }
    }
}
