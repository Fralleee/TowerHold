using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "VAKT/Feedback/SquashStretch")]
public class SquashStretchAction : FeedbackAction
{
	[SerializeField]
	AnimationCurve _squashAndStretchCurve = new AnimationCurve(
		new Keyframe(0, 0),
		new Keyframe(0.25f, 1f), // Overshoot
		new Keyframe(0.5f, -0.25f),  // Squash
		new Keyframe(0.75f, 0.1f), // Overshoot in the opposite direction
		new Keyframe(1, 0));       // Return to original scale
	[SerializeField] float _duration = 0.4f;
	[SerializeField] float _strength = 1.1f;

	public override IEnumerator Execute(Transform target)
	{
		if (target != null)
		{
			var originalScale = target.localScale;
			var targetScale = originalScale * _strength;

			var time = 0f;
			while (time < _duration)
			{
				target.localScale = Vector3.Lerp(originalScale, targetScale, _squashAndStretchCurve.Evaluate(time / _duration));
				time += Time.deltaTime;
				yield return null;
			}

			target.localScale = originalScale;
		}
	}
}
