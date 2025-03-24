using Entities;
using Event;
using Item;
using Trait;
using UnityEngine;

namespace Game
{
    public class DatabaseManager : MonoBehaviour
    {
        public static DatabaseManager Instance { get; private set; }

        public ItemDatabase itemDatabase;
        public EventDatabase eventDatabase;
        public EnemyDatabase enemyDatabase;
        public TraitDatabase traitDatabase;
        //public SkillDatabase skillDatabase;

        public void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

    }

   
}
