using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;

[Serializable, InlineProperty]
public class Afflictions
{
	[ListDrawerSettings(DefaultExpandedState = true, ShowFoldout = true)]
	public List<Affliction> AfflictionList = new List<Affliction>();

	readonly Turret _turret;

	public Afflictions(Turret turret)
	{
		_turret = turret;
	}

	public bool PreferNewTarget => AfflictionList.Any(behavior => behavior.PreferNewTarget);

	public void TriggerAfflictions(Target target, Affliction excludedAffliction = null)
	{
		foreach (var affliction in AfflictionList)
		{
			if (affliction == excludedAffliction)
			{
				continue;
			}
			affliction.Trigger(_turret, target);
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
