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

        public EntitiesData GetEntitiesData(string entityID) //entity 데이터 반환 [ID기반]
        {
            return entityList.Find(entity => entity.EntityID == entityID);
        }

        public ItemData GetItemData(string itemID) //item 데이터 반환 [ID기반]
        {
            return itemList.Find(item => item.itemID == itemID);
        }

        public GodData GetGodData(string godID)//god 데이터 반환 [ID기반]
        {
            return godList.Find(god => god.GodID == godID);
        }

        public TraitData GetTraitData(string traitID)//trait 데이터 반환 [ID기반]
        {
            return traitList.Find(trait => trait.traitID == traitID);
        }

        public ItemData GetRandomItem(int floor)//층수 기반 랜덤아이템 반환
        {
            return GetRandomItemByRarity(itemList, floor);
        }

        public ItemData GetRandomEquipment(int floor)//층수 기반 랜덤 장비"만" 반환
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

        public TraitData GetRandomTrait(int floor)//층수 기반 랜덤 특성 반환
        {
            return GetRandomTraitByRarity(traitList, floor);
        }

        public GodData GetRandomGod()//랜덤한 신앙 반환
        {
            return godList.GetRandom();
        }

        //층수 기반 희귀도를 반영하여 무작위 아이템 반환
        private ItemData GetRandomItemByRarity(List<ItemData> pool, int floor)
        {
            var rarity = GetRarity(floor);
            return pool.Where(i => i.rarity == rarity).ToList().GetRandom();
        }
        
        //층수 기반 희귀도를 반영하여 무작위 특성 반환
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
        }//층수 기반 희귀도 확률 정의

        private void ParseEntityList()//entityList를 enemyList / npcList로 분리
        {
            npcList = entityList.Where(e => e.EntityType == EntityType.Npc).ToList();
            enemyList = entityList.Where(e => e.EntityType == EntityType.Monster ||
                                              e.EntityType == EntityType.Boss).ToList();
        }
        
        // subRace ID 기반으로 상위 종족과 하위 종족 데이터 반환
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

        // 모든 데이터 불러오기 + 리소스 (텍스트, 이미지 등) 연결
        public void LoadAll()
        {
            // Effect 레지스트리에 등록
            GodEffectRegistry.RegisterAll();
            TraitEffectRegistry.RegisterAll();
            RaceEffectRegistry.RegisterAll();
            ItemEffectRegistry.RegisterAll();
            EntityEffectRegistry.RegisterAll();
                
            // JSON에서 데이터 로드
            itemList = JsonLoader.LoadItemData();
            godList = JsonLoader.LoadGodData();
            traitList = JsonLoader.LoadTraitData();
            eventLines = JsonLoader.LoadCompleteEventData();
            raceList = JsonLoader.LoadRaceData();
            entityList = JsonLoader.LoadEntitiesData();
            
            ParseEntityList(); // NPC/Enemy 분류
            
            // 이벤트 대사 및 이미지 리소스 로드
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
            
            // 신, 특성, 종족, 아이템별 codexPath 및 이미지 로드
            // (각 항목 별 codexText와 이미지 Sprite로 연결됨)
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