using UnityEngine.UIElements;

public class EnemyInformation : VisualElement
{
	public Image Image;

	public VisualElement Container;
	public Label Name;

	public VisualElement StatsContainer;

	public VisualElement DefenseContainer;
	public RichTextWithImages Health;
	public RichTextWithImages Armor;
	public RichTextWithImages MagicResistance;

	public VisualElement OffenseContainer;
	public RichTextWithImages Damage;
	public RichTextWithImages AttacksPerSecond;
	public RichTextWithImages AttackRange;

	public EnemyInformation()
	{
		Image = new Image();
		Image.AddToClassList("image");
		Add(Image);

		Container = new VisualElement();
		Container.AddToClassList("container");
		Add(Container);

		Name = new Label();
		Name.AddToClassList("name");
		Container.Add(Name);

		StatsContainer = new VisualElement();
		StatsContainer.AddToClassList("stats-container");
		Container.Add(StatsContainer);

		DefenseContainer = new VisualElement();
		DefenseContainer.AddToClassList("defense-container");
		StatsContainer.Add(DefenseContainer);

		Health = new RichTextWithImages();
		Health.AddToClassList("row");
		DefenseContainer.Add(Health);

		Armor = new RichTextWithImages();
		Armor.AddToClassList("row");
		DefenseContainer.Add(Armor);

		MagicResistance = new RichTextWithImages();
		MagicResistance.AddToClassList("row");
		DefenseContainer.Add(MagicResistance);

		OffenseContainer = new VisualElement();
		OffenseContainer.AddToClassList("offense-container");
		StatsContainer.Add(OffenseContainer);

		Damage = new RichTextWithImages();
		Damage.AddToClassList("row");
		OffenseContainer.Add(Damage);

		AttacksPerSecond = new RichTextWithImages();
		AttacksPerSecond.AddToClassList("row");
		OffenseContainer.Add(AttacksPerSecond);

		AttackRange = new RichTextWithImages();
		AttackRange.AddToClassList("row");
		OffenseContainer.Add(AttackRange);
	}

	public void Setup(Enemy enemy = null)
	{
		if (enemy == null)
		{
			// Image.sprite = enemy.Image;
			Name.text = "Unknown";
			return;
		}

		// Image.sprite = enemy.Image;
		Name.text = enemy.name;

		Health.Clear();
		Health.TextAndIcon($"{enemy.Health}", GameIcons.Health);

		Damage.Clear();
		Armor.TextAndIcon($"{enemy.Armor}", GameIcons.Armor);

		MagicResistance.Clear();
		MagicResistance.TextAndIcon($"{enemy.MagicResistance}", GameIcons.MagicResistance);

		var attackInformation = enemy.GetAttackInformation();

		Damage.Clear();
		Damage.TextAndIcon($"{attackInformation.Damage}", attackInformation.DamageType.AsIcon());

		AttacksPerSecond.Clear();
		AttacksPerSecond.TextAndIcon($"{attackInformation.AttacksPerSecond}", GameIcons.AttacksPerSecond);

		AttackRange.Clear();
		AttackRange.TextAndIcon($"{attackInformation.AttackRange.AsShortUI()}", GameIcons.AttackRange);
	}
}
