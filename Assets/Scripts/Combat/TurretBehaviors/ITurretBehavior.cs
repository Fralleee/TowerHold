public interface ITurretBehavior
{
	void Execute(Turret turret, Enemy target);

	void Tooltip(RichTextWithImages descriptionContainer, StyleSettings styleSettings, Turret turret);
}
