using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class PUNSingleton<T> : PunBehaviour where T : PunBehaviour
{
    private static bool _isQuitting;

    public bool DontDestroyOnLoadConfig;

    private static T instance;

    /**
   Returns the instance of this singleton.
    */
    public static T Instance
    {
        get
        {
            if (_isQuitting)
                return null;

            if (instance == null)
            {
                instance = (T)FindObjectOfType(typeof(T));

                if (instance == null)
                {
                    GameObject newInstance = new GameObject(typeof(T).ToString());
                    instance = newInstance.AddComponent<T>();

                    DontDestroyOnLoad(newInstance);

                    return instance;
                }
            }

            return instance;
        }
    }

    /// <summary>
    /// Check if the instance has been instantiated.
    /// </summary>
    /// <returns>True if instance is not null</returns>
    public static bool CheckSanity()
    {
        if (instance != null)
            return true;

        return false;
    }

    protected virtual void Awake()
    {
        if (DontDestroyOnLoadConfig)
            DontDestroyOnLoad(gameObject);
    }

    private void OnApplicationQuit()
    {
        _isQuitting = true;
    }
}

