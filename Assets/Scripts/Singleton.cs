using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>// Класс для подгрузки данных
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance is null)
            {
                _instance = FindObjectOfType<T>();
            }
            else if (_instance != FindObjectOfType<T>()) // если ситуация что Manager не должен создаваться
            {
                Destroy(FindObjectOfType<T>()); // то его нужно удалить
            }

            DontDestroyOnLoad(FindObjectOfType<T>()); // при загрузке Manager всегда должен оставатся на сцене

            return _instance;
        }
    }
    protected virtual void Awake()
    {
        if (_instance != null)
        {
            Debug.LogErrorFormat("[Singleton] Trying to instantiate a second instance of singleton class {0}", GetType().Name);
        }
        else
        {
            _instance = (T) this;
        }
    }

    public static bool IsInitialized => _instance != null;

    protected virtual void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
}
