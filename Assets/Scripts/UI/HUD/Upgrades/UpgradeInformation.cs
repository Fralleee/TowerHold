using UnityEngine.UIElements;

public class UpgradeInformation
{
	int _count;
	readonly Label _label;

	public UpgradeInformation(Label label)
	{
		_count = 0;
		_label = label;
		_label.text = "0";
	}

	public void IncrementNumber()
	{
		_count++;
		_label.text = _count.ToString();
	}
}
