using System.Collections.Generic;
using System.Linq;
using Entities;
using Event;
using God;
using Item;
using JetBrains.Annotations;
using Race;
using Skill;
using Trait;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game
{
    public class DatabaseManager : MonoBehaviour
    {
        public static DatabaseManager Instance { get; private set; }

        public List<ItemData> itemList;
        public List<EntitiesData> entityList;
        public List<EntitiesData> npcList;
        public List<EntitiesData> enemyList;
        public List<TraitData> traitList;
        public List<EventData> eventLines;
        public List<GodData> godList;
        public List<RaceData> raceList;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                
                LoadAll();

                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public EntitiesData GetEntitiesData(string entityID)
        {
            return entityList.Find(entity => entity.EntityID == entityID);
        }

        public ItemData GetItemData(string itemID)
        {
            return itemList.Find(item => item.itemID == itemID);
        }

        public GodData GetGodData(string godID)
        {
            return godList.Find(god => god.GodID == godID);
        }

        public TraitData GetTraitData(string traitID)
        {
            return traitList.Find(trait => trait.traitID == traitID);
        }

        public ItemData GetRandomItem(int floor)
        {
            return GetRandomItemByRarity(itemList, floor);
        }

        public ItemData GetRandomEquipment(int floor)
        {
            var pool = itemList.Where(i => i.itemType == ItemType.Equipment).ToList();
            if (pool.Count == 0)
            {
                return null;
            }

            var rarity = GetRarity(floor);
            var filtered = pool.Where(i => i.rarity == rarity).ToList();

            if (filtered.Count == 0)
            {
                filtered = pool.Where(i => i.rarity == Rarity.Common).ToList();
                
                if (filtered.Count == 0)
                {
                    Debug.LogWarning($"[DatabaseManager] Common도 없음 → 전체 풀에서 무작위 장비 지급");
                    return pool.GetRandom();
                }
            }

            return filtered.GetRandom();
        }

        public TraitData GetRandomTrait(int floor)
        {
            return GetRandomTraitByRarity(traitList, floor);
        }

        public GodData GetRandomGod()
        {
            return godList.GetRandom();
        }

        private ItemData GetRandomItemByRarity(List<ItemData> pool, int floor)
        {
            var rarity = GetRarity(floor);
            return pool.Where(i => i.rarity == rarity).ToList().GetRandom();
        }
        
        private TraitData GetRandomTraitByRarity(List<TraitData> pool, int floor)
        {
            var rarity = GetRarity(floor);
            return pool.Where(i => i.rarity == rarity).ToList().GetRandom();
        }

        private Rarity GetRarity(int floor)
        {
            Dictionary<Rarity, int> weights;

            if (floor >= 5)
            {
                weights = new()
                {
                    { Rarity.Common, 45 },
                    { Rarity.Rare, 30 },
                    { Rarity.Epic, 15 },
                    { Rarity.Legendary, 8 },
                    { Rarity.Absolute, 2 }
                };
            }
            else if (floor >= 3)
            {
                weights = new()
                {
                    { Rarity.Common, 45 },
                    { Rarity.Rare, 30 },
                    { Rarity.Epic, 15 },
                    { Rarity.Legendary, 10 }
                };
            }
            else
            {
                {
                    weights = new()
                    {
                        { Rarity.Common, 50 },
                        { Rarity.Rare, 30 },
                        { Rarity.Epic, 20 }
                    };
                }
            }

            int totalWeight = weights.Values.Sum();
            int roll = Random.Range(0, totalWeight);
            int cumulative = 0;

            foreach (var pair in weights)
            {
                cumulative += pair.Value;
                if(roll < cumulative)
                    return pair.Key;
            }

            return weights.Keys.First();
        }

        private void ParseEntityList()
        {
            npcList = entityList.Where(e => e.EntityType == EntityType.Npc).ToList();
            enemyList = entityList.Where(e => e.EntityType == EntityType.Monster ||
                                              e.EntityType == EntityType.Boss).ToList();
        }
        
        public (RaceData race, SubRaceData sub) GetSubRaceByID(string subRaceID)
        {
            foreach (var race in raceList)
            {
                foreach (var sub in race.subRace)
                {
                    if (sub.subRaceID == subRaceID)
                        return (race, sub);
                }
            }
            return (null, null);
        }

        public void LoadAll()
        {
            GodEffectRegistry.RegisterAll();
            TraitEffectRegistry.RegisterAll();
            RaceEffectRegistry.RegisterAll();
            ItemEffectRegistry.RegisterAll();
            EntityEffectRegistry.RegisterAll();
                
            itemList = JsonLoader.LoadItemData();
            godList = JsonLoader.LoadGodData();
            traitList = JsonLoader.LoadTraitData();
            eventLines = JsonLoader.LoadCompleteEventData();
            raceList = JsonLoader.LoadRaceData();
            entityList = JsonLoader.LoadEntitiesData();
            ParseEntityList();
            
            foreach(var e in eventLines)
            {
                foreach (var phase in e.phases)
                {
                    if (!string.IsNullOrEmpty(phase.imagePath))
                    {
                        phase.phaseImage = Resources.Load<Sprite>(phase.imagePath);
                    }
                    
                    foreach (var dialogue in phase.dialogues)
                    {
                        if (!string.IsNullOrEmpty(dialogue.dialoguePath))
                        {
                            var dialoguetext = Resources.Load<TextAsset>(dialogue.dialoguePath);
                            if(dialoguetext != null)
                                dialogue.dialogueText = dialoguetext.text;
                            else
                                Debug.LogWarning($"[DatabaseManager] TextAsset not found: {dialogue.dialoguePath}");
                        }
                    }
                }
            }
            
            
            foreach (var god in godList)
            {
                if (!string.IsNullOrEmpty(god.codexPath))
                {
                    var codex = Resources.Load<TextAsset>(god.codexPath);
                    god.codexText = codex.text;
                }

                if (!string.IsNullOrEmpty(god.imagePath))
                {
                    god.GodImage = Resources.Load<Sprite>(god.imagePath);
                }

                if (!string.IsNullOrEmpty(god.bgPath))
                {
                    god.GodBackgroundImage = Resources.Load<Sprite>(god.bgPath);
                }
            }
            
            foreach (var trait in traitList)
            {
                if (!string.IsNullOrEmpty(trait.codexPath))
                {
                    var codex = Resources.Load<TextAsset>(trait.codexPath);
                    trait.codexText = codex.text;
                }

                if (!string.IsNullOrEmpty(trait.imagePath))
                {
                    trait.traitImage = Resources.Load<Sprite>(trait.imagePath);
                }
            }
            
            foreach (var race in raceList)
            {
                if (!string.IsNullOrEmpty(race.codexPath))
                {
                    var codex = Resources.Load<TextAsset>(race.codexPath);
                    race.codexText = codex.text;
                }

                if (!string.IsNullOrEmpty(race.imagePath))
                {
                    race.raceImage = Resources.Load<Sprite>(race.imagePath);
                }

                if (race.subRace != null)
                {
                    foreach (var subRace in race.subRace)
                    {
                        Debug.Log($"[🔍 경로 확인] {subRace.subRaceID} → codexPath: '{subRace.codexPath}'");

                        if (!string.IsNullOrEmpty(subRace.codexPath))
                        {
                            var codex = Resources.Load<TextAsset>(subRace.codexPath);
                            if (codex != null)
                            {
                                subRace.codexText = codex.text;
                                Debug.Log($"✅ [로드 성공] {subRace.subRaceID}: {subRace.codexPath}");
                            }
                            else
                            {
                                Debug.LogWarning($"❌ [로드 실패] {subRace.subRaceID}: '{subRace.codexPath}' 경로에 TextAsset이 없음");
                            }
                        }
                        else
                        {
                            Debug.LogWarning($"⚠️ [codexPath 없음] subRaceID: {subRace.subRaceID} → codexPath가 비어 있음");
                        }

                        if (!string.IsNullOrEmpty(subRace.imagePath))
                        {
                            var sprite = Resources.Load<Sprite>(subRace.imagePath);
                            if (sprite != null)
                            {
                                subRace.subRaceImage = sprite;
                            }
                            else
                            {
                                Debug.LogWarning($"❌ [이미지 없음] {subRace.subRaceID}: '{subRace.imagePath}' 경로에 이미지 없음");
                            }
                        }
                    }
                }
            }
            
            foreach (var item in itemList)
            {
                if (!string.IsNullOrEmpty(item.codexPath))
                {
                    var codex = Resources.Load<TextAsset>(item.codexPath);
                    if (codex != null)
                    {
                        item.codexText = codex.text;
                        Debug.Log($"[Item Codex ✅] Loaded codex for '{item.itemID}' from path: '{item.codexPath}'");
                    }
                    else
                    {
                        Debug.LogWarning($"[Item Codex ❌] Failed to load codex for '{item.itemID}' from path: '{item.codexPath}'");
                    }
                }
                else
                {
                    Debug.LogWarning($"[Item Codex ❌] Failed to load codex for '{item.itemID}' from path: '{item.codexPath}'");
                }

                if (!string.IsNullOrEmpty(item.imagePath))
                {
                    item.itemImage = Resources.Load<Sprite>(item.imagePath);
                }
            }
        }
    }
}