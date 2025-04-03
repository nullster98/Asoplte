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

        public ItemDatabase itemDatabase;
        public EntitiesDatabase entitiesDatabase;
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
                ResetDatabases();
                InitializeDatabases();

                GodEffectRegistry.RegisterAll();
                TraitEffectRegistry.RegisterAll();
                RaceEffectRegistry.RegisterAll();
                
                godList = JsonLoader.LoadGodData();
                traitList = JsonLoader.LoadTraitData();
                eventLines = JsonLoader.LoadFlatLinesFromJson();
                raceList = JsonLoader.LoadRaceData();
                
                foreach (var line in eventLines)
                {
                    Debug.Log($"[EVENT] {line.eventName} - {line.phaseName} - {line.dialogueText}");
                };

                DontDestroyOnLoad(gameObject);
            }
            else
            {
                //Destroy(gameObject);
            }
        }


        private void ResetDatabases()
        {
            itemDatabase?.ResetDatabase();
            entitiesDatabase?.ResetDatabase();
        }

        private void InitializeDatabases()
        {
            if (itemDatabase != null && itemDatabase.itemList.Count == 0)
            {
                ItemCreator.CreateAllItems();
                Debug.Log("모든 아이템 생성 완료!");
            }

            if (entitiesDatabase != null && entitiesDatabase.EnemyList.Count == 0) EntitiesCreator.InitializeEnemies();
        }

        public GodData GetGodByIndex(int index)
        {
            if (index < 0 || index >= godList.Count) return null;
            return godList[index];
        }

        public TraitData GetTraitByIndex(int index)
        {
            if (index < 0 || index >= traitList.Count) return null;
            return traitList[index];
        }
    }
}