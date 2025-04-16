using System.Collections;
using System.Collections.Generic;
using PlayerScript;
using UnityEngine;

public interface IEffect
{
    void ApplyEffect(IUnit target);
    string EffectDescription { get; }
}

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
    public void ApplyEffect(IUnit target)
    {
        target.ChangeStat(stat, amount);
    }
    public void RemoveEffect(IUnit target)
    {
        target.ChangeStat(stat, -amount);
    }
    public string EffectDescription => null;

}

public class HealEffect : IEffect
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
