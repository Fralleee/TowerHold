using TMPro;
using UnityEngine;

public class ResourceUpdateUI : MonoBehaviour
{
	TextMeshProUGUI _text;

	void Awake()
	{
		_text = GetComponent<TextMeshProUGUI>();
		GoldManager.OnGoldChange += UpdateGold;
	}

	void UpdateGold(int gold) => _text.text = gold.ToString();
}
