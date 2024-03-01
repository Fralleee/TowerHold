using System.Collections;
using UnityEngine;

public abstract class FeedbackAction : ScriptableObject
{
	public abstract IEnumerator Execute(Transform target);
}
