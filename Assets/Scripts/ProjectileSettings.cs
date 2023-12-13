using System;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class ProjectileSettings
{
	public float Speed = 30f;
	public bool IsSpinning;
	[ShowIf("IsSpinning")] public Vector3 SpinAxis = Vector3.up; // Default spin axis
	[ShowIf("IsSpinning")] public float SpinSpeed = 360f; // Degrees per second
	public bool UseTrajectory;
	[ShowIf("UseTrajectory")] public float ArcHeight = 5f;
}
