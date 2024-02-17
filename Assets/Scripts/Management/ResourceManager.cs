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

	void Start()
	{
		_incomeCoroutine = StartCoroutine(ActivateIncomeAfterDelay(GameController.GameSettings.FreezeTime));
		_defaultTextSpawnPosition = Tower.Instance.transform.position + (Vector3.up * 6);
		AddResource(GameController.GameSettings.StartingResources, _defaultTextSpawnPosition);
		AddIncome(GameController.GameSettings.StartingIncome);
	}

	IEnumerator ActivateIncomeAfterDelay(float delay)
	{
		yield return new WaitForSeconds(delay);
		_incomeCoroutine = StartCoroutine(PassiveIncomeCoroutine());
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
		Resources += amount;
		OnResourceChange(Resources);
		ScoreManager.Instance.ResourcesEarned += amount;

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

	public void StopIncome()
	{
		if (_incomeCoroutine != null)
		{
			StopCoroutine(_incomeCoroutine);
			_incomeCoroutine = null;
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		OnResourceChange = delegate
		{ };
	}
}
