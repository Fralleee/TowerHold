using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ColorPalette))]
public class ColorPaletteEditor : Editor
{
	Vector2 _scrollPosition;
	public override void OnInspectorGUI()
	{
		_ = DrawDefaultInspector(); // Draw the default inspector for rows and columns

		var palette = (ColorPalette)target;

		// Ensure the Colors array is initialized and matches the specified grid dimensions
		if (palette.Colors == null || palette.Colors.Length != palette.Rows * palette.Columns)
		{
			palette.Colors = new Color[palette.Rows * palette.Columns];
		}

		_scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
		CreateColorGrid(palette);
		EditorGUILayout.EndScrollView();

		if (GUILayout.Button("Save Palette as PNG"))
		{
			palette.SavePaletteAsPNG();
		}
		if (GUILayout.Button("Import JSON Palette (Palettte.app format)"))
		{
			ImportJSONPalette();
		}
	}

	void CreateColorGrid(ColorPalette palette)
	{
		for (var y = 0; y < palette.Rows; y++)
		{
			_ = EditorGUILayout.BeginHorizontal();
			for (var x = 0; x < palette.Columns; x++)
			{
				var index = (y * palette.Columns) + x;
				palette.Colors[index] = EditorGUILayout.ColorField(GUIContent.none, palette.Colors[index], false, false, false, GUILayout.Width(48), GUILayout.Height(48));
			}
			EditorGUILayout.EndHorizontal();
		}
	}

	void ImportJSONPalette()
	{
		var filePath = EditorUtility.OpenFilePanel("Load JSON Palette", "", "json");
		if (string.IsNullOrEmpty(filePath))
		{
			return;
		}

		var jsonContent = File.ReadAllText(filePath);
		var paletteCollection = JsonUtility.FromJson<PaletteCollection>("{\"palettes\":" + jsonContent + "}");

		// Analyze the JSON to determine the required size
		var maxSwatches = 0; // Maximum number of swatches in any single palette
		paletteCollection.palettes.ForEach(palette => maxSwatches = Mathf.Max(maxSwatches, palette.swatches.Count));

		var paletteSO = (ColorPalette)target;
		paletteSO.Rows = paletteCollection.palettes.Count;
		paletteSO.Columns = maxSwatches;
		paletteSO.Colors = new Color[paletteSO.Rows * paletteSO.Columns];

		for (var paletteIndex = 0; paletteIndex < paletteCollection.palettes.Count; paletteIndex++)
		{
			var palette = paletteCollection.palettes[paletteIndex];
			for (var swatchIndex = 0; swatchIndex < maxSwatches; swatchIndex++)
			{
				var colorIndex = paletteIndex * maxSwatches + swatchIndex;
				if (swatchIndex < palette.swatches.Count)
				{
					var swatch = palette.swatches[swatchIndex];
					if (ColorUtility.TryParseHtmlString(swatch.color, out var color))
					{
						paletteSO.Colors[colorIndex] = color;
					}
					else
					{
						// Handle failed color parsing, maybe set to a default color
						paletteSO.Colors[colorIndex] = Color.clear;
					}
				}
				else
				{
					// Fill remaining cells in the row with a default color
					paletteSO.Colors[colorIndex] = Color.clear; // Or any default color
				}
			}
		}

		EditorUtility.SetDirty(target);
	}
}

[System.Serializable]
public class PaletteCollection
{
#pragma warning disable IDE1006 // Naming Styles
	public List<Palette> palettes;
#pragma warning restore IDE1006 // Naming Styles
}


[System.Serializable]
public class Palette
{
#pragma warning disable IDE1006 // Naming Styles
	public string paletteName;
	public List<Swatch> swatches;
#pragma warning restore IDE1006 // Naming Styles
}

[System.Serializable]
public class Swatch
{
#pragma warning disable IDE1006 // Naming Styles
	public string name;
	public string color;
#pragma warning restore IDE1006 // Naming Styles
}
