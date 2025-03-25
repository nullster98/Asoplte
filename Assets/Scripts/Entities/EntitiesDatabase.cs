using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Entities
{
    [CreateAssetMenu(fileName = "EnemyDatabase", menuName = "Game/Enemy Database")]
    public class EntitiesDatabase : ScriptableObject
    {
        [SerializeField] private List<EntitiesData> enemyList = new List<EntitiesData>();

        public ReadOnlyCollection<EntitiesData> EnemyList => enemyList.AsReadOnly();

        public EntitiesData GetEnemyByID(int? enemyID)
        {
            if (!enemyID.HasValue) return null;
            return enemyList.Find(enemy => enemy.EnemyID == enemyID);
        }

        public void AddEnemy(EntitiesData enemy)
        {
            if (enemy == null)
            {
                Debug.LogError("EnemyDatabase: 추가하려는 EnemyData가 null입니다!");
                return;
            }

            enemyList.Add(enemy);
            Debug.Log($"적 추가 완료: {enemy.Name} (ID: {enemy.EnemyID})");
        }

        public void ResetEnemyData() => enemyList.Clear();

        public EntitiesData GetRandomEnemy()
        {
            if (enemyList.Count != 0) return enemyList[Random.Range(0, enemyList.Count)];
            Debug.LogError("데이터베이스에 적이 없음");
            return null;

        }
    }
}
