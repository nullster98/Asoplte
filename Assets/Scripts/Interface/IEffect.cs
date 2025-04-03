using System.Collections;
using System.Collections.Generic;
using PlayerScript;
using UnityEngine;

public interface IEffect
{
    void ApplyEffect(IUnit target);
}

public class StatModifierEffect : IEffect
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
}
