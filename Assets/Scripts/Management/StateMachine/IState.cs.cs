public interface IState<out T>
{
	T Identifier { get; }
	void OnEnter();
	void OnLogic();
	void OnExit();
}
