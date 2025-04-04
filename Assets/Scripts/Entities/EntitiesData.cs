using System.Collections.Generic;
using UnityEngine;
using Utility;

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
        public string EntityID;
        public EntityType EntityType;
        public string Name;
        public string Description;
        public Sprite EnemySprite;
        public List<int> SpawnableFloors;
        public bool IsEventOnly;
        public int MaxHp;
        public int MaxMp;
        public int Attack;
        public int Defense;
        public List<IEffect> Effects;
        public string EffectKey;

        public void initializeEffect()
        {
            if (string.IsNullOrWhiteSpace(EffectKey))
            {
                Effects = new List<IEffect>();
                return;
            }
            
            string[] effectKeys = EffectKey.Split(',');
            
            foreach (var key in effectKeys)
            {
                var trimmedKey = key.Trim();
                if (string.IsNullOrWhiteSpace(trimmedKey)) continue;
                
                var effect = EffectFactory.Create(trimmedKey);
        
                if (effect != null)
                {
                    Debug.Log($"[✅ 추가됨] {trimmedKey} → {effect.GetType().Name}");
                    Effects.Add(effect);
                }
                else
                {
                    Debug.LogWarning($"[❌ 생성 실패] {trimmedKey}");
                }
            }
        }
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

        public EntitiesData Clone(int level)
        {
            return new EntitiesData
            {
                EntityID = this.EntityID,
                Name = this.Name,
                EntityType = this.EntityType,
                MaxHp = 30 + level * 10 + Random.Range(0, 6),
                MaxMp = this.MaxMp,
                Attack = 5 + level * 2 + Random.Range(0, 3),
                Defense = 2 + Mathf.RoundToInt(level * 1.5f + Random.Range(0, 2)),
                SpawnableFloors = new List<int>(this.SpawnableFloors),
                IsEventOnly = this.IsEventOnly,
                EffectKey = this.EffectKey
            };
        }

    }
}