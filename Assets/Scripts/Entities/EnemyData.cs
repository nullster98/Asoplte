using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Enemy
{
    None,
    Monster,
    Boss,
    NPC
}
[System.Serializable]
public class EnemyData
{
    public float EnemyID;
    public Enemy NPCType;
    public string Name;
    public Sprite EnemySprite;
    public float Level;
    public float MaxHP;
    public float CurrentHP;
    public float MaxMP;
    public float Attack;
    public float Defense;
    //public List<string> Abilities;

    public EnemyData(float id, string name, Enemy type, float level, float maxHP, float maxMP, float attack, float defense, Sprite sprite)
    {
        this.EnemyID = id;
        this.Name = name;
        this.NPCType = type;
        this.Level = level;
        this.MaxHP = maxHP;
        this.CurrentHP = maxHP;
        this.MaxMP = maxMP;
        this.Attack = attack;
        this.Defense = defense;
        this.EnemySprite = sprite;
    }

    private void Awake()
    {
        CurrentHP = MaxHP;
    }

    public void TakeDamage(int damage)
    {
        CurrentHP -= damage;
        if(CurrentHP < 0) CurrentHP = 0;
    }    

    public void Heal(int amount)
    {
        CurrentHP += amount;
        if(CurrentHP > MaxHP) CurrentHP = MaxHP;
    }
}
