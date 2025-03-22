using UnityEngine;

namespace Entities
{
    public enum EnemyType
    {
        None,
        Monster,
        Boss,
        Npc
    }
    [System.Serializable]
    public class EnemyData
    {
        public int EnemyID { get; private set; }
        public EnemyType NpcType { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public Sprite EnemySprite { get; private set; }
        public int Level { get; private set; }
        public int MaxHp { get; private set; }
        public int CurrentHp { get; private set; }
        public int MaxMp { get; private set; }
        public int Attack { get; private set; }
        public int Defense { get; private set; }
        //public List<string> Abilities;

        public void LoadEnemySprite()
        {
            string folderPath = NpcType switch
            {
                EnemyType.Npc => "Entities/NPC/Images",
                EnemyType.Monster => "Entities/Monster/Images",
                EnemyType.Boss => "Entities/Boss/Images",
                _ => "Entities/Default"
            };

            EnemySprite = Resources.Load<Sprite>($"{folderPath}/{Name}") ?? Resources.Load<Sprite>("Entities/default");
        
        }

        public void GetDescription()
        {
            string folderPath = NpcType switch
            {
                EnemyType.Npc => "Entitie/NPC/Descriptions",
                EnemyType.Monster => "Entitie/Monster/Descriptions",
                EnemyType.Boss => "Entitie/Boss/Descriptions",
                _ => "Entitie/Default"
            };

            TextAsset textAsset = Resources.Load<TextAsset>($"{folderPath}/{Name}");
            Description = textAsset != null ? textAsset.text : "설명 없음";
        }

        public EnemyData(int id, string name, EnemyType type, int level, int maxHp, int maxMp, int attack, int defense)
        {
            EnemyID = id;
            Name = name;
            NpcType = type;
            Level = level;
            MaxHp = maxHp;
            CurrentHp = maxHp;
            MaxMp = maxMp;
            Attack = attack;
            Defense = defense;

            LoadEnemySprite();
            GetDescription();

        }

        public void TakeDamage(int damage)
        {
            CurrentHp -= damage;
            if(CurrentHp < 0) CurrentHp = 0;
        }    

        public void Heal(int amount)
        {
            CurrentHp += amount;
            if(CurrentHp > MaxHp) CurrentHp = MaxHp;
        }
    }
}