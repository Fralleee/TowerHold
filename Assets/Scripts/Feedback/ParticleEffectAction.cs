using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "VAKT/Feedback/ParticleEffect")]
public class ParticleEffectAction : FeedbackAction
{
	[SerializeField] ParticleSystem _particleEffectPrefab;

	public override IEnumerator Execute(Transform target)
	{
		if (target != null && _particleEffectPrefab != null)
		{
			var spawnedEffect = Instantiate(_particleEffectPrefab, target.position, Quaternion.identity);
			yield return new WaitForSeconds(spawnedEffect.main.duration);
			Destroy(spawnedEffect.gameObject);
		}
	}
}
