using System;
using System.Linq;
using Sirenix.OdinInspector;

[Serializable, InlineProperty]
public class Afflictions
{
	[ListDrawerSettings(DefaultExpandedState = true, ShowFoldout = true)]
	public Affliction[] AfflictionList;

	public bool PreferNewTarget => AfflictionList.Any(affliction => affliction.PreferNewTarget);

	public void TriggerAfflictions(Target target, Turret turret, Affliction excludedAffliction = null)
	{
		foreach (var affliction in AfflictionList)
		{
			if (affliction == excludedAffliction)
			{
				continue;
			}
			affliction.Trigger(turret, target);
		}
	}

	public void Tooltip(RichTextWithImages descriptionContainer, StyleSettings styleSettings, Turret turret)
	{
		foreach (var affliction in AfflictionList)
		{
			affliction.Tooltip(descriptionContainer, styleSettings, turret);
		}
	}
}
