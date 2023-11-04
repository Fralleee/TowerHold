using System.Collections;
using UnityEngine;

public class GoldManager : MonoBehaviour
{
    public int gold = 100; // Starting gold
    public float passiveIncomeRate = 1.0f; // Gold generated per second
    private Coroutine incomeCoroutine;

    void Start()
    {
        // Delay the start of passive income generation
        incomeCoroutine = StartCoroutine(ActivateIncomeAfterDelay(GameController.instance.freezeTime));
    }

    // Consider separating the start of the coroutine from the method waiting for delay.
    // This adds clarity and avoids starting a coroutine within another coroutine, which can be hard to manage.
    private IEnumerator ActivateIncomeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        incomeCoroutine = StartCoroutine(PassiveIncomeCoroutine());
    }

    // The coroutine name is good as it describes its purpose.
    // However, consider what should happen if the game is paused or if the game ends. Should this coroutine stop?
    private IEnumerator PassiveIncomeCoroutine()
    {
        // If you need to stop income on game end, you can add a condition here.
        while (true)
        {
            yield return new WaitForSeconds(1f);
            gold += Mathf.FloorToInt(passiveIncomeRate);
            // Consider updating some UI element here to reflect the change in gold.
        }
    }

    // The method name 'AddIncome' is clear, but it might be mistaken for a method that directly adds gold.
    // Consider renaming it to 'IncreaseIncomeRate' or 'UpgradeIncomeRate' for clarity.
    public void AddIncome(float amount)
    {
        passiveIncomeRate += amount;
    }

    // SpendGold method is well named and checks for sufficient funds before allowing a purchase.
    public bool SpendGold(int amount)
    {
        if (amount <= gold)
        {
            gold -= amount;
            return true;
        }
        else
        {
            // It's good to provide feedback, but consider using a UI element to notify the user, not just the debug log.
            Debug.LogWarning("Not enough gold!");
            return false;
        }
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
}
