using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
  static T _localInstance;
  public static bool Destroyed;

  public static T Instance
  {
    get
    {
      if (Destroyed)
        return null;

      if (_localInstance)
        return _localInstance;

      _localInstance = (T)FindObjectOfType(typeof(T));

      if (_localInstance)
        return _localInstance;

      var singletonObject = new GameObject();
      _localInstance = singletonObject.AddComponent<T>();
      singletonObject.name = typeof(T) + " (Singleton)";

      DontDestroyOnLoad(singletonObject);

      return _localInstance;
    }
  }

  protected virtual void Awake()
  {
    if (_localInstance && _localInstance != (this as T))
    {
      Destroy(gameObject);
      return;
    }

    _localInstance = this as T;
  }

  void OnApplicationQuit()
  {
    Destroyed = true;
  }

  protected virtual void OnDestroy()
  {
    Destroyed = true;
  }
}