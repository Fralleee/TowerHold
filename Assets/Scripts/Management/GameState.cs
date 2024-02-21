using UnityEngine;

public enum GameState
{
	Idle,        // Nothing is happening -- nothing is running
	Preparation, // Freeze time, player has some time to setup shop before enemies spawn
	Live,        // Game is live
	Conclusion   // Game is over -- either victory or defeat
}

static class GameStateExtensions
{
	public static Color Color(this GameState state)
	{
		return state switch
		{
			GameState.Idle => UnityEngine.Color.gray,
			GameState.Preparation => UnityEngine.Color.blue,
			GameState.Live => UnityEngine.Color.green,
			GameState.Conclusion => UnityEngine.Color.red,
			_ => UnityEngine.Color.gray
		};
	}
}
