using UnityEngine;

public static class GameObjectExtension
{
    public static T GetOrAddComponent<T>(this GameObject target)
        where T : Component
    {
        var component = target.GetComponent<T>();
        if (component != null) return component;

        Debug.LogWarning(string.Format("No {0} in the {1}.", typeof(T), target.name));

        component = target.AddComponent<T>();

        return component;
    }
}
