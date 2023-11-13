using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourceUpdateUI : MonoBehaviour
{
    TextMeshProUGUI text;

    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        GoldManager.OnGoldChange += UpdateGold;
    }

    private void UpdateGold(int gold)
    {
        text.text = gold.ToString();
    }
}
