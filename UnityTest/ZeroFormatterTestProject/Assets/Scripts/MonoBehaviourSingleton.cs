using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoBehaviourSingleton <T> : MonoBehaviour where T : MonoBehaviour
{
    private static bool Removed = false;
    private static object Locked = new object();
    private static T Inst;

    public static T Instance
    {
        get
        {
            if (Removed)
            {
                Debug.LogWarning("MonoBehaviourSingleton Instance '" + typeof(T) + "destroyed. ");
                return null;
            }

            lock (Locked)
            {
                if (Inst == null)
                {
                    Inst = (T)FindObjectOfType(typeof(T));

                    if (Inst == null)
                    {
                        var singletonObject = new GameObject();
                        Inst = singletonObject.AddComponent<T>();
                        singletonObject.name = typeof(T).ToString() + " (Singleton)";

                        DontDestroyOnLoad(singletonObject);
                    }
                }

                return Inst;
            }
        }
    }

    private void OnApplicationQuit()
    {
        Removed = true;
    }

    private void OnDestroy()
    {
        Removed = true;
    }
}


