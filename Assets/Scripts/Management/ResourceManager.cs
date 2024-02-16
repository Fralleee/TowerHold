using System;
using System.Collections;
using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
	public static Action<int> OnResourceChange = delegate { };
	public int Resources = 0;
	public int IncomeRate = 0;
	public int StartingResources = 500;
	public float PassiveIncomeRate = 50f;

	[SerializeField] FloatingText _floatingText;

	Coroutine _incomeCoroutine;
	Vector3 _defaultTextSpawnPosition;

	void Start()
	{
		_incomeCoroutine = StartCoroutine(ActivateIncomeAfterDelay(GameController.Instance.FreezeTime));
		_defaultTextSpawnPosition = Tower.Instance.transform.position + (Vector3.up * 6);
		AddResource(StartingResources);
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
			AddResource(Mathf.FloorToInt(PassiveIncomeRate + IncomeRate));
		}
	}

	public void AddIncome(float amount) => PassiveIncomeRate += amount;

	public void AddResource(int amount, Vector3? spawnPosition = null)
	{
		Resources += amount;
		OnResourceChange(Resources);
		ScoreManager.Instance.ResourcesEarned += amount;

		var position_ = spawnPosition ?? _defaultTextSpawnPosition;
		_ = _floatingText.Spawn(position_, $"+{amount}");
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
