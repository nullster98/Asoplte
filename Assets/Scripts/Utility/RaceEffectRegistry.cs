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
}

internal class GolemEffect : IEffect
{
    public void ApplyEffect(IUnit target)
    {
        
    }
}

internal class RuneGolemEffect : IEffect
{
    public void ApplyEffect(IUnit target)
    {
        
    }
}

internal class LightGolemEffect : IEffect
{
    public void ApplyEffect(IUnit target)
    {
        
    }
}
internal class ForestGolemEffect : IEffect
{
    public void ApplyEffect(IUnit target)
    {
        
    }
}

internal class OakEffect : IEffect
{
    public void ApplyEffect(IUnit target)
    {
        
    }
}

internal class OakWarriorEffect : IEffect
{
    public void ApplyEffect(IUnit target)
    {
        
    }
}
internal class GrayOakEffect : IEffect
{
    public void ApplyEffect(IUnit target)
    {
        
    }
}
internal class RedOakEffect : IEffect
{
    public void ApplyEffect(IUnit target)
    {
        
    }
}