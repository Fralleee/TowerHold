using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BloodEffect : MonoBehaviour
{
	[SerializeField] float _lifetime = 10f;
	[SerializeField] float _scaleUpSpeed = 5f;

	DecalProjector _decalProjector;

	void Awake()
	{
		_decalProjector = GetComponent<DecalProjector>();

		transform.localScale = Vector3.zero;
	}

	void Start()
	{
		_ = StartCoroutine(ScaleUp());
		_ = StartCoroutine(FadeOut());
	}

	IEnumerator ScaleUp()
	{
		var scale = 0f;
		while (scale < 1f)
		{
			scale += Time.deltaTime * _scaleUpSpeed;
			transform.localScale = Vector3.one * scale;
			yield return null;
		}
	}

	IEnumerator FadeOut()
	{
		yield return new WaitForSeconds(_lifetime);

		_decalProjector.fadeFactor = 1f;
		var time = 0f;
		while (time < 1f)
		{
			time += Time.deltaTime;
			_decalProjector.fadeFactor = 1 - time;
			yield return null;
		}
		Destroy(gameObject);
	}
}
