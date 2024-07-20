using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public ItemSO[] ownedItemsSO;
    public GameObject[] inventoryPanelsGO;
    public InventoryTemplate[] inventoryPanels;

    void Start()
    {
        // set as active the items if each is owned
        for (int i = 0; i < ownedItemsSO.Length; i++)
        {
            if (ownedItemsSO[i].isOwned)
            {
                inventoryPanelsGO[i].SetActive(true);
            }
        }

        LoadPanels();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadPanels()
    {
        for (int i = 0; i < ownedItemsSO.Length; i++)
        {
            inventoryPanels[i].titleText.text = ownedItemsSO[i].title;
            inventoryPanels[i].descriptionText.text = ownedItemsSO[i].description;
        }
    }

    public void LoadPanelsFor(string categoryName)
    {
        for (int i = 0; i < ownedItemsSO.Length; i++)
        {
            if (ownedItemsSO[i].category.Equals(categoryName) && ownedItemsSO[i].isOwned)
            {
                inventoryPanelsGO[i].SetActive(true);
                inventoryPanels[i].titleText.text = ownedItemsSO[i].title;
                inventoryPanels[i].descriptionText.text = ownedItemsSO[i].description;
            }
            else
            {
                inventoryPanelsGO[i].SetActive(false);
            }
        }
    }
}
