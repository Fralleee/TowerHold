using UnityEngine;
using UnityEngine.UIElements;

public class UpdateUI : MonoBehaviour
{
	[SerializeField] Texture2D _levelIcon;

	TooltipController _tooltipController;
	VisualElement _progressContainer;
	Label _levelLabel;
	Label _coinLabel;
	Label _incomeLabel;
	CustomProgressBar _healthBar;
	CustomProgressBar _levelBar;

	EventBinding<GameStartEvent> _gameStartEvent;
	EventBinding<LevelChangedEvent> _levelChangedEvent;
	EventBinding<IncomeChangedEvent> _incomeChangedEvent;
	EventBinding<ResourceChangedEvent> _resourceChangedEvent;
	EventBinding<TowerHealthChangedEvent> _towerHealthChangedEvent;

	void OnEnable()
	{
		_gameStartEvent = new EventBinding<GameStartEvent>(e => _levelBar.AddToClassList("active"));
		EventBus<GameStartEvent>.Register(_gameStartEvent);

		_levelChangedEvent = new EventBinding<LevelChangedEvent>(e => _levelLabel.text = $"Level: {e.CurrentLevel}");
		EventBus<LevelChangedEvent>.Register(_levelChangedEvent);

		_incomeChangedEvent = new EventBinding<IncomeChangedEvent>(e => _incomeLabel.text = $"Income: {e.CurrentIncome}");
		EventBus<IncomeChangedEvent>.Register(_incomeChangedEvent);

		_resourceChangedEvent = new EventBinding<ResourceChangedEvent>(e => _coinLabel.text = $"Coin: {e.CurrentResources}");
		EventBus<ResourceChangedEvent>.Register(_resourceChangedEvent);

		_towerHealthChangedEvent = new EventBinding<TowerHealthChangedEvent>(e =>
		{
			_healthBar.MinMaxValue = (e.Health, e.MaxHealth);
			_healthBar.Value = e.Health / (float)e.MaxHealth;
		});
		EventBus<TowerHealthChangedEvent>.Register(_towerHealthChangedEvent);
	}

	void OnDisable()
	{
		EventBus<GameStartEvent>.Deregister(_gameStartEvent);
		EventBus<LevelChangedEvent>.Deregister(_levelChangedEvent);
		EventBus<IncomeChangedEvent>.Deregister(_incomeChangedEvent);
		EventBus<ResourceChangedEvent>.Deregister(_resourceChangedEvent);
		EventBus<TowerHealthChangedEvent>.Deregister(_towerHealthChangedEvent);
	}

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

		_tooltipController.RegisterTooltip(_levelLabel, new TooltipContent(null, "Level", null, "This is the current level"));
		_tooltipController.RegisterTooltip(_coinLabel, new TooltipContent(null, "Coin", null, "This is the amount of coins you have"));
		_tooltipController.RegisterTooltip(_incomeLabel, new TooltipContent(null, "Income", null, "This is the amount of coins you get per second"));
		_tooltipController.RegisterTooltip(_healthBar, new TooltipContent(null, "Health", null, "This is the health of your base"));
		_tooltipController.RegisterTooltip(_levelBar, new TooltipContent(null, "Level", null, "This is the progress of the current level"));
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
}
