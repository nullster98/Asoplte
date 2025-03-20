using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnemyCreator
{

    public static void InitializeEnemies(int floor)
    {
        if (DatabaseManager.Instance.enemyDatabase == null)
        {
            Debug.LogError("적 데이터베이스가 null입니다! EnemyCreator에서 초기화 실패");
            return;
        }
        Debug.Log($"적 데이터베이스 정상 확인 (현재 층: {floor})");

        DatabaseManager.Instance.enemyDatabase.ResetEnemyData();
        Debug.Log($"적 데이터베이스 초기화 완료 (현재 층: {floor})");

        // 층마다 등장할 수 있는 몬스터 종류만 등록
        string[] availableMonsters = GetAvailableMonsters(floor);
        foreach (string enemyName in availableMonsters)
        {
            EnemyData newEnemy = new EnemyData(
                id: Random.Range(1000, 9999), // 임시 ID (중복 방지)
                name: enemyName,
                type: Enemy.Monster,
                level: 1, // 레벨은 전투 시작 시 결정됨
                maxHP: 0, // 능력치는 전투 시작 시 설정
                maxMP: 0,
                attack: 0,
                defense: 0
            );

            newEnemy.LoadEnemySprite();   // 이미지 로드
            newEnemy.GetDescription();   //  설명 로드

            DatabaseManager.Instance.enemyDatabase.AddEnemy(newEnemy);
        }
    }

    private static string[] GetAvailableMonsters(int floor)
    {
        string[] floor1Monsters = {"Skeleton" };
        string[] floor2Monsters = {  };
        string[] floor3Monsters = {  };

        return floor switch
        {
            1 => floor1Monsters,
            2 => floor2Monsters,
            3 => floor3Monsters,
            _ => floor3Monsters // 기본적으로 가장 강한 층 몬스터
        };
    }

    public static EnemyData StartBattle(int floor)
    {
        List<EnemyData> availableEnemies = DatabaseManager.Instance.enemyDatabase.EnemyList;

        if (availableEnemies.Count == 0)
        {
            Debug.LogError("적 데이터베이스가 비어 있습니다!");
            return null;
        }

        // 랜덤한 적 선택
        EnemyData baseEnemy = availableEnemies[Random.Range(0, availableEnemies.Count)];

        // 능력치 설정 (층에 따라 변동)
        int level = Random.Range(floor, floor + 4);
        EnemyData battleEnemy = new EnemyData(
            baseEnemy.EnemyID,
            baseEnemy.Name,
            baseEnemy.NPCType,
            level,
            30 + level * 10 + Random.Range(0, 6),  // HP 증가
            (0 == 0) ? 0 : 0 + level * 5 + Random.Range(0, 3),  // MP (없을 수도 있음)
            5 + level * 2 + Random.Range(0, 3),  // 공격력 증가
            2 + Mathf.RoundToInt(level * 1.5f + Random.Range(0, 2))  // 방어력 증가
        );

        battleEnemy.EnemySprite = baseEnemy.EnemySprite;
        battleEnemy.Description = baseEnemy.Description;

        return battleEnemy;
    }

}
