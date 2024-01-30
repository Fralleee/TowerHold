using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ColorPalette))]
public class ColorPaletteEditor : Editor
{
	Vector2 _scrollPosition;
	public override void OnInspectorGUI()
	{
		var palette = (ColorPalette)target;

		_scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
		DrawPaletteRows(palette);
		EditorGUILayout.EndScrollView();

		if (GUILayout.Button("Randomize Palette"))
		{
			palette.RandomizePalette();
			EditorUtility.SetDirty(palette);
		}

		if (GUILayout.Button("Load Palette from PNG"))
		{
			palette.LoadPaletteFromPNG(palette);
			EditorUtility.SetDirty(palette);
		}

		if (GUILayout.Button("Save Palette as PNG"))
		{
			palette.SavePaletteAsPNG();
		}
	}

	void DrawPaletteRows(ColorPalette palette)
	{
		foreach (var field in palette.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
		{
			if (field.FieldType == typeof(ColorPaletteRow))
			{
				var row = (ColorPaletteRow)field.GetValue(palette);
				EditorGUILayout.LabelField(field.Name);
				_ = EditorGUILayout.BeginHorizontal();
				for (var i = 0; i < row.Colors.Length; i++)
				{
					row.Colors[i] = EditorGUILayout.ColorField(GUIContent.none, row.Colors[i], false, false, false, GUILayout.Width(48), GUILayout.Height(48));
				}
				EditorGUILayout.EndHorizontal();
			}
		}
	}
}
