public interface ITurretBehavior
{
	void Execute(Turret turret, Target target);

	void Tooltip(RichTextWithImages descriptionContainer, StyleSettings styleSettings, Turret turret);
}
