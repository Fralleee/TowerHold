using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenTransition : MonoBehaviour
{

	public float dissolveDuration = 2f;
	private float dissolveThreshold = 1f; // Start fully obscured

	Image _image;
	Material _material; // Assign your dissolve shader material here

	void Awake()
	{
		_image = GetComponentInChildren<Image>();
		_material = _image.material;
	}

	void Start()
	{
		StartCoroutine(StartDissolveEffect());
	}

	IEnumerator StartDissolveEffect()
	{
		float time = 0;

		while (time < dissolveDuration)
		{
			dissolveThreshold = Mathf.Lerp(1f, 0f, time / dissolveDuration);
			_material.SetFloat("_Threshold", dissolveThreshold);
			time += Time.deltaTime;
			yield return null;
		}

		_material.SetFloat("_Threshold", 0f); // Ensure fully revealed
	}
}
