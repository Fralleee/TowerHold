using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopSlot : MonoBehaviour
{
    public TextMeshProUGUI itemNameText;
    public Image itemImage;
    public TextMeshProUGUI costText;
    public Button purchaseButton;

    private ShopItem item;

    public void SetupSlot(ShopItem newItem)
    {
        item = newItem;
        itemNameText.text = item.itemName;
        itemImage.sprite = item.image;
        costText.text = item.cost.ToString();
        purchaseButton.interactable = true;
        purchaseButton.onClick.AddListener(PurchaseItem);
    }

    private void PurchaseItem()
    {
        if (GameController.Instance.gold >= item.cost)
        {
            GameController.Instance.gold -= item.cost;
            purchaseButton.interactable = false; // Disable the button after purchase
            item.OnPurchase(); // Apply the item            
        }
    }
}
