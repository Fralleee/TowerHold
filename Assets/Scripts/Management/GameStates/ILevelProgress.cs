public interface ILevelProgress
{
	int CurrentLevel { get; }
	float TimeLeft { get; }
	float TotalTime { get; }
	float LevelProgress { get; }
}
