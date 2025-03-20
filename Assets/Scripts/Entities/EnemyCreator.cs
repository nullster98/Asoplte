using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnemyCreator
{

    public static void InitializeEnemies(int floor)
    {
        if (DatabaseManager.Instance.enemyDatabase == null)
        {
            Debug.LogError("�� �����ͺ��̽��� null�Դϴ�! EnemyCreator���� �ʱ�ȭ ����");
            return;
        }
        Debug.Log($"�� �����ͺ��̽� ���� Ȯ�� (���� ��: {floor})");

        DatabaseManager.Instance.enemyDatabase.ResetEnemyData();
        Debug.Log($"�� �����ͺ��̽� �ʱ�ȭ �Ϸ� (���� ��: {floor})");

        // ������ ������ �� �ִ� ���� ������ ���
        string[] availableMonsters = GetAvailableMonsters(floor);
        foreach (string enemyName in availableMonsters)
        {
            EnemyData newEnemy = new EnemyData(
                id: Random.Range(1000, 9999), // �ӽ� ID (�ߺ� ����)
                name: enemyName,
                type: Enemy.Monster,
                level: 1, // ������ ���� ���� �� ������
                maxHP: 0, // �ɷ�ġ�� ���� ���� �� ����
                maxMP: 0,
                attack: 0,
                defense: 0
            );

            newEnemy.LoadEnemySprite();   // �̹��� �ε�
            newEnemy.GetDescription();   //  ���� �ε�

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
            _ => floor3Monsters // �⺻������ ���� ���� �� ����
        };
    }

    public static EnemyData StartBattle(int floor)
    {
        List<EnemyData> availableEnemies = DatabaseManager.Instance.enemyDatabase.EnemyList;

        if (availableEnemies.Count == 0)
        {
            Debug.LogError("�� �����ͺ��̽��� ��� �ֽ��ϴ�!");
            return null;
        }

        // ������ �� ����
        EnemyData baseEnemy = availableEnemies[Random.Range(0, availableEnemies.Count)];

        // �ɷ�ġ ���� (���� ���� ����)
        int level = Random.Range(floor, floor + 4);
        EnemyData battleEnemy = new EnemyData(
            baseEnemy.EnemyID,
            baseEnemy.Name,
            baseEnemy.NPCType,
            level,
            30 + level * 10 + Random.Range(0, 6),  // HP ����
            (0 == 0) ? 0 : 0 + level * 5 + Random.Range(0, 3),  // MP (���� ���� ����)
            5 + level * 2 + Random.Range(0, 3),  // ���ݷ� ����
            2 + Mathf.RoundToInt(level * 1.5f + Random.Range(0, 2))  // ���� ����
        );

        battleEnemy.EnemySprite = baseEnemy.EnemySprite;
        battleEnemy.Description = baseEnemy.Description;

        return battleEnemy;
    }

}
