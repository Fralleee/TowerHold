#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

public class PaletteGenerator : EditorWindow
{
	Color[,] _colors;
	int _gridSize = 6;

	[MenuItem("Tools/Color Palette Generator")]
	public static void ShowWindow()
	{
		GetWindow<PaletteGenerator>("Color Palette Generator").minSize = new Vector2(300, 200);
	}

	void OnGUI()
	{
		_gridSize = EditorGUILayout.IntField("Grid Size", _gridSize);
		_gridSize = Mathf.Max(1, _gridSize);

		// Initialize colors array if necessary
		if (_colors == null || _colors.GetLength(0) != _gridSize)
		{
			_colors = new Color[_gridSize, _gridSize];
		}

		for (var y = 0; y < _gridSize; y++)
		{
			EditorGUILayout.BeginHorizontal();
			for (var x = 0; x < _gridSize; x++)
			{
				_colors[x, y] = EditorGUILayout.ColorField(_colors[x, y], GUILayout.Width(64), GUILayout.Height(32), GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
			}
			EditorGUILayout.EndHorizontal();
		}

		if (GUILayout.Button("Generate Palette"))
		{
			GeneratePalette();
		}
		if (GUILayout.Button("Save Palette Settings"))
		{
			SavePaletteSettings();
		}

		if (GUILayout.Button("Load Palette Settings"))
		{
			LoadPaletteSettings();
		}

		if (GUILayout.Button("Load Image as Palette"))
		{
			LoadImageAsPalette();
		}
	}

	void SavePaletteSettings()
	{
		var path = EditorUtility.SaveFilePanel("Save Palette Settings", Application.dataPath, "PaletteSettings", "palette");
		if (!string.IsNullOrEmpty(path))
		{
			var colorDataArray = ConvertColorsToArray(_colors);
			var json = JsonUtility.ToJson(new ColorDataArray { Colors = colorDataArray });
			File.WriteAllText(path, json);
			Debug.Log($"Palette settings saved to {path}");
		}
	}
	void LoadPaletteSettings()
	{
		var path = EditorUtility.OpenFilePanel("Load Palette Settings", Application.dataPath, "palette");
		if (!string.IsNullOrEmpty(path))
		{
			var json = File.ReadAllText(path);
			var colorDataArray = JsonUtility.FromJson<ColorDataArray>(json);
			_colors = ConvertArrayToColors(colorDataArray.Colors);
			_gridSize = Mathf.RoundToInt(Mathf.Sqrt(colorDataArray.Colors.Length));
			Debug.Log($"Palette settings loaded from {path}");
		}
	}

	void LoadImageAsPalette()
	{
		var path = EditorUtility.OpenFilePanel("Load Image", Application.dataPath, "png");
		if (!string.IsNullOrEmpty(path))
		{
			var fileData = File.ReadAllBytes(path);
			var texture = new Texture2D(2, 2);
			if (texture.LoadImage(fileData))
			{
				_gridSize = texture.width; // Assuming the texture is square for simplicity
				_colors = new Color[_gridSize, _gridSize];

				// Adjusted loop to invert the y-coordinate
				for (var y = 0; y < _gridSize; y++)
				{
					for (var x = 0; x < _gridSize; x++)
					{
						// Invert the y-coordinate when assigning colors
						_colors[x, _gridSize - 1 - y] = texture.GetPixel(x, y);
					}
				}
				Debug.Log($"Image loaded as palette from {path}");
			}
			else
			{
				Debug.LogError("Failed to load image as texture");
			}
		}
	}


	Color[,] ConvertArrayToColors(ColorData[] colorDataArray)
	{
		var size = Mathf.RoundToInt(Mathf.Sqrt(colorDataArray.Length));
		var colors = new Color[size, size];
		for (var i = 0; i < colorDataArray.Length; i++)
		{
			var x = i % size;
			var y = i / size;
			colors[x, y] = colorDataArray[i].Color;
		}
		return colors;
	}

	ColorData[] ConvertColorsToArray(Color[,] colors)
	{
		var colorDataArray = new ColorData[colors.GetLength(0) * colors.GetLength(1)];
		for (var y = 0; y < colors.GetLength(1); y++)
		{
			for (var x = 0; x < colors.GetLength(0); x++)
			{
				colorDataArray[y * colors.GetLength(0) + x] = new ColorData { Color = colors[x, y] };
			}
		}
		return colorDataArray;
	}

	void GeneratePalette()
	{
		var texture = new Texture2D(_gridSize, _gridSize);

		for (var y = 0; y < _gridSize; y++)
		{
			for (var x = 0; x < _gridSize; x++)
			{
				texture.SetPixel(x, _gridSize - 1 - y, _colors[x, y]);
			}
		}

		texture.Apply();

		// Encode texture into PNG
		var bytes = texture.EncodeToPNG();
		DestroyImmediate(texture); // Clean up the temporary texture


		var defaultPath = Path.Combine(Application.dataPath, "ColorPalette.png");
		var path = EditorUtility.SaveFilePanel("Save Palette", Path.GetDirectoryName(defaultPath), Path.GetFileName(defaultPath), "png");
		if (!string.IsNullOrEmpty(path))
		{
			File.WriteAllBytes(path, bytes);
			AssetDatabase.Refresh(); // Refresh the AssetDatabase to show the new file

			Debug.Log($"Palette saved to {path}");
		}
	}
}

[System.Serializable]
public class ColorDataArray
{
	public ColorData[] Colors;
}

[System.Serializable]
public class ColorData
{
	public Color Color;
}

#endif
