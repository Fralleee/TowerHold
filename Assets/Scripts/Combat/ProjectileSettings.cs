using System;
using Sirenix.OdinInspector;

[Serializable]
public class ProjectileSettings
{
	public Projectile ProjectilePrefab;
	public float Speed = 30f;
	public bool UseParabolicArc;
	[ShowIf("UseParabolicArc")] public float MaxArcHeight = 5f;
}
