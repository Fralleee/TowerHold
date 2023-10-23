using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public ShopSlot[] slots; // Assign your slots in the editor
    public Button refreshButton;
    public Image progressBar;
    public TextMeshProUGUI refreshCostText;
    public ShopItem[] availableItems;

    int refreshCost = 50; // Initial cost to refresh the shop manually

    void Start()
    {
        RefreshShop();
        refreshButton.onClick.AddListener(ManualRefresh);
        GameController.OnLevelChanged += RefreshShop;
    }

    void Update()
    {
        progressBar.fillAmount = GameController.Instance.timeLeft / GameController.Instance.levelTime;
    }

    public void RefreshShop()
    {
        // Sample logic to populate shop slots
        foreach (ShopSlot slot in slots)
        {
            ShopItem item = GetRandomItem(GameController.Instance.currentLevel);
            slot.SetupSlot(item);
        }

        refreshCostText.text = refreshCost.ToString();
    }

    ShopItem GetRandomItem(int currentLevel)
    {
        // Filter out the items that meet the level requirement
        List<ShopItem> eligibleItems = new List<ShopItem>();
        foreach (ShopItem item in availableItems)
        {
            if (item.minLevel <= currentLevel)
            {
                eligibleItems.Add(item);
            }
        }

        // If there are no eligible items, return null or handle it as you prefer
        if (eligibleItems.Count == 0)
            return null;

        // Choose a random item from the eligible items
        int randomIndex = Random.Range(0, eligibleItems.Count);
        return eligibleItems[randomIndex];
    }


    void ManualRefresh()
    {
        if (GameController.Instance.gold >= refreshCost) // Assuming you have a gold system
        {
            GameController.Instance.gold -= refreshCost;
            RefreshShop();
            refreshCost += 50; // Increase the cost for the next manual refresh
            refreshCostText.text = refreshCost.ToString();
        }
    }
}
