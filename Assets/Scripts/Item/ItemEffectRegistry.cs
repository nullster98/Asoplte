using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ItemEffectRegistry
{
    public static void RegisterAll()
    {
        //특수 고유 효과장비
    }
    
    public static IEffect Create(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            Debug.LogWarning($"[key]가 없음");
            return null;
        }

        var effect = EffectRegistry.Create(key);

        return effect;
    }
}

internal class SpecialEffect : IEffect
{
    public string EffectDescription => null;

    public void ApplyEffect(IUnit target)
    {
        
    }
}
