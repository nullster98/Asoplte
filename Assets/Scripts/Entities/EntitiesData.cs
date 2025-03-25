using UnityEngine;

namespace Entities
{
    public enum EntitiesType
    {
        Monster,
        Boss,
        Npc
    }
    [System.Serializable]
    public class EntitiesData
    {
        public int EnemyID { get; private set; }
        public EntitiesType NpcType { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public Sprite EnemySprite { get; private set; }
        public int Level { get; private set; }
        public int MaxHp { get; private set; }
        public int MaxMp { get; private set; }
        public int Attack { get; private set; }
        public int Defense { get; private set; }
        //public List<string> Abilities;

        public void LoadEnemySprite()
        {
            string folderPath = NpcType switch
            {
                EntitiesType.Npc => "Entities/NPC/Images",
                EntitiesType.Monster => "Entities/Monster/Images",
                EntitiesType.Boss => "Entities/Boss/Images",
                _ => "Entities/Default"
            };

            EnemySprite = Resources.Load<Sprite>($"{folderPath}/{Name}") ?? Resources.Load<Sprite>("Entities/default");
        
        }

        public void GetDescription()
        {
            string folderPath = NpcType switch
            {
                EntitiesType.Npc => "Entitie/NPC/Descriptions",
                EntitiesType.Monster => "Entitie/Monster/Descriptions",
                EntitiesType.Boss => "Entitie/Boss/Descriptions",
                _ => "Entitie/Default"
            };

            TextAsset textAsset = Resources.Load<TextAsset>($"{folderPath}/{Name}");
            Description = textAsset != null ? textAsset.text : "설명 없음";
        }

        public EntitiesData(int id, string name, EntitiesType type, int level, int maxHp, int maxMp, int attack, int defense)
        {
            EnemyID = id;
            Name = name;
            NpcType = type;
            Level = level;
            MaxHp = maxHp;
            MaxMp = maxMp;
            Attack = attack;
            Defense = defense;

            LoadEnemySprite();
            GetDescription();

        }

    }
}