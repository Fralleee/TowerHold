public interface IAffliction
{
	void Trigger(Turret turret, Target target);

	void Tooltip(RichTextWithImages descriptionContainer, StyleSettings styleSettings, Turret turret);
}
