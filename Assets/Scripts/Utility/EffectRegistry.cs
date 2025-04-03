using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EffectRegistry
{
    private static readonly Dictionary<string, Func<IEffect>> registry = new();

    public static void Register(string key, Func<IEffect> creator)
    {
        if(!registry.ContainsKey(key))
            registry.Add(key, creator);
    }

    public static bool Contains(string key) => registry.ContainsKey(key);
    public static IEffect Create(string key) => registry.TryGetValue(key, out var create)
                                                ? create() : null;
    
    [RuntimeInitializeOnLoadMethod]
    public static void InitializeAllEffects()
    {
        var types = typeof(IEffect).Assembly.GetTypes();

        foreach (var type in types)
        {
            if (!type.IsAbstract && typeof(IEffect).IsAssignableFrom(type))
            {
                try
                {
                    System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(type.TypeHandle);
                    Debug.Log($"[EffectRegistry] 클래스 로딩됨: {type.Name}");
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"[EffectRegistry] {type.Name} 로딩 중 오류: {e.Message}");
                }
            }
        }

        Debug.Log($"[EffectRegistry] 모든 Effect 클래스 초기화 완료. 총 등록 수: {registry.Count}");
    }
}
