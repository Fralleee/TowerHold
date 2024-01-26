using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "ColorPalette", menuName = "Color Palette")]
public class ColorPalette : ScriptableObject
{
	[HideInInspector] public Color[] Colors;
	public int Rows = 4; // Number of rows in the grid
	public int Columns = 6; // Number of columns in the grid

	[ContextMenu("Save Palette as PNG")]
	public void SavePaletteAsPNG()
	{
		var texture = new Texture2D(Columns, Rows, TextureFormat.RGB24, false);

		for (var i = 0; i < Colors.Length; i++)
		{
			var x = i % Columns;
			var y = i / Columns;
			texture.SetPixel(x, Rows - 1 - y, Colors[i]); // Invert the y-coordinate to correct the orientation
		}

		texture.Apply(); // Apply all SetPixel changes

		var bytes = texture.EncodeToPNG();
		DestroyImmediate(texture); // Clean up the texture to avoid memory leak

		var path = EditorUtility.SaveFilePanelInProject("Save Palette as PNG", "ColorPalette", "png", "Please enter a file name to save the color palette to");
		if (!string.IsNullOrEmpty(path))
		{
			System.IO.File.WriteAllBytes(path, bytes);
			AssetDatabase.Refresh();
			Debug.Log("Palette saved as PNG at " + path);
		}
	}
}
