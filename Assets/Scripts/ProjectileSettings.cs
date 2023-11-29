using System;
using UnityEngine;

[Serializable]
public class ProjectileSettings
{
	public float Speed = 30f;
	public bool RotateTowardsTarget;
	public bool IsSpinning;
	public Vector3 SpinAxis = Vector3.up; // Default spin axis
	public float SpinSpeed = 360f; // Degrees per second
	public bool UseGravity;
}
