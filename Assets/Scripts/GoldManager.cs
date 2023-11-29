using System;
using System.Collections;
using UnityEngine;
public class ResourceManager : Singleton<ResourceManager>
{
	public static Action<int> OnResourceChange = delegate { };
	public int Resources = 0;
	public int IncomeRate = 0;

	Coroutine _incomeCoroutine;

	void Start()
	{
		_incomeCoroutine = StartCoroutine(ActivateIncomeAfterDelay(GameController.Instance.FreezeTime));
		AddResource(GameController.Instance.ResourceManagerSettings.StartingResources);
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
			AddResource(Mathf.FloorToInt(GameController.Instance.ResourceManagerSettings.PassiveIncomeRate + IncomeRate));
		}
	}

	public void AddIncome(float amount) => GameController.Instance.ResourceManagerSettings.PassiveIncomeRate += amount;

	public void AddResource(int amount)
	{
		Resources += amount;
		OnResourceChange(Resources);
		ScoreManager.Instance.ResourcesEarned += amount;
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
