using System.Collections.Generic;
using Entities;
using Event;
using Item;
using Skill;
using Trait;
using UnityEngine;

namespace Game
{
    public class DatabaseManager : MonoBehaviour
    {
        public static DatabaseManager Instance { get; private set; }

        public ItemDatabase itemDatabase;
        public EntitiesDatabase entitiesDatabase;
        public TraitDatabase traitDatabase;
        public SkillDatabase skillDatabase;
        public List<FlatEventLine> eventLines;

        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                ResetDatabases();
                InitializeDatabases();

                eventLines = JsonLoader.LoadFlatLinesFromJson();
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
    }
}