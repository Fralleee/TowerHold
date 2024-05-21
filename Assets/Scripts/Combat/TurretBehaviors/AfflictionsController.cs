using System;
using System.Linq;
using Sirenix.OdinInspector;

[Serializable, InlineProperty, HideLabel]
public class AfflictionsController
{
	[ListDrawerSettings(DefaultExpandedState = true, ShowFoldout = true), InlineEditor(InlineEditorModes.GUIOnly)]
	public Affliction[] Afflictions;

	public bool PreferNewTarget => Afflictions.Any(affliction => affliction.PreferNewTarget);

	public void TriggerAfflictions(Target target, Turret turret, Affliction excludedAffliction = null)
	{
		foreach (var affliction in Afflictions)
		{
			if (affliction == excludedAffliction)
			{
				continue;
			}
			affliction.Trigger(turret, target);
		}
	}

	public void Tooltip(RichTextWithImages descriptionContainer, Turret turret)
	{
		foreach (var affliction in Afflictions)
		{
			affliction.Tooltip(descriptionContainer, turret);
		}
	}
}
