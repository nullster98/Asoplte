using System.Collections.Generic;
using Game;
using JetBrains.Annotations;
using UnityEngine;

namespace Entities
{
    public static class EntitiesCreator
    {

        public static void InitializeEnemies(int floor)
        {
            var enemyDatabase = DatabaseManager.Instance.entitiesDatabase;
            if (enemyDatabase == null)
            {
                Debug.LogError("적 데이터베이스가 null입니다! EnemyCreator에서 초기화 실패");
                return;
            }
            Debug.Log($"적 데이터베이스 정상 확인 (현재 층: {floor})");

            enemyDatabase.ResetEnemyData();
            Debug.Log($"적 데이터베이스 초기화 완료 (현재 층: {floor})");
            
            enemyDatabase.AddEnemy(
            new EntitiesData(3001, "Skeleton", EntityType.Monster, 
                100, 0, 10, 3, 
                new List<int>{1}, false)
            );
            
        }
        

        public static EntitiesData StartBattle(int floor)
        {
            var enemyDatabase = DatabaseManager.Instance.entitiesDatabase;
            if (enemyDatabase == null || enemyDatabase.EnemyList.Count == 0)
            {
                Debug.LogError("적 데이터베이스가 비어 있거나 null입니다!");
                return null;
            }

            // 랜덤한 적 선택
            EntitiesData baseEnemy = enemyDatabase.GetRandomEnemy(floor);

            if (baseEnemy == null)
            {
                Debug.LogError("적을 찾을 수 없습니다!");
                return null;
            }

            // 새로운 적 객체 생성 (기존 데이터 수정 방지)
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
