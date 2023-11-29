using TMPro;
using UnityEngine;

public class ResourceUpdateUI : MonoBehaviour
{
	TextMeshProUGUI _text;

	void Awake()
	{
		_text = GetComponent<TextMeshProUGUI>();
		ResourceManager.OnResourceChange += UpdateResources;
	}

	void UpdateResources(int resources)
	{
		Debug.Log("Updating resources: " + resources);
		_text.text = resources.ToString();
	}
}
