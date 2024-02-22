using UnityEngine;
using UnityEngine.UIElements;

public class UpdateUI : MonoBehaviour
{
	TooltipController _tooltipController;
	VisualElement _progressContainer;
	Label _levelLabel;
	Label _coinLabel;
	Label _incomeLabel;
	CustomProgressBar _healthBar;
	CustomProgressBar _levelBar;

	void Awake()
	{
		var uiDocument = GetComponent<UIDocument>();
		_tooltipController = GetComponent<TooltipController>();

		_levelLabel = uiDocument.rootVisualElement.Q<Label>("LevelLabel");
		_coinLabel = uiDocument.rootVisualElement.Q<Label>("CoinLabel");
		_incomeLabel = uiDocument.rootVisualElement.Q<Label>("IncomeLabel");

		_progressContainer = uiDocument.rootVisualElement.Q("Progress");
		_healthBar = _progressContainer.Q<CustomProgressBar>("HealthBar");
		_levelBar = _progressContainer.Q<CustomProgressBar>("LevelBar");


		_tooltipController.RegisterTooltip(_levelLabel, new TooltipContent("Level", "The current level", "This is the current level"));
		_tooltipController.RegisterTooltip(_coinLabel, new TooltipContent("Coin", "The amount of coins you have", "This is the amount of coins you have"));
		_tooltipController.RegisterTooltip(_incomeLabel, new TooltipContent("Income", "The amount of coins you get per second", "This is the amount of coins you get per second"));
		_tooltipController.RegisterTooltip(_healthBar, new TooltipContent("Health", "The health of your base", "This is the health of your base"));
		_tooltipController.RegisterTooltip(_levelBar, new TooltipContent("Level", "The progress of the current level", "This is the progress of the current level"));


		GameController.OnGameStart += OnGameStart;
		GameController.OnLevelChanged += OnLevelChanged;
		Tower.OnHealthChanged += OnHealthChanged;
		ResourceManager.OnResourceChange += OnResourceChanged;
		ResourceManager.OnIncomeChange += OnIncomeChanged;
	}

	void Start()
	{
		_healthBar.UseChangeBar = true;
		_levelBar.UseChangeBar = false;
		InvokeRepeating(nameof(UpdateLevelProgress), 0, 1f);
	}

	void UpdateLevelProgress()
	{
		var (timeLeft, totalTime, progress) = GameController.Instance.LevelProgress;
		var value = Mathf.Round(progress * totalTime) / totalTime;

		_levelBar.MinMaxValue = (Mathf.CeilToInt(timeLeft), Mathf.CeilToInt(totalTime));
		_levelBar.Value = value;
	}

	void OnGameStart()
	{
		_levelBar.AddToClassList("active");
	}

	void OnLevelChanged(int level)
	{
		_levelLabel.text = $"Level: {level}";
	}

	void OnHealthChanged(int currentHealth, int maxHealth)
	{
		_healthBar.MinMaxValue = (currentHealth, maxHealth);
		_healthBar.Value = currentHealth / (float)maxHealth;
	}


	void OnResourceChanged(int coin)
	{
		_coinLabel.text = $"Coin: {coin}";
	}

	void OnIncomeChanged(int income)
	{
		_incomeLabel.text = $"Income: {income}";
	}

	void OnDestroy()
	{
		GameController.OnGameStart -= OnGameStart;
		GameController.OnLevelChanged -= OnLevelChanged;
		Tower.OnHealthChanged -= OnHealthChanged;
		ResourceManager.OnResourceChange -= OnResourceChanged;
		ResourceManager.OnIncomeChange -= OnIncomeChanged;
	}
}
