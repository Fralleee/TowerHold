using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugSettings : MonoBehaviour
{
	public void SetSpeed(float speed)
	{
		Debug.Log($"Setting speed to {speed}");
		Time.timeScale = speed;
	}
}
