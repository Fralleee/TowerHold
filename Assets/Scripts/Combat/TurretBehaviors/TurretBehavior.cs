using UnityEngine;

public abstract class TurretBehavior : ScriptableObject, ITurretBehavior
{
	public abstract void Execute(Turret turret, Enemy target);

	public abstract void Tooltip(RichTextWithImages descriptionContainer, StyleSettings styleSettings, Turret turret);

	public abstract bool PreferNewTarget { get; }
}
