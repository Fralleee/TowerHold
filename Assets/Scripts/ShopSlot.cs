using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopSlot : MonoBehaviour
{
    public TextMeshProUGUI itemNameText;
    public Image itemImage;
    public Image categoryImage;
    public TextMeshProUGUI costText;
    public Button purchaseButton;

    ShopItem item;

    Image image;

    void Awake()
    {
        image = GetComponent<Image>();
    }

    public void SetupSlot(ShopItem newItem)
    {
        item = newItem;
        itemNameText.text = item.itemName;
        itemImage.sprite = item.image;

        if (UIManager.Instance.categorySprites.TryGetValue(item.category, out Sprite sprite))
        {
            categoryImage.sprite = sprite;
        }
        else
        {
            Debug.LogError("No sprite found for category: " + item.category);
        }

        if (UIManager.Instance.categoryColors.TryGetValue(item.category, out Color color))
        {
            image.color = new Color(color.r, color.g, color.b, 0.5f);
            categoryImage.color = color;
            itemImage.color = color;
        }
        else
        {
            Debug.LogError("No color found for category: " + item.category);
        }

        costText.text = item.cost.ToString();
        purchaseButton.interactable = true;
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

    public void PurchaseItem()
    {
        if (GoldManager.Instance.SpendGold(item.cost))
        {
            item.OnPurchase(); // Apply the item      
            DisableButton();
        }
    }
}
