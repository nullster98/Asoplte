using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public static class EnemyCreator
{

    public static void InitializeEnemies(int floor)
    {
        var enemyDatabase = DatabaseManager.Instance.enemyDatabase;
        if (enemyDatabase == null)
        {
            Debug.LogError("적 데이터베이스가 null입니다! EnemyCreator에서 초기화 실패");
            return;
        }
        Debug.Log($"적 데이터베이스 정상 확인 (현재 층: {floor})");

        enemyDatabase.ResetEnemyData();
        Debug.Log($"적 데이터베이스 초기화 완료 (현재 층: {floor})");

        // 층마다 등장할 수 있는 몬스터 종류만 등록
        List<string> availableMonsters = GetAvailableMonsters(floor);
        foreach (string enemyName in availableMonsters)
        {
            EnemyData newEnemy = new EnemyData(
                id: Random.Range(1000, 9999), // 임시 ID (중복 방지)
                name: enemyName,
                type: EnemyType.Monster,
                level: 1, // 레벨은 전투 시작 시 결정됨
                maxHp: 0, // 능력치는 전투 시작 시 설정
                maxMp: 0,
                attack: 0,
                defense: 0
            );
            enemyDatabase.AddEnemy(newEnemy);
        }
    }

    [ItemCanBeNull]
    private static List<string> GetAvailableMonsters(int floor)
    {
        var floor1Monsters = new List<string> { "Skeleton" };
        var floor2Monsters = new List<string> { "Zombie", "Ghoul" };
        var floor3Monsters = new List<string> { "DarkKnight", "Lich" };


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
        var enemyDatabase = DatabaseManager.Instance.enemyDatabase;
        if (enemyDatabase == null || enemyDatabase.EnemyList.Count == 0)
        {
            Debug.LogError("적 데이터베이스가 비어 있거나 null입니다!");
            return null;
        }

        // 랜덤한 적 선택
        EnemyData baseEnemy = enemyDatabase.GetRandomEnemy();

        if (baseEnemy == null)
        {
            Debug.LogError("적을 찾을 수 없습니다!");
            return null;
        }

        // 새로운 적 객체 생성 (기존 데이터 수정 방지)
        int level = Random.Range(floor, floor + 4);
        return new EnemyData(
            id: baseEnemy.EnemyID,
            name: baseEnemy.Name,
            type: baseEnemy.NpcType,
            level: level,
            maxHp: 30 + level * 10 + Random.Range(0, 6),
            maxMp: 0, // MP 필요 시 조정
            attack: 5 + level * 2 + Random.Range(0, 3),
            defense: 2 + Mathf.RoundToInt(level * 1.5f + Random.Range(0, 2))
        );


    }

}
