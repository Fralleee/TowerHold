using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Spellbook : MonoBehaviour
{

    [SerializeField] Spell[] spells;
    [SerializeField] Button buttonPrefab;

    void Awake()
    {
        foreach (var spell in spells)
        {
            var button = Instantiate(buttonPrefab, transform);
            button.GetComponentInChildren<TextMeshProUGUI>().text = spell.name;
            button.onClick.AddListener(spell.Perform);
            spell.button = button;
        }
    }
}
