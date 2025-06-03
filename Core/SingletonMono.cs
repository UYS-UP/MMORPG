using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonMono<T> : MonoBehaviour where T : SingletonMono<T>
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<T>();
                if (instance != null) return instance;
                var singletonObject = new GameObject();
                instance = singletonObject.AddComponent<T>();
                singletonObject.name = $"{typeof(T).Name}";
            }

            return instance;
        }
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
        }
    }
}
