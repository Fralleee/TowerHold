#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "ColorPalette", menuName = "Color Palette")]
public class ColorPalette : ScriptableObject
{
	public ColorPaletteRow Terrain = new ColorPaletteRow();
	public ColorPaletteRow Trees = new ColorPaletteRow();
	public ColorPaletteRow Rocks = new ColorPaletteRow();
	public ColorPaletteRow SkinAndHair = new ColorPaletteRow();
	public ColorPaletteRow Cloth = new ColorPaletteRow();
	public ColorPaletteRow Wood = new ColorPaletteRow();
	public ColorPaletteRow Metal = new ColorPaletteRow();
	public ColorPaletteRow Gems = new ColorPaletteRow();

	[ContextMenu("Randomize Palette")]
	public void RandomizePalette()
	{
		foreach (var field in GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
		{
			if (field.FieldType == typeof(ColorPaletteRow))
			{
				var row = (ColorPaletteRow)field.GetValue(this);
				for (var i = 0; i < row.Colors.Length; i++)
				{
					row.Colors[i] = Random.ColorHSV();
				}
			}
		}
	}

	[ContextMenu("Load Palette from PNG")]
	public void LoadPaletteFromPNG(ColorPalette palette)
	{
		var path = EditorUtility.OpenFilePanel("Load PNG Palette", "", "png");
		if (string.IsNullOrEmpty(path))
		{
			return;
		}

		var fileData = System.IO.File.ReadAllBytes(path);
		var texture = new Texture2D(2, 2, TextureFormat.RGB24, false);
		if (texture.LoadImage(fileData))
		{
			var width = Mathf.Min(8, texture.width);
			var height = Mathf.Min(8, texture.height);

			ColorPaletteRow[] rows = { palette.Terrain, palette.Trees, palette.Rocks, palette.SkinAndHair, palette.Cloth, palette.Wood, palette.Metal, palette.Gems };

			for (var rowIndex = 0; rowIndex < height; rowIndex++)
			{
				var row = rows[rowIndex];
				for (var colorIndex = 0; colorIndex < width; colorIndex++)
				{
					var color = texture.GetPixel(colorIndex, texture.height - 1 - rowIndex);
					row.Colors[colorIndex] = color;
				}
			}
			Debug.Log("Palette loaded from PNG");
		}
		else
		{
			Debug.LogError("Failed to load image from path: " + path);
		}

		DestroyImmediate(texture);
	}

	[ContextMenu("Save Palette as PNG")]
	public void SavePaletteAsPNG()
	{
		var width = 8;
		var height = 8;
		var texture = new Texture2D(width, height, TextureFormat.RGB24, false);

		var rows = new ColorPaletteRow[] { Terrain, Trees, Rocks, SkinAndHair, Cloth, Wood, Metal, Gems };
		for (var rowIndex = 0; rowIndex < rows.Length; rowIndex++)
		{
			var row = rows[rowIndex];
			for (var colorIndex = 0; colorIndex < row.Colors.Length; colorIndex++)
			{
				texture.SetPixel(colorIndex, height - 1 - rowIndex, row.Colors[colorIndex]);
			}
		}
		texture.Apply();

		var bytes = texture.EncodeToPNG();
		DestroyImmediate(texture);

		var path = EditorUtility.SaveFilePanelInProject("Save Palette as PNG", "ColorPalette", "png", "Please enter a file name to save the color palette to");
		if (!string.IsNullOrEmpty(path))
		{
			System.IO.File.WriteAllBytes(path, bytes);
			AssetDatabase.Refresh();
			Debug.Log("Palette saved as PNG at " + path);
		}
	}
}
#endif
