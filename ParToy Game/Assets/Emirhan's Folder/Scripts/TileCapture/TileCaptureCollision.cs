using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileCaptureCollision : MonoBehaviour
{
    private Material playerColorMaterial;

    void Start()
    {

        Transform playerColorTransform = transform.Find("PlayerColor");

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
                Debug.Log(renderer.material);
                Debug.Log(playerColorMaterial);
            }
        }
    }
}
