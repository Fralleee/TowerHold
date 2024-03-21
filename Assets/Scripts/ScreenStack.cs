using System.Collections.Generic;

public class ScreenStack
{
	readonly Stack<UIScreen> _screenStack = new Stack<UIScreen>();

	public bool IsEmpty => _screenStack.Count <= 1;

	public void PushScreen(UIScreen screen)
	{
		if (_screenStack.Count > 0)
		{
			_screenStack.Peek().Hide(false);
		}

		_screenStack.Push(screen);
		screen.Show();
	}

	public void PopScreen()
	{
		if (_screenStack.Count > 0)
		{
			_screenStack.Pop().Hide();
		}

		if (_screenStack.Count > 0)
		{
			_screenStack.Peek().Show();
		}
	}

	public void ClearStack()
	{
		while (_screenStack.Count > 1)
		{
			_screenStack.Pop().Hide();
		}

		_screenStack.Peek().Show();
	}
}
