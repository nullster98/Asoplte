using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]  // Unity ����ȭ �����ϵ��� ����
public abstract class ItemEffect
{
    public abstract void ApplyEffect(Player player);
}

[System.Serializable]
public class EquipEffect : ItemEffect
{
    [Serializable] // �� ���� ������ ������ ����ü
    public struct StatModifier
    {
        public string StatName;  // ������ ���� (��: "Atk", "Def", "HP")
        public float Value;  // ���� �Ǵ� ������ ��
    }

    public List<StatModifier> StatModifiers = new List<StatModifier>(); // ���� ���� ������ ����

    public EquipEffect(List<StatModifier> statModifiers)
    {
        this.StatModifiers = statModifiers;
    }

    public override void ApplyEffect(Player player)
    {
        foreach (var modifier in StatModifiers)
        {
            player.ChangeStat(modifier.StatName, modifier.Value);
            Debug.Log($"{modifier.StatName}��(��) {modifier.Value} ��ŭ ����Ǿ����ϴ�!");
        }
    }
}

[System.Serializable]
public class PoisonEffect : ItemEffect
{
    public int PoisonDamage;
    public int Duration;

    public PoisonEffect(int poisonDamage, int duration)
    {
        this.PoisonDamage = poisonDamage;
        this.Duration = duration;
    }

    public override void ApplyEffect(Player player)
    {
        //player.ApplyPoison(PoisonDamage, Duration);
    }
}

[System.Serializable]
public class ConsumptionEffect : ItemEffect
{
    public string EffectDescription;
    public int HealAmount;

    public ConsumptionEffect(string description, int healAmount)
    {
        this.EffectDescription = description;
        this.HealAmount = healAmount;
    }

    public override void ApplyEffect(Player player)
    {
        Debug.Log($"�Һ� ������ ���! ȿ��: {EffectDescription}");
        float currentHP = player.GetStat("CurrentHP");
        float maxHP = player.GetStat("HP");
        float newHP = Mathf.Min(currentHP + HealAmount, maxHP);
        player.ChangeStat("CurrentHP", newHP - currentHP);
    }
}