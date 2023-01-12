using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    // Check to see if we're about to be destroyed.
    private static bool _mShuttingDown = false;
    private static object _mLock = new object();
    private static T _mInstance;

    /// <summary>
    /// Access singleton instance through this propriety.
    /// </summary>
    public static T Instance
    {
        get
        {
            if (_mShuttingDown)
            {
                Debug.LogError("[Singleton] Instance '" + typeof(T) + "' already destroyed. Returning null.");
                return null;
            }

            lock (_mLock)
            {
                if (_mInstance == null)
                {
                    // Search for existing instance.
                    _mInstance = (T)FindObjectOfType(typeof(T));

                    // Create new instance if one doesn't already exist.
                    if (_mInstance == null)
                    {
                        // Need to create a new GameObject to attach the singleton to.
                        var singletonObject = new GameObject();
                        _mInstance = singletonObject.AddComponent<T>();
                        singletonObject.name = typeof(T) + " (Singleton)";
                    }

                    UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneChanged;
                }

                return _mInstance;
            }
        }
    }


    protected void Awake() { _mShuttingDown = false; }
    protected void OnApplicationQuit() => _mShuttingDown = false;

    protected void OnDestroy()
    {
        _mShuttingDown = true;
    }

    protected static void OnSceneChanged(Scene scene, LoadSceneMode mode)
    {
        _mShuttingDown = false;
    }
}
