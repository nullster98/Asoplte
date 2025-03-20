using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
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
    public string Description;
    public Sprite EnemySprite;
    public float Level;
    public float MaxHP;
    public float CurrentHP;
    public float MaxMP;
    public float Attack;
    public float Defense;
    //public List<string> Abilities;

    public void LoadEnemySprite()
    {
        string folderPath = NPCType switch
        {
            Enemy.NPC => "Entitie/NPC/Images",
            Enemy.Monster => "Entitie/Monster/Images",
            Enemy.Boss => "Entitie/Boss/Images",
            _ => "Entite/Default"
        };

       EnemySprite = Resources.Load<Sprite>($"{folderPath}/{Name}");
        if (EnemySprite == null)
        {
            EnemySprite = Resources.Load<Sprite>("Entitie/default");
        }
    }

    public void GetDescription()
    {
        string folderPath = NPCType switch
        {
            Enemy.NPC => "Entitie/NPC/Descriptions",
            Enemy.Monster => "Entitie/Monster/Descriptions",
            Enemy.Boss => "Entitie/Boss/Descriptions",
            _ => "Entite/Default"
        };

        TextAsset textAsset = Resources.Load<TextAsset>($"{folderPath}/{Name}");
        Description = textAsset != null ? textAsset.text : "설명 없음";
    }

    public EnemyData(float id, string name, Enemy type, float level, float maxHP, float maxMP, float attack, float defense)
    {
        EnemyID = id;
        Name = name;
        NPCType = type;
        Level = level;
        MaxHP = maxHP;
        CurrentHP = maxHP;
        MaxMP = maxMP;
        Attack = attack;
        Defense = defense;

        LoadEnemySprite();
        GetDescription();

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
