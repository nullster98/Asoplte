using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]  // Unity 직렬화 가능하도록 설정
public abstract class ItemEffect
{
    public abstract void ApplyEffect(Player player);
}

[System.Serializable]
public class EquipEffect : ItemEffect
{
    [Serializable] // 각 스탯 정보를 저장할 구조체
    public struct StatModifier
    {
        public string StatName;  // 적용할 스탯 (예: "Atk", "Def", "HP")
        public float Value;  // 증가 또는 감소할 값
    }

    public List<StatModifier> StatModifiers = new List<StatModifier>(); // 여러 개의 스탯을 저장

    public EquipEffect(List<StatModifier> statModifiers)
    {
        this.StatModifiers = statModifiers;
    }

    public override void ApplyEffect(Player player)
    {
        foreach (var modifier in StatModifiers)
        {
            player.ChangeStat(modifier.StatName, modifier.Value);
            Debug.Log($"{modifier.StatName}이(가) {modifier.Value} 만큼 변경되었습니다!");
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
        Debug.Log($"소비 아이템 사용! 효과: {EffectDescription}");
        float currentHP = player.GetStat("CurrentHP");
        float maxHP = player.GetStat("HP");
        float newHP = Mathf.Min(currentHP + HealAmount, maxHP);
        player.ChangeStat("CurrentHP", newHP - currentHP);
    }
}