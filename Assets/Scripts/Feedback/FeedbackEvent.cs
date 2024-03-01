using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "VAKT/Feedback/FeedbackEvent")]
public class FeedbackEvent : ScriptableObject
{
	public List<FeedbackAction> Actions;

	public void TriggerFeedback(Transform target, MonoBehaviour caller)
	{
		foreach (var action in Actions)
		{
			_ = caller.StartCoroutine(action.Execute(target));
		}
	}
}
