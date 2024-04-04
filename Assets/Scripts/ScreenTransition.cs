using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenTransition : MonoBehaviour
{
	[SerializeField] float _dissolveDuration = 2f;
	float _dissolveThreshold = 1f;

	Image _image;
	Material _material;

	readonly int _threshold = Shader.PropertyToID("_Threshold");

	void Awake()
	{
		_image = GetComponentInChildren<Image>();
		_material = _image.material;
	}

	void Start()
	{
		_ = StartCoroutine(StartDissolveEffect());
	}

	IEnumerator StartDissolveEffect()
	{
		float time = 0;

		while (time < _dissolveDuration)
		{
			_dissolveThreshold = Mathf.Lerp(1f, 0f, time / _dissolveDuration);
			_material.SetFloat(_threshold, _dissolveThreshold);
			time += Time.deltaTime;
			yield return null;
		}

		_material.SetFloat(_threshold, 0f);
	}
}
