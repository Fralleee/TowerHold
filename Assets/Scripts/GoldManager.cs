using System;
using System.Collections;
using UnityEngine;
public class GoldManager : Singleton<GoldManager>
{
	public static Action<int> OnGoldChange = delegate { };
	public int Gold = 0;
	public int IncomeRate = 0;

	Coroutine _incomeCoroutine;

	void Start()
	{
		_incomeCoroutine = StartCoroutine(ActivateIncomeAfterDelay(GameController.Instance.FreezeTime));
		EarnGold(GameController.Instance.GoldManagerSettings.StartingGold);
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
			EarnGold(Mathf.FloorToInt(GameController.Instance.GoldManagerSettings.PassiveIncomeRate + IncomeRate));
		}
	}

	public void AddIncome(float amount) => GameController.Instance.GoldManagerSettings.PassiveIncomeRate += amount;

	public void EarnGold(int amount)
	{
		Gold += amount;
		OnGoldChange(Gold);
		ScoreManager.Instance.GoldEarned += amount;
	}

	public bool SpendGold(int amount)
	{
		if (amount <= Gold)
		{
			Gold -= amount;
			OnGoldChange(Gold);
			ScoreManager.Instance.GoldSpent += amount;
			return true;
		}

		return false;
	}

	public int GetCurrentGold() => Gold;

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

		OnGoldChange = delegate
		{ };
	}
}
