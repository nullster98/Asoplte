using System.Collections;
using System.Collections.Generic;
using PlayerScript;
using UnityEngine;

public interface IEffect // 모든 효과 클래스가 구현해야 하는 인터페이스
{
    void ApplyEffect(IUnit target);
    string EffectDescription { get; } // 효과 설명 문자열
}

// 스탯 증가/감소 효과
public class StatModifierEffect : IEffect, IRemovableEffect
{
    private readonly string stat;
    private readonly int amount;

    public string StatName => stat;
    public int Amount => amount;
    public string Description => $"{stat} {(amount >= 0 ? "+" : "")}{amount}";

    public StatModifierEffect(string stat, int amount)
    {
        this.stat = stat;
        this.amount = amount;
    }
    public void ApplyEffect(IUnit target) // 스탯에 효과 적용
    {
        target.ChangeStat(stat, amount);
    }
    public void RemoveEffect(IUnit target) // 스탯 효과 제거 (원상복구)
    {
        target.ChangeStat(stat, -amount);
    }
    public string EffectDescription => null; // 현재 미사용 중

}

public class HealEffect : IEffect // 체력 회복 효과
{
    private readonly int healAmount;
    private readonly bool isPercentage;
    public int Amount => healAmount;
    //public string Description => $"HP +{healAmount}";

    public HealEffect(int amount, bool isPercentage = false)
    {
        this.healAmount = amount;
        this.isPercentage = isPercentage;
    }
    public void ApplyEffect(IUnit target)
    {
        int maxHP = target.GetStat("MaxHP");
        int currentHP = target.GetStat("CurrentHP");

        int rawHeal = isPercentage
            ? Mathf.FloorToInt(maxHP * (healAmount / 100f))
            : healAmount;
        
        int acutalHeal = Mathf.Min(healAmount, maxHP - currentHP);
        if (acutalHeal <= 0)
        {
            Debug.Log("최대체력 초과");
            return;
        }
        target.ChangeStat("CurrentHP", acutalHeal);
    }

    public string EffectDescription => null;
}
