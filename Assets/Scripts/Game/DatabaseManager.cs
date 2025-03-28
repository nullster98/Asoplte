using System;
using Entities;
using Event;
using Item;
using Trait;
using UnityEditor;
using UnityEngine;

namespace Game
{
    public class DatabaseManager : MonoBehaviour
    {
        public static DatabaseManager Instance { get; private set; }

        public ItemDatabase itemDatabase;
        public EventDatabase eventDatabase;
        public EntitiesDatabase entitiesDatabase;
        public TraitDatabase traitDatabase;
        //public SkillDatabase skillDatabase;

        public void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
                ResetDatabases();
                InitializeDatabases();
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
            eventDatabase?.ResetDatabase();
            entitiesDatabase?.ResetDatabase();
        }

        private void InitializeDatabases()
        {
            if (itemDatabase != null && itemDatabase.itemList.Count == 0)
            {
                ItemCreator.CreateAllItems();
                Debug.Log("모든 아이템 생성 완료!");
            }

            if (eventDatabase != null && eventDatabase.events.Count == 0)
            {
                EventCreator.GenerateEvents();
                Debug.Log("모든 이벤트 생성 완료!");
            }

            if (entitiesDatabase != null && entitiesDatabase.EnemyList.Count == 0)
            {
                EntitiesCreator.InitializeEnemies();
            }

        }

    }

   
}
