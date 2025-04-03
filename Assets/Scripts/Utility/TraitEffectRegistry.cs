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
}

internal class StrongFaithEffect : IEffect
{
    public void ApplyEffect(IUnit target)
    {
        
    }
}
internal class PowerfulForceEffect : IEffect
{
    public void ApplyEffect(IUnit target)
    {
        
    }
}
internal class MentalResponseEffect : IEffect
{
    public void ApplyEffect(IUnit target)
    {
        
    }
}
internal class AbsoluteEffect : IEffect
{
    public void ApplyEffect(IUnit target)
    {
        
    }
}
internal class BlindEffect : IEffect
{
    public void ApplyEffect(IUnit target)
    {
        
    }
}
internal class WeaknessEffect : IEffect
{
    public void ApplyEffect(IUnit target)
    {
        
    }
}
internal class SleeplessEffect : IEffect
{
    public void ApplyEffect(IUnit target)
    {
        
    }
}
internal class AbsoluteNEffect : IEffect
{
    public void ApplyEffect(IUnit target)
    {
        
    }
}


