using System.Collections;
using System.Collections.Generic;
using Trait;
using UnityEngine;

public static class TraitEffectRegistry
{
    public static void RegisterAll()
    {
        EffectRegistry.Register("StrongFaith", () => new StrongFaithEffect());
        EffectRegistry.Register("PowerfulForce", () => new PowerfulForceEffect());
        EffectRegistry.Register("MentalResponse", () => new MentalResponseEffect());
        EffectRegistry.Register("Absolute", () => new AbsoluteEffect());
        EffectRegistry.Register("Blind", () => new BlindEffect());
        EffectRegistry.Register("Weakness", () => new WeaknessEffect());
        EffectRegistry.Register("Sleepless", () => new SleeplessEffect());
        EffectRegistry.Register("AbsoluteN", () => new AbsoluteNEffect());
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

internal class StrongFaithEffect : IEffect
{
    public string EffectDescription => null;

    public void ApplyEffect(IUnit target)
    {
        
    }
}
internal class PowerfulForceEffect : IEffect
{
    public string EffectDescription => null;

    public void ApplyEffect(IUnit target)
    {
        
    }
}
internal class MentalResponseEffect : IEffect
{
    public string EffectDescription => null;

    public void ApplyEffect(IUnit target)
    {
        
    }
}
internal class AbsoluteEffect : IEffect
{
    public string EffectDescription => null;

    public void ApplyEffect(IUnit target)
    {
        
    }
}
internal class BlindEffect : IEffect
{
    public string EffectDescription => null;

    public void ApplyEffect(IUnit target)
    {
        
    }
}
internal class WeaknessEffect : IEffect
{
    public string EffectDescription => null;

    public void ApplyEffect(IUnit target)
    {
        
    }
}
internal class SleeplessEffect : IEffect
{
    public string EffectDescription => null;

    public void ApplyEffect(IUnit target)
    {
        
    }
}
internal class AbsoluteNEffect : IEffect
{
    public string EffectDescription => null;

    public void ApplyEffect(IUnit target)
    {
        
    }
}


