using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public static class RaceEffectRegistry 
{
    public static void RegisterAll()
    {
        EffectRegistry.Register("Golem", () => new GolemEffect());
        EffectRegistry.Register("RuneGolem", () => new RuneGolemEffect());
        EffectRegistry.Register("LightGolem", () => new LightGolemEffect());
        EffectRegistry.Register("ForestGolem", () => new ForestGolemEffect());
        
        EffectRegistry.Register("Oak", () => new OakEffect());
        EffectRegistry.Register("OakWarrior", () => new OakWarriorEffect());
        EffectRegistry.Register("GrayOak", () => new GrayOakEffect());
        EffectRegistry.Register("RedOak", () => new RedOakEffect());


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

internal class GolemEffect : IEffect
{
    public string EffectDescription => null;

    public void ApplyEffect(IUnit target)
    {
        
    }
}

internal class RuneGolemEffect : IEffect
{
    public string EffectDescription => null;

    public void ApplyEffect(IUnit target)
    {
        
    }
}

internal class LightGolemEffect : IEffect
{
    public string EffectDescription => null;

    public void ApplyEffect(IUnit target)
    {
        
    }
}
internal class ForestGolemEffect : IEffect
{
    public string EffectDescription => null;

    public void ApplyEffect(IUnit target)
    {
        
    }
}

internal class OakEffect : IEffect
{
    public string EffectDescription => null;

    public void ApplyEffect(IUnit target)
    {
        
    }
}

internal class OakWarriorEffect : IEffect
{
    public string EffectDescription => null;

    public void ApplyEffect(IUnit target)
    {
        
    }
}
internal class GrayOakEffect : IEffect
{
    public string EffectDescription => null;

    public void ApplyEffect(IUnit target)
    {
        
    }
}
internal class RedOakEffect : IEffect
{
    public string EffectDescription => null;

    public void ApplyEffect(IUnit target)
    {
        
    }
}