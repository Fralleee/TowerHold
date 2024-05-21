using UnityEngine.UIElements;

[UxmlElement]
public partial class Tooltip : VisualElement
{
	public VisualElement Border;
	public VisualElement Content;
	public VisualElement Data;
	public VisualElement Shadow;
	public VisualElement Image;
	public VisualElement Sparkling;

	public Tooltip()
	{
		AddToClassList("tooltip-container");

		Border = new VisualElement();
		Border.AddToClassList("border");
		Add(Border);

		Content = new VisualElement();
		Content.AddToClassList("content");
		Add(Content);

		Shadow = new VisualElement();
		Shadow.AddToClassList("shadow");
		Content.Add(Shadow);

		Sparkling = new VisualElement();
		Sparkling.AddToClassList("sparkling");
		Content.Add(Sparkling);

		Data = new VisualElement();
		Data.AddToClassList("data");
		Content.Add(Data);
	}

	public void AddData(VisualElement content)
	{
		Data.Clear();
		Data.Add(content);
	}
}
