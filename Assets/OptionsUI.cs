using UnityEngine;
using UnityEngine.UIElements;

public class OptionsUI : MonoBehaviour
{
	UIDocument _uiDocument;
	DropdownField _qualityDropdown;

	void Awake()
	{
		_uiDocument = GetComponent<UIDocument>();
		_qualityDropdown = _uiDocument.rootVisualElement.Q<DropdownField>("QualityDropdown");

		InitializeQualityDropdown();
	}

	void InitializeQualityDropdown()
	{
		_qualityDropdown.choices.Clear();
		foreach (var qualityName in QualitySettings.names)
		{
			_qualityDropdown.choices.Add(qualityName);
		}

		_qualityDropdown.value = QualitySettings.names[QualitySettings.GetQualityLevel()];
		_ = _qualityDropdown.RegisterValueChangedCallback(QualityChanged);
	}

	void QualityChanged(ChangeEvent<string> evt)
	{
		var qualityIndex = System.Array.IndexOf(QualitySettings.names, evt.newValue);
		if (qualityIndex >= 0)
		{
			QualitySettings.SetQualityLevel(qualityIndex, true);
		}
	}
}
