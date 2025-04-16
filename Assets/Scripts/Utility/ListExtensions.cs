using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions 
{
    public static T GetRandom<T>(this List<T> list)
    {
        if (list == null || list.Count == 0)
        {
            Debug.LogWarning("[GetRandom] 리스트가 비어 있습니다!");
            return default;
        }

        int index = Random.Range(0, list.Count);
        return list[index];
    }
    
    public static T GetRandom<T>(this List<T> list, System.Predicate<T> filter)
    {
        var filtered = list.FindAll(filter);
        return filtered.GetRandom(); // 재귀 호출
    }
}
