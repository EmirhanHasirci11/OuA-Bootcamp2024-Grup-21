using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TileCaptureCollision : NetworkBehaviour
{
    private Material playerColorMaterial;

    void Start()
    {

        Transform playerColorTransform = transform.Find("PlayerColorObject");

        if (playerColorTransform != null)
        {
            playerColorMaterial = playerColorTransform.GetComponent<Renderer>().material;
        Debug.Log(playerColorMaterial.color);
        }
        else
        {
            Debug.LogError("PlayerColor child GameObject not found!");
        }
    }
  
    private void OnTriggerEnter(Collider other)
    {        
        if (other.gameObject.CompareTag("Tile"))
        {
            Renderer renderer = other.gameObject.GetComponent<Renderer>();
            if (renderer.material != playerColorMaterial)
            {
                renderer.material = playerColorMaterial;              
            }
        }
    }
}
