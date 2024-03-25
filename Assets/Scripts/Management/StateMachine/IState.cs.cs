public interface IState<out T>
{
	T Identifier { get; }
	void OnEnter();
	void OnUpdate();
	void OnExit();
}
