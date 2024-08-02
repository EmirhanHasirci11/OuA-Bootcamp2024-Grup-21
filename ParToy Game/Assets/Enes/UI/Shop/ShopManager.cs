using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public int coins;
    public TMP_Text coinUI;
    public ItemSO[] shopItemsSO;
    public GameObject[] shopPanelsGO;
    public ShopTemplate[] shopPanels;
    public Button[] myPurchaseBtns;

    void Start()
    {
        // set as active the items if each is not owned
        for (int i = 0; i < shopItemsSO.Length; i++)
        {
            if (!shopItemsSO[i].isOwned)
            {
                shopPanelsGO[i].SetActive(true);
            }
        }

        coinUI.text = "Coins: " + coins.ToString();

        LoadPanels();
        CheckPurchaseable();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddCoins()
    {
        coins++;
        coinUI.text = "Coins: " + coins.ToString();
        CheckPurchaseable();
    }

    // purchase button interactiveness
    public void CheckPurchaseable()
    {
        for (int i = 0; i < shopItemsSO.Length; i++)
        {
            if (coins >= shopItemsSO[i].price) 
                myPurchaseBtns[i].interactable = true;
            else
                myPurchaseBtns[i].interactable = false;
        }
    }

    public void PurchaseItem(int btnNo)
    {
        if (coins >= shopItemsSO[btnNo].price)
        {
            // decrease total amount of coins
            coins = coins - shopItemsSO[btnNo].price;
            coinUI.text = "Coins: " + coins.ToString();

            // own item and remove  it from the shop list
            shopItemsSO[btnNo].isOwned = true;
            shopPanelsGO[btnNo].SetActive(false);

            CheckPurchaseable();
        }
    }

    public void LoadPanels()
    {
        for (int i = 0; i < shopItemsSO.Length; i++)
        {
            shopPanels[i].titleText.text = shopItemsSO[i].title;
            shopPanels[i].descriptionText.text = shopItemsSO[i].description;
            shopPanels[i].priceText.text = "Coins: " + shopItemsSO[i].price.ToString();
        }
    }

    public void LoadPanelsFor(string categoryName)
    {
        for (int i = 0; i < shopItemsSO.Length; i++)
        {
            if (shopItemsSO[i].category.Equals(categoryName) && !shopItemsSO[i].isOwned)
            {
                shopPanelsGO[i].SetActive(true);
                shopPanels[i].titleText.text = shopItemsSO[i].title;
                shopPanels[i].descriptionText.text = shopItemsSO[i].description;
                shopPanels[i].priceText.text = "Coins: " + shopItemsSO[i].price.ToString();
            }
            else
            {
                shopPanelsGO[i].SetActive(false);
            }
        }
    }
}
