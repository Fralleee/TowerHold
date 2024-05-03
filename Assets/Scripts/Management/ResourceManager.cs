using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
	[ReadOnly] public int Resources = 0;
	[ReadOnly] public int Income = 0;

	[SerializeField] FloatingText _floatingText;

	Coroutine _incomeCoroutine;
	Vector3 _defaultTextSpawnPosition;

	EventBinding<GameStartEvent> _gameStartEvent;
	EventBinding<GameEndEvent> _gameEndEvent;

	void OnEnable()
	{
		_gameStartEvent = new EventBinding<GameStartEvent>(e => OnGameStart());
		EventBus<GameStartEvent>.Register(_gameStartEvent);

		_gameEndEvent = new EventBinding<GameEndEvent>(e => OnGameEnd());
		EventBus<GameEndEvent>.Register(_gameEndEvent);
	}

	void OnDisable()
	{
		EventBus<GameStartEvent>.Deregister(_gameStartEvent);
		EventBus<GameEndEvent>.Deregister(_gameEndEvent);
	}

	void Start()
	{
		_defaultTextSpawnPosition = Tower.Instance.transform.position + (Vector3.up * 6);
	}

	void OnGameStart()
	{
		AddResource(GameController.GameSettings.StartingResources, _defaultTextSpawnPosition);
		AddIncome(GameController.GameSettings.StartingIncome);
		_incomeCoroutine = StartCoroutine(PassiveIncomeCoroutine());
	}


	void OnGameEnd()
	{
		if (_incomeCoroutine != null)
		{
			StopCoroutine(_incomeCoroutine);
			_incomeCoroutine = null;
		}
	}

	IEnumerator PassiveIncomeCoroutine()
	{
		while (true)
		{
			yield return new WaitForSeconds(1f);
			AddResourceSilent(Mathf.FloorToInt(Income));
		}
	}

	public void AddIncome(int amount)
	{
		Income += amount;
		EventBus<IncomeChangedEvent>.Raise(new IncomeChangedEvent { CurrentIncome = Income });
	}

	public void AddResourceSilent(int amount)
	{
		Resources += amount;
		EventBus<ResourceChangedEvent>.Raise(new ResourceChangedEvent { CurrentResources = Resources });
		ScoreManager.Instance.ResourcesEarned += amount;
	}

	public void AddResource(int amount, Vector3 spawnPosition)
	{
		AddResourceSilent(amount);
		_ = _floatingText.Spawn(spawnPosition, $"+{amount}");
	}

	public bool SpendResources(int amount)
	{
		if (amount <= Resources)
		{
			Resources -= amount;
			EventBus<ResourceChangedEvent>.Raise(new ResourceChangedEvent { CurrentResources = Resources });
			ScoreManager.Instance.ResourcesSpent += amount;
			return true;
		}

		return false;
	}
}
