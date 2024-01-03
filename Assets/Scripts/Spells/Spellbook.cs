using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Spellbook : MonoBehaviour
{
	[SerializeField] Spell[] _spells;
	[SerializeField] Button _buttonPrefab;

	void Awake()
	{
		foreach (var spell in _spells)
		{
			var button = Instantiate(_buttonPrefab, transform);
			button.GetComponentInChildren<TextMeshProUGUI>().text = spell.name;
			button.onClick.AddListener(spell.Perform);
			spell.Button = button;
		}
	}
}
