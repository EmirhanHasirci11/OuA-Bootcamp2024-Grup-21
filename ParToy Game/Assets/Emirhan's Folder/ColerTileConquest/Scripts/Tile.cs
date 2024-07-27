using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Emirhan_s_Folder.ColerTileConquest.Scripts
{
    public class Tile : NetworkBehaviour
    {
        [SerializeField] private Renderer tileRenderer;

        public Color CurrentColor { get; private set; }

        [ServerRpc(RequireOwnership = false)]
        public void ColorTileServerRpc(Color newColor, ulong playerId)
        {
            if (IsServer)
            {
                ColorTileInternal(newColor);
            }
        }

        private void ColorTileInternal(Color newColor)
        {
            // Update color counts
            var currentCounts = Player.ColorTracker.ColorCounts.Value;
            if (currentCounts == null)
            {
                currentCounts = new Dictionary<Color, int>();
            }

            if (currentCounts.ContainsKey(CurrentColor))
            {
                currentCounts[CurrentColor] -= 1;
            }
            if (currentCounts.ContainsKey(newColor))
            {
                currentCounts[newColor] += 1;
                Debug.Log(currentCounts[newColor]);
            }
            else
            {
                currentCounts.Add(newColor, 1);
            }
            Player.ColorTracker.ColorCounts.Value = currentCounts;

            CurrentColor = newColor;
            tileRenderer.material.color = CurrentColor;
        }
    }
}
