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
    
    public void RemoveEffect(Player player)
    {

    }
}
