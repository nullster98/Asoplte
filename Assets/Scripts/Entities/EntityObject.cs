using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using Game;
using JetBrains.Annotations;
using PlayerScript;
using TMPro;
using Unity;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Entities
{
    public class EntityObject : MonoBehaviour, IUnit
    {
        public EntitiesData enemyData;
        
        public UnitType UnitType => UnitType.Enemy;
        
        private Dictionary<string, int> stats = new()
        {
            { "Atk", 0 },
            { "Def", 0 },
            { "HP", 0 },
            { "MP", 0 },
            { "CurrentHP", 0 },
            { "CurrentMP", 0 }
        };

        public int HP => GetStat("HP");
        public int MP => GetStat("MP");
        public int Atk => GetStat("Atk");
        public int Def => GetStat("Def");
        public int CurrentHP => GetStat("CurrentHP");
        public int CurrentMP => GetStat("CurrentMP");
        
        public string godID;
        public List<string> traitIDs = new();
        public List<string> itemIDs = new();

        public string npcEventID;
        public GameObject shopPanel;
        public GameObject infoPanel;

        public NpcReaction reaction;

        private void Start()
        {
            if (enemyData.EntityType == EntityType.Npc)
            {
                reaction = enemyData.EvaluateReactionTo(Player.Instance);
            }
        }

        public void Initialize(EntitiesData data)
        {
            enemyData = data;
            SetStat("CurrentHP", data.MaxHp); 
            SetStat("CurrentMP", data.MaxMp);
            stats["HP"] = data.MaxHp;
            stats["MP"] = data.MaxMp;
            stats["Atk"] = data.Attack;
            stats["Def"] = data.Defense;

            var image = GetComponent<Image>();
            if (image != null && data.EnemySprite != null)
            {
                image.sprite = data.EnemySprite;
            }
            else
            {
                Debug.LogWarning("Image가 없거나 Sprite가 비어 있습니다!");
            }
        }
        
        public void InitializeRandomLoadout(EntityType type, int floor)
        {
            traitIDs.Clear();
            itemIDs.Clear();
            godID = null;

            switch (type)
            {
                case EntityType.Monster:
                    // 신 없음
                    int enemyTraitCount = Random.Range(0, 2); // 0~1
                    for (int i = 0; i < enemyTraitCount; i++)
                        traitIDs.Add(DatabaseManager.Instance.GetRandomTrait(floor).traitID);

                    if (Random.value < 0.5f) // 50% 확률로 장비 하나
                        itemIDs.Add(DatabaseManager.Instance.GetRandomEquipment(floor).itemID);
                    break;

                case EntityType.Npc:
                    godID = DatabaseManager.Instance.GetRandomGod().GodID;

                    int npcTraitCount = Random.Range(0, 1); // 2~3
                    for (int i = 0; i < npcTraitCount; i++)
                        traitIDs.Add(DatabaseManager.Instance.GetRandomTrait(floor).traitID);

                    int npcItemCount = Random.Range(0, 1); // 0~2
                    for (int i = 0; i < npcItemCount; i++)
                        itemIDs.Add(DatabaseManager.Instance.GetRandomItem(floor).itemID);
                    break;

                case EntityType.Boss:
                    // 고정 처리: godID, traitIDs는 외부에서 세팅하거나 EntitiesData.effect에 있음
                    break;
            }
        }

        public void SetStat(string key, int value)
        {
            stats[key] = value;
        }

        public void TakeDamage(int dmg)
        {
            ChangeStat("CurrentHP", -dmg);
            if (CurrentHP <= 0) SetStat("CurrentHP", 0);
        }
        
        public IEnumerator FlashOnHit(Color flashColor, float duration = 0.1f)
        {
            var image = GetComponentInChildren<Image>();
            if (image == null) yield break;

            Color original = image.color;
            image.color = flashColor;
            yield return new WaitForSeconds(duration);
            image.color = original;
        }

        public void Heal(int amount)
        {
            ChangeStat("CurrentHP", amount);
            if (CurrentHP > HP) SetStat("CurrentHP", HP);
        }

        public void ApplyEffect(IEffect effect)
        {
            effect.ApplyEffect(this);
        }

        public void ApplyEffectList(IEnumerable<IEffect> effects)
        {
            foreach (var effect in effects)
            {
                effect.ApplyEffect(this);
            }
        }

        public void ApplySelectedData()
        {
            if (!string.IsNullOrEmpty(godID))
            {
                var godData = DatabaseManager.Instance.GetGodData(godID);
                if (godData == null)
                    Debug.LogWarning($"신 효과 생성 실패 : {godID}");
                foreach (var effect in godData.SpecialEffect)
                {
                    if(effect != null)
                        ApplyEffect(effect);
                    else
                        Debug.LogWarning($"[신 효과 null] godID : {godData} 내 effect 중 null 발견");
                }
                
            }

            foreach (var traitId in traitIDs)
            {
                var traitData = DatabaseManager.Instance.GetTraitData(traitId);
                if (traitData == null)
                    Debug.LogWarning($"특성 효과 생성 실패 : {traitId}");
                foreach (var effect in traitData.traitEffect)
                {
                    if (effect != null)
                        ApplyEffect(effect);
                    else
                        Debug.LogWarning($"[특성 효과 null] traitID: {traitId} 내 effect 중 null 발견");

                }

            }

            foreach (var itemId in itemIDs)
            {
                var itemData = DatabaseManager.Instance.GetItemData(itemId);
                if (itemData == null)
                {
                    Debug.LogWarning($"[아이템ID 오류] '{itemId}'에 해당하는 ItemData 없음");
                    continue;
                }

                foreach (var effect in itemData.effects)
                {
                    if (effect != null)
                        ApplyEffect(effect);
                    else
                        Debug.LogWarning($"[아이템 효과 null] itemID: {itemId} 내 effect 중 null 발견");
                }
            }
        }

        public void ChangeStat(string statName, int value)
        {
            if (stats.ContainsKey(statName))
            {
                stats[statName] += value;
                Debug.Log($"{statName}이(가) {value} 만큼 변경됨. 현재 값: {stats[statName]}");
            }
        }

        public int GetStat(string key)
        {
            return stats.TryGetValue(key, out int value) ? value : 0;
        }

    }
}
