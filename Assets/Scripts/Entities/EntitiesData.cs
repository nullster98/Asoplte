using System.Collections.Generic;
using PlayerScript;
using UnityEngine;
using UnityEngine.Serialization;
using Utility;

namespace Entities
{
    public enum EntityType //Entity 종류
    {
        None,
        Monster,
        Boss,
        Npc
    }

    public enum NpcReaction // NPC전용 플레이어에게 보일 반응
    {
        Favorable,
        Neutral,
        Hostile
    }
    [System.Serializable]
    public class EntitiesData //모든 Entity에 대한 데이터. 추후 Enemy,Boss/NPC를 나눌 예정
    {
        public string EntityID;
        public EntityType EntityType;
        public string Name;
        public string codexText;
        public string codexPath;
        public Sprite EnemySprite;
        public string imagePath;
        public List<int> SpawnableFloors;
        public bool IsEventOnly;
        public int MaxHp;
        public int MaxMp;
        public int Attack;
        public int Defense;
        public List<IEffect> Effects;
        public string EffectKey;
        public string summary;

        public List<string> PreferredTraits;
        public List<string> DislikedTraits;

        public string FixedGodID;
        public string Personality;

        public string linkedEventPhaseID;

        public void initializeEffect() //EffectKey기반으로 실제 효과 리스트 생성
        {
            if (string.IsNullOrWhiteSpace(EffectKey))
            {
                Effects = new List<IEffect>();
                return;
            }
            
            string[] effectKeys = EffectKey.Split('|');
            
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
        
        public void InitializeNpcBehavior() //NPC의 고정 신앙 및 성향 출력시
        {
            if (EntityType != EntityType.Npc) return;

            // 선호, 비선호 특성 초기화 등
            Debug.Log($"{Name}의 성향: {Personality}, 고정 신앙: {FixedGodID}");
        }

        public EntitiesData Clone(int level) //특정 레벨을 기반으로 능력치가 반영된 클론 생성
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
                SpawnableFloors = this.SpawnableFloors != null ? new List<int>(this.SpawnableFloors) : new List<int>(),
                PreferredTraits = this.PreferredTraits != null ? new List<string>(this.PreferredTraits) : null,
                DislikedTraits = this.DislikedTraits != null ? new List<string>(this.DislikedTraits) : null,
                IsEventOnly = this.IsEventOnly,
                EffectKey = this.EffectKey,
                FixedGodID = this.FixedGodID,
                Personality = this.Personality,
                codexPath = this.codexPath,
                imagePath = this.imagePath,
                summary = this.summary
            };
        }
        public NpcReaction EvaluateReactionTo(Player player) //NPC가 플레이어를 평가하여 반응을 반환
        {
            if (EntityType != EntityType.Npc)
                return NpcReaction.Neutral;

            int score = 0;

            // 선호 특성
            foreach (var preferred in PreferredTraits)
            {
                if (player.HasTrait(preferred)) score += 1;
            }

            // 비선호 특성
            foreach (var disliked in DislikedTraits)
            {
                if (player.HasTrait(disliked)) score -= 1;
            }

            // 신앙 비교 추후 상성로직 추가 예정
            if (!string.IsNullOrEmpty(FixedGodID))
            {
                if (player.selectedGod != null && player.selectedGod.GodID == FixedGodID)
                    score += 2;
            }

            if (score >= 2) return NpcReaction.Favorable;
            if (score <= -2) return NpcReaction.Hostile;
            return NpcReaction.Neutral;
        }
    }
}