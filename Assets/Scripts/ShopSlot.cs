using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopSlot : MonoBehaviour
{
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI costText;
    public Image itemImage;
    public Image categoryImage;
    public Button purchaseButton;

    ShopItem item;

    Button button;

    void Awake()
    {
        button = GetComponent<Button>();
    }

    public void SetupSlot(ShopItem newItem)
    {
        SetUIAlpha(1f);

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
            categoryImage.color = color;
            itemImage.color = color;

            button.colors = new ColorBlock()
            {
                normalColor = new Color(color.r, color.g, color.b, 0.25f),
                highlightedColor = new Color(color.r, color.g, color.b, 0.85f),
                pressedColor = new Color(color.r, color.g, color.b, 0.5f),
                disabledColor = new Color(color.r, color.g, color.b, 0.1f),
                colorMultiplier = 1,
                fadeDuration = 0.1f
            };
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

    void SetUIAlpha(float alpha)
    {
        itemNameText.alpha = alpha;
        costText.alpha = alpha;
        itemImage.color = new Color(itemImage.color.r, itemImage.color.g, itemImage.color.b, alpha);
        categoryImage.color = new Color(categoryImage.color.r, categoryImage.color.g, categoryImage.color.b, alpha);
    }

    void DisableButton()
    {
        purchaseButton.interactable = false;
        SetUIAlpha(0.1f);
    }

    public void PurchaseItem()
    {
        if (GoldManager.Instance.SpendGold(item.cost))
        {
            item.OnPurchase();
            DisableButton();
        }
    }
}
