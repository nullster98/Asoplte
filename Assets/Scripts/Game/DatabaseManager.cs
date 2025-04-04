using System.Collections.Generic;
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
        public List<TraitData> traitList;
        public SkillDatabase skillDatabase;
        public List<FlatEventLine> eventLines;
        public List<GodData> godList;
        public List<RaceData> raceList;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;

                GodEffectRegistry.RegisterAll();
                TraitEffectRegistry.RegisterAll();
                RaceEffectRegistry.RegisterAll();
                ItemEffectRegistry.RegisterAll();
                EntityEffectRegistry.RegisterAll();
                
                itemList = JsonLoader.LoadItemData();
                godList = JsonLoader.LoadGodData();
                traitList = JsonLoader.LoadTraitData();
                eventLines = JsonLoader.LoadFlatLinesFromJson();
                raceList = JsonLoader.LoadRaceData();
                entityList = JsonLoader.LoadEntitiesData();
                
                foreach (var line in eventLines)
                {
                    Debug.Log($"[EVENT] {line.eventName} - {line.phaseName} - {line.dialogueText}");
                };

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
    }
}