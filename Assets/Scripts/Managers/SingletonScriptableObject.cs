using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject
{
    static T _instance = null;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                T[] results = Resources.FindObjectsOfTypeAll<T>();
                if (results.Length != 1)
                {
                    Debug.LogError($"SingletonScriptableObject.Instance : results length is {results.Length} for {typeof(T).ToString()}");
                    return null;
                }
                _instance = results[0];
            }

            return _instance;
        }
    }
}
