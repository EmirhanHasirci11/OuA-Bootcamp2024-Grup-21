using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetcodeMovement : NetworkBehaviour
{
    public NetworkVariable<Vector3> Position=new NetworkVariable<Vector3>();
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
            Move();
    }

    void Update()
    {
        transform.position = Position.Value;
    }
    public void Move()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            var rand = GetRandomPosition();
            transform.position = rand;
            Position.Value = rand;
        }else
        {
            SubmitPositionRequestServerRpc();
        }
    }
    [ServerRpc]
    void SubmitPositionRequestServerRpc(ServerRpcParams rpcParams = default)
    {
        Position.Value=GetRandomPosition();
    }
    public Vector3 GetRandomPosition()
    {
        return new Vector3(Random.Range(-3f, 3f),0f ,Random.Range(-3f, 3f));
    }
}
