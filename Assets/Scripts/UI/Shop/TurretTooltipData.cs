public class TurretTooltipData : TooltipData
{
    private Label damageTypeLabel;
    private Label damageLabel;
    private Label dpsLabel;
    private Label attackCooldownLabel;
    private Label rangeLabel;

    public TurretTooltipData() : base()
    {
        damageTypeLabel = new Label();
        damageLabel = new Label();
        dpsLabel = new Label();
        attackCooldownLabel = new Label();
        rangeLabel = new Label();

        Add(damageTypeLabel);
        Add(damageLabel);
        Add(dpsLabel);
        Add(attackCooldownLabel);
        Add(rangeLabel);

        // Set class names for styling if needed
        damageTypeLabel.AddToClassList("item-damage-type");
        // Add class names for other labels similarly
    }

    public override void UpdateInformation(ShopItem item)
    {
        base.UpdateInformation(item); // Call the base method to update common information

        if (item is Turret turret)
        {
            // Update turret-specific information
            damageTypeLabel.text = $"Damage Type: {turret.DamageType}";
            damageLabel.text = $"Damage: {turret.Damage}";
            dpsLabel.text = $"DPS: {turret.DPS}";
            attackCooldownLabel.text = $"Cooldown: {turret.AttackCooldown}";
            rangeLabel.text = $"Range: {turret.Range}";
        }
    }
}
