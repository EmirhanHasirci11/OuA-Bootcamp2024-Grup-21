using Unity.Netcode;
using UnityEngine;

public class MyCustomNetworkManager : NetworkManager
{
    private void Start()
    {
        OnClientConnectedCallback += HandleClientConnected;
        OnClientDisconnectCallback += HandleClientDisconnected;
    }

    private void HandleClientConnected(ulong clientId)
    {
        if (IsServer && IsClient && clientId == LocalClientId)
        {
            SpawnPlayerServerRpc(clientId);
        }

        // Custom logic can go here
        // Base event callback is not called because it doesn't exist as a method to be called.
    }

    [ServerRpc]
    private void SpawnPlayerServerRpc(ulong clientId, ServerRpcParams serverRpcParams = default)
    {
        if (IsServer)
        {
            FindObjectOfType<SpawnManager>().HandleClientConnected(clientId);
        }
    }

    private void HandleClientDisconnected(ulong clientId)
    {
        if (IsServer)
        {
            Debug.Log($"Client disconnected from server with ID: {clientId}");
            FindObjectOfType<SpawnManager>().HandleClientDisconnected(clientId);
        }

        // Optional: Only uncomment this if you have additional logic in the
        // base class's disconnect handler that you need to execute:
        // base.OnClientDisconnectCallback(clientId);
    }
}
