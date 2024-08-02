using Assets.Emirhan_s_Folder.ColerTileConquest.Scripts;
using Unity.Netcode;
using UnityEngine;

public class Tile : NetworkBehaviour
{
    [SerializeField] private Renderer tileRenderer;

    public Color CurrentColor
    {
        get => tileRenderer.material.color;
        private set => tileRenderer.material.color = value;
    }

    [ServerRpc(RequireOwnership = false)]
    public void ChangeColorServerRpc(Color newColor, Color oldColor)
    {
        CurrentColor = newColor;
        ChangeColorClientRpc(newColor);

        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { NetworkManager.Singleton.LocalClientId }
            }
        };

        // Tile rengini değiştiren oyuncuya bildirim gönder
        UpdatePlayerScoreClientRpc(newColor, oldColor, clientRpcParams);
    }

    [ClientRpc]
    private void ChangeColorClientRpc(Color newColor)
    {
        CurrentColor = newColor;
    }

    [ClientRpc]
    private void UpdatePlayerScoreClientRpc(Color newColor, Color oldColor, ClientRpcParams clientRpcParams = default)
    {
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            player.CaptureTileServerRpc(true, oldColor);
        }
    }
}
