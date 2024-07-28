using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Emirhan_s_Folder.ColerTileConquest.Scripts
{
    public class Tile : NetworkBehaviour
    {
        [SerializeField] private Renderer tileRenderer;

        public Color CurrentColor { get; private set; }

    
    }
}
