using UnityEngine;
public static class GenericsUtility
{
    public static T MakeSingleton<T>(this T manager) where T : MonoBehaviour
    {
        T[] instances = Object.FindObjectsByType<T>(FindObjectsSortMode.None);

        if (instances.Length > 1)
        {
            Object.Destroy(manager.gameObject);
            return instances[0];
        }
        else
        {
            Object.DontDestroyOnLoad(manager.gameObject);
            return manager;
        }
    }

    public static T GetManager<T>(this MonoBehaviour manager) where T : Manager
    {
        T[] instances = Object.FindObjectsByType<T>(FindObjectsSortMode.None);

        if (instances.Length > 0)
        {
            return instances[0];
        }
        else
        {
            Debug.LogError("Manager of type " + typeof(T) + " not found in scene!");
            return null;
        }
    }
}

