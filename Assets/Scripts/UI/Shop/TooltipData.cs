public class TooltipData : VisualElement
{
    protected Label nameLabel;
    protected Label costLabel;
    protected Label descriptionLabel;

    public TooltipData()
    {
        // Initialize common UI elements
        nameLabel = new Label();
        costLabel = new Label();
        descriptionLabel = new Label();

        // Add to the VisualElement
        Add(nameLabel);
        Add(costLabel);
        Add(descriptionLabel);

        // Set class names for styling if needed
        nameLabel.AddToClassList("item-name");
        costLabel.AddToClassList("item-cost");
        descriptionLabel.AddToClassList("item-description");
    }

    public virtual void UpdateInformation(ShopItem item)
    {
        nameLabel.text = item.Name;
        costLabel.text = $"Cost: {item.Cost}";
        descriptionLabel.text = item.Description;
    }
}
