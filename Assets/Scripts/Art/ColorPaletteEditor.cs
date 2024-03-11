#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ColorPalette))]
public class ColorPaletteEditor : Editor
{
	Vector2 _scrollPosition;
	readonly string[][] _labels = new string[][]
	{
		new string[] {"Ground", "Tree", "Rock", "Wood"},
		new string[] {"Skin", "Detail", "Empty", "Empty"},
		new string[] {"Wood", "MetalDark", "MetalLight", "Gem"},
		new string[] {"Base", "Crystal", "Empty", "Empty"}
	};

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
		var fields = palette.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
		for (var j = 0; j < fields.Length; j++)
		{
			var field = fields[j];
			if (field.FieldType == typeof(ColorPaletteRow))
			{
				var row = (ColorPaletteRow)field.GetValue(palette);
				EditorGUILayout.LabelField(field.Name);
				_ = EditorGUILayout.BeginHorizontal();
				for (var i = 0; i < row.Colors.Length; i++)
				{
					_ = EditorGUILayout.BeginVertical(GUILayout.MaxWidth(70));
					row.Colors[i] = EditorGUILayout.ColorField(GUIContent.none, row.Colors[i], false, false, false, GUILayout.Width(64), GUILayout.Height(64));
					EditorGUILayout.LabelField(_labels[j][i], GUILayout.Width(64)); // Label below color field
					EditorGUILayout.EndVertical(); // End vertical group
				}
				EditorGUILayout.EndHorizontal();
				GUILayout.Space(20);
			}
		}
	}


}
#endif
