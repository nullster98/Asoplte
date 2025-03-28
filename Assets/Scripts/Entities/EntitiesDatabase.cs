using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace Entities
{
    [CreateAssetMenu(fileName = "EntityDatabase", menuName = "Game/Entity Database")]
    public class EntitiesDatabase : ScriptableObject
    {
        [SerializeField] private List<EntitiesData> enemyList = new List<EntitiesData>();

        public ReadOnlyCollection<EntitiesData> EnemyList => enemyList.AsReadOnly();

        public EntitiesData GetEnemyByID(int? enemyID)
        {
            if (!enemyID.HasValue) return null;
            return enemyList.Find(enemy => enemy.EntityID == enemyID);
        }

        public void AddEnemy(EntitiesData enemy)
        {
            if (enemy == null)
            {
                Debug.LogError("EnemyDatabase: 추가하려는 EnemyData가 null입니다!");
                return;
            }

            enemyList.Add(enemy);
            Debug.Log($"적 추가 완료: {enemy.Name} (ID: {enemy.EntityID})");
            Debug.Log($"적 데이터 : {enemy.EnemySprite}");
        }

        public void ResetDatabase() => enemyList.Clear();

        public EntitiesData GetRandomEnemy(int floor)
        {
            var spawnable = enemyList
                .Where(e => !e.IsEventOnly && e.SpawnableFloors.Contains(floor))
                .ToList();

            if (spawnable.Count == 0) return null;
            return spawnable[Random.Range(0, spawnable.Count)];

        }
    }
}
