using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
	public static Action<int> OnResourceChange = delegate { };
	public static Action<int> OnIncomeChange = delegate { };

	[ReadOnly] public int Resources = 0;
	[ReadOnly] public int Income = 0;

	[SerializeField] FloatingText _floatingText;

	Coroutine _incomeCoroutine;
	Vector3 _defaultTextSpawnPosition;

	protected override void Awake()
	{
		base.Awake();

		GameController.OnGameStart += OnGameStart;
		GameController.OnGameEnd += OnGameEnd;
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
		OnIncomeChange(Mathf.FloorToInt(Income));
	}

	public void AddResourceSilent(int amount)
	{
		Resources += amount;
		OnResourceChange(Resources);
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
			OnResourceChange(Resources);
			ScoreManager.Instance.ResourcesSpent += amount;
			return true;
		}

		return false;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		OnResourceChange = delegate
		{ };

		OnIncomeChange = delegate
		{ };

		GameController.OnGameStart -= OnGameStart;
		GameController.OnGameEnd -= OnGameEnd;
	}
}
