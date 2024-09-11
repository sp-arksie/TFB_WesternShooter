using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class DictionaryExtensions
{
    public static void RemoveAll<TKey, TValue>(
        Dictionary<TKey, TValue> dictionary,
        Func<TKey, TValue, bool> predicate)
    {
        List<TKey> keys = dictionary.Keys.Where(k => predicate(k, dictionary[k])).ToList();
        
        foreach (TKey key in keys)
        {
            dictionary.Remove(key);
        }
    }
}
