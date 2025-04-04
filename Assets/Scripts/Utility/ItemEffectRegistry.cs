using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ItemEffectRegistry
{
    public static void RegisterAll()
    {
        //특수 고유 효과장비
    }
}

internal class SpecialEffect : IEffect
{
    public void ApplyEffect(IUnit target)
    {
        
    }
}
