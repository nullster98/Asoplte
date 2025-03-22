using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Entities
{
    [CreateAssetMenu(fileName = "EnemyDatabase", menuName = "Game/Enemy Database")]
    public class EnemyDatabase : ScriptableObject
    {
        [SerializeField] private List<EnemyData> enemyList = new List<EnemyData>();

        public ReadOnlyCollection<EnemyData> EnemyList => enemyList.AsReadOnly();

        public EnemyData GetEnemyByID(int? enemyID)
        {
            if (!enemyID.HasValue) return null;
            return enemyList.Find(enemy => enemy.EnemyID == enemyID);
        }

        public void AddEnemy(EnemyData enemy)
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

        public EnemyData GetRandomEnemy()
        {
            if (enemyList.Count != 0) return enemyList[Random.Range(0, enemyList.Count)];
            Debug.LogError("데이터베이스에 적이 없음");
            return null;

        }
    }
}
