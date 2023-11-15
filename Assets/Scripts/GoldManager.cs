using System;
using System.Collections;
using UnityEngine;
public class GoldManager : Singleton<GoldManager>
{
    public static Action<int> OnGoldChange = delegate { };
    Coroutine incomeCoroutine;
    public int gold = 0;

    public int IncomeRate = 0;

    void Start()
    {
        // Delay the start of passive income generation
        incomeCoroutine = StartCoroutine(ActivateIncomeAfterDelay(GameController.Instance.freezeTime));
        EarnGold(GameController.Instance.goldManagerSettings.startingGold);
    }

    // Consider separating the start of the coroutine from the method waiting for delay.
    // This adds clarity and avoids starting a coroutine within another coroutine, which can be hard to manage.
    IEnumerator ActivateIncomeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        incomeCoroutine = StartCoroutine(PassiveIncomeCoroutine());
    }

    // The coroutine name is good as it describes its purpose.
    // However, consider what should happen if the game is paused or if the game ends. Should this coroutine stop?
    IEnumerator PassiveIncomeCoroutine()
    {
        // If you need to stop income on game end, you can add a condition here.
        while (true)
        {
            yield return new WaitForSeconds(1f);
            EarnGold(Mathf.FloorToInt(GameController.Instance.goldManagerSettings.passiveIncomeRate + IncomeRate));
            // Consider updating some UI element here to reflect the change in gold.
        }
    }

    // The method name 'AddIncome' is clear, but it might be mistaken for a method that directly adds gold.
    // Consider renaming it to 'IncreaseIncomeRate' or 'UpgradeIncomeRate' for clarity.
    public void AddIncome(float amount)
    {
        GameController.Instance.goldManagerSettings.passiveIncomeRate += amount;
    }

    public void EarnGold(int amount)
    {
        gold += amount;
        OnGoldChange(gold);
        ScoreManager.Instance.goldEarned += amount;
    }

    // SpendGold method is well named and checks for sufficient funds before allowing a purchase.
    public bool SpendGold(int amount)
    {
        if (amount <= gold)
        {
            gold -= amount;
            OnGoldChange(gold);
            ScoreManager.Instance.goldSpent += amount;
            return true;
        }

        return false;
    }

    // This method is straightforward and clearly named.
    public int GetCurrentGold()
    {
        return gold;
    }

    // StopIncome is a good control method, but consider what should trigger it.
    // Should there be a method to restart income as well?
    public void StopIncome()
    {
        if (incomeCoroutine != null)
        {
            StopCoroutine(incomeCoroutine);
            incomeCoroutine = null;
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        OnGoldChange = delegate { };
    }
}
