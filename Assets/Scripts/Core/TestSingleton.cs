using UnityEngine;

public class TestSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
	static T _instance;

	public static T Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindFirstObjectByType<T>();

				if (_instance == null)
				{
					var singletonObject = new GameObject();
					_instance = singletonObject.AddComponent<T>();
					singletonObject.name = typeof(T) + " (Singleton)";
				}
			}

			return _instance;
		}
	}

	protected virtual void Awake()
	{
		if (_instance != null && _instance != this)
		{
			Destroy(gameObject);
		}
		else
		{
			_instance = this as T;
		}
	}

	protected virtual void OnDestroy()
	{
		if (_instance == this)
		{
			_instance = null;
		}
	}
}
