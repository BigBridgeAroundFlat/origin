using System.Collections.Generic;

public static class DictionaryExtensions
{
    public static bool IsEmpty<TKey, TValue>(this IDictionary<TKey, TValue> source)
    {
        if (source == null || source.Count <= 0)
        {
            return true;
        }
        return false;
    }
}