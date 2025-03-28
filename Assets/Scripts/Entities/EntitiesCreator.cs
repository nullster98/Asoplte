using System.Collections.Generic;
using Game;
using JetBrains.Annotations;
using UnityEngine;

namespace Entities
{
    public static class EntitiesCreator
    {

        public static void InitializeEnemies()
        {
            var enemyDatabase = DatabaseManager.Instance.entitiesDatabase;
            if (enemyDatabase == null)
            {
                Debug.LogError("적 데이터베이스가 null입니다! EnemyCreator에서 초기화 실패");
                return;
            }

            enemyDatabase.ResetDatabase();
            
            enemyDatabase.AddEnemy(
            new EntitiesData(3001, "Skeleton", EntityType.Monster, 
                100, 0, 10, 3, 
                new List<int>{1}, false)
            );
            
        }
        

        public static EntitiesData GenerateRandomEnemy(EntitiesData baseEnemy, int floor)
        {
            int level = Random.Range(floor, floor + 4);
            
            return new EntitiesData(
                id: baseEnemy.EntityID,
                name: baseEnemy.Name,
                type: baseEnemy.EntityType,
                maxHp: 30 + level * 10 + Random.Range(0, 6),
                maxMp: 0, // MP 필요 시 조정
                attack: 5 + level * 2 + Random.Range(0, 3),
                defense: 2 + Mathf.RoundToInt(level * 1.5f + Random.Range(0, 2)),
                spawnableFloors: baseEnemy.SpawnableFloors,
                isEventOnly: baseEnemy.IsEventOnly
            );


        }

    }
}
