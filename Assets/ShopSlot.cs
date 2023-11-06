using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopSlot : MonoBehaviour
{
    public TextMeshProUGUI itemNameText;
    public Image itemImage;
    public TextMeshProUGUI costText;
    public Button purchaseButton;

    ShopItem item;

    public void SetupSlot(ShopItem newItem)
    {
        item = newItem;
        itemNameText.text = item.itemName;
        itemImage.sprite = item.image;
        costText.text = item.cost.ToString();
        purchaseButton.interactable = true;
        purchaseButton.onClick.AddListener(PurchaseItem);
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    void DisableButton()
    {

        purchaseButton.interactable = false; // Disable the button after purchase
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    void PurchaseItem()
    {
        if (GoldManager.Instance.SpendGold(item.cost))
        {
            item.OnPurchase(); // Apply the item      
            DisableButton();
        }
    }
}
