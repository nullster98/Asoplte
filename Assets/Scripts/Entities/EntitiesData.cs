using System.Collections.Generic;
using UnityEngine;

namespace Entities
{
    public enum EntityType
    {
        None,
        Monster,
        Boss,
        Npc
    }
    [System.Serializable]
    public class EntitiesData
    {
        public int EntityID { get; private set; }
        public EntityType EntityType { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public Sprite EnemySprite { get; private set; }
        public List<int> SpawnableFloors { get; private set; }//스폰 층수
        public bool IsEventOnly { get; private set; } // 이벤트 전용 여부
        public int MaxHp { get; private set; }
        public int MaxMp { get; private set; }
        public int Attack { get; private set; }
        public int Defense { get; private set; }

        public void LoadEnemySprite()
        {
            string folderPath = EntityType switch
            {
                EntityType.Npc => "Entity/NPC/Images",
                EntityType.Monster => "Entity/Monster/Images",
                EntityType.Boss => "Entity/Boss/Images",
                _ => "Entities/Default"
            };

            EnemySprite = Resources.Load<Sprite>($"{folderPath}/{Name}") ?? Resources.Load<Sprite>("Entity/default");
        
        }

        public void GetDescription()
        {
            string folderPath = EntityType switch
            {
                EntityType.Npc => "Entity/NPC/Descriptions",
                EntityType.Monster => "Entity/Monster/Descriptions",
                EntityType.Boss => "Entity/Boss/Descriptions",
                _ => "Entity/Default"
            };

            TextAsset textAsset = Resources.Load<TextAsset>($"{folderPath}/{Name}");
            Description = textAsset != null ? textAsset.text : "설명 없음";
        }

        public EntitiesData(int id, string name, EntityType type, 
            int maxHp, int maxMp, int attack, int defense, List<int> spawnableFloors,
            bool isEventOnly = false)
        {
            EntityID = id;
            Name = name;
            EntityType = type;
            MaxHp = maxHp;
            MaxMp = maxMp;
            Attack = attack;
            Defense = defense;
            SpawnableFloors = spawnableFloors;
            IsEventOnly = isEventOnly;
            
            LoadEnemySprite();
            GetDescription();
        }

    }
}