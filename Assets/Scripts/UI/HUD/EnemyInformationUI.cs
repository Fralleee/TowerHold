using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class EnemyInformationUI : Controller
{
	EventBinding<EnemySpawnEvent> _enemySpawnEvent;
	EventBinding<EnemyVariantsSelectedEvent> _enemyVariantsSelectedEvent;

	Box _enemyInformation;
	Enemy[] _selectedVariants;
	readonly HashSet<Enemy> _spawnedVariants = new HashSet<Enemy>();

	void OnEnable()
	{
		_enemySpawnEvent = new EventBinding<EnemySpawnEvent>(OnEnemySpawn);
		EventBus<EnemySpawnEvent>.Register(_enemySpawnEvent);

		_enemyVariantsSelectedEvent = new EventBinding<EnemyVariantsSelectedEvent>(e => _selectedVariants = e.Variants);
		EventBus<EnemyVariantsSelectedEvent>.Register(_enemyVariantsSelectedEvent);
	}

	void OnDisable()
	{
		EventBus<EnemySpawnEvent>.Deregister(_enemySpawnEvent);
		EventBus<EnemyVariantsSelectedEvent>.Deregister(_enemyVariantsSelectedEvent);
	}

	protected override void Awake()
	{
		base.Awake();
		var uiDocument = GetComponent<UIDocument>();
		_enemyInformation = uiDocument.rootVisualElement.Q<Box>("EnemyInformationContainer");

		Controls.Keyboard.ShowEnemyInformation.performed += ToggleEnemyInformation;
		Controls.Keyboard.ShowEnemyInformation.canceled += ToggleEnemyInformation;
	}

	void OnEnemySpawn(EnemySpawnEvent enemySpawnEvent)
	{
		if (!_spawnedVariants.Contains(enemySpawnEvent.Enemy))
		{
			_spawnedVariants.Add(enemySpawnEvent.Enemy);
			PopulateEnemyInformation();
		}
	}

	void ToggleEnemyInformation(InputAction.CallbackContext context)
	{
		_enemyInformation.ToggleInClassList("show");
	}

	void PopulateEnemyInformation()
	{
		_enemyInformation.Clear();

		var knownEnemies = _selectedVariants.Where(enemy => _spawnedVariants.Contains(enemy)).OrderBy(enemy => enemy.Value).ToArray();
		var unknownEnemies = _selectedVariants.Where(enemy => !_spawnedVariants.Contains(enemy)).ToArray();

		foreach (var enemy in knownEnemies)
		{
			var enemyInformation = new EnemyInformation();
			enemyInformation.Setup(enemy);
			_enemyInformation.Add(enemyInformation);
		}

		foreach (var enemy in unknownEnemies)
		{
			var enemyInformation = new EnemyInformation();
			enemyInformation.Setup();
			_enemyInformation.Add(enemyInformation);
		}
	}
}
