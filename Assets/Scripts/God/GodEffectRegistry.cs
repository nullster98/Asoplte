using System.Collections;
using System.Collections.Generic;
using God;
using UnityEngine;

public static class GodEffectRegistry
{
    public static void RegisterAll()
    {
        EffectRegistry.Register("Liberty", () => new LibertyGodEffect());
        EffectRegistry.Register("Revenge", () => new RevengeGodEffect());
        EffectRegistry.Register("Furious", () => new FuriousGodEffect());
        // 추후 다른 고유 효과도 여기에 추가
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


internal class LibertyGodEffect : IEffect
{
    public string EffectDescription => null;

    public void ApplyEffect(IUnit target)
    {
        //적용되나 확인
        target.ChangeStat("Atk", 100);
        // 실제 효과 구현 예정
    }

    public override string ToString() => "고유효과 준비중";
}

internal class RevengeGodEffect : IEffect
{
    public string EffectDescription => null;

    public override string ToString() => "고유효과 준비중";

    public void ApplyEffect(IUnit target)
    {
    }
}

internal class FuriousGodEffect : IEffect
{
    public string EffectDescription => null;

    public override string ToString() => "고유효과 준비중";

    public void ApplyEffect(IUnit target)
    {
    }
}