using Assets.Emirhan_s_Folder.ColerTileConquest.Scripts;
using Unity.Netcode;
using UnityEngine;

public class TileCaptureCollision : NetworkBehaviour
{
    private Player player;

    void Start()
    {
        player = GetComponentInParent<Player>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tile"))
        {
            Tile tile = other.GetComponent<Tile>();

            if (tile != null && tile.CurrentColor != player.PlayerColor)
            {
                tile.ChangeColorServerRpc(player.PlayerColor, tile.CurrentColor);
            }
        }
    }
}
