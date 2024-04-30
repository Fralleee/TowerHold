using UnityEngine;

public abstract class Affliction : ScriptableObject, IAffliction
{
	public abstract void Trigger(Turret turret, Target target);

	public abstract void Tooltip(RichTextWithImages descriptionContainer, StyleSettings styleSettings, Turret turret);

	public abstract bool PreferNewTarget { get; }
}
