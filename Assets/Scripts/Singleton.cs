using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static volatile T instance;
    private static readonly object lockObject = new object();

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                lock (lockObject)
                {
                    // 再度nullチェックを行う
                    if (instance == null)
                    {
                        instance = FindObjectOfType<T>();
                    }
                }
            }

            return instance;
        }
    }

    // Awakeメソッドを使ってDontDestroyOnLoadを呼び出す
    protected virtual void Awake()
    {
        lock (lockObject)
        {
            if (Instance == null)
            {
                OnAwake();
                return;
            }
            else if (instance != this)
            {
                // すでにインスタンスが設定されている場合は破棄する
                Destroy(gameObject);
                Debug.Log("Destroy");
            }
            else
            {
                DontDestroyOnLoad(gameObject);
            }
        }
    }

    protected virtual void Start()
    {
        // DO NOTHING
    }

    protected virtual void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    // 設定を読み込むメソッド
    protected void InitializeSingletonObject(string name)
    {
        var loadObject = Resources.Load<GameObject>(name);

        if (loadObject!= null)
        {
            Debug.Log($"Settings Loaded: {name}");
            GameObject singletonObject = Instantiate(loadObject);
            instance = singletonObject.AddComponent<T>();
            DontDestroyOnLoad(singletonObject);
        }
        else
        {
            Debug.LogError($"Failed to load {name}.");
        }
    }

    protected virtual void OnAwake()
    {
        // DO NOTHIG
        // ここで name を渡しInitileze()を呼ぶ.
        // Initialize(string name)
    }
}