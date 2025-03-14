using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnemyCreator
{

    public static void InitializeEnemies(EnemyDatabase enemyDatabase)
    {
        if (enemyDatabase == null)
        {
            Debug.LogError("�� �����ͺ��̽��� null�Դϴ�! EnemyCreator���� �ʱ�ȭ ����");
            return;
        }
        Debug.Log("�� �����ͺ��̽� ���� Ȯ��");

        enemyDatabase.ResetEnemyData();
        Debug.Log("�� �����ͺ��̽� �ʱ�ȭ �Ϸ�");

        EnemyData newEnemy = CreateEnemy(1, "Skeleton", Enemy.Monster, 30, 0, 5, 2, EventManager.Instance.floor);
        if (newEnemy != null) enemyDatabase.AddEnemy(newEnemy);
    }

    private static EnemyData CreateEnemy(int id, string name,Enemy type,int basicHP, int basicMP, int basicAtk, int basicDef, int floor)
    {
        int level = Random.Range(floor, floor + 4);

        Sprite enemySprite = Resources.Load<Sprite>($"images/Enemys/{name}");

        EnemyData newEnemy = new EnemyData(
            id,
            name,
            type,
            level,
            basicHP + level * 10 + Random.Range(0, 6),
            (basicMP == 0) ? 0 : basicMP + level * 5 + Random.Range(0, 3),
            basicAtk + level * 2 + Random.Range(0, 3),
            basicDef + Mathf.RoundToInt(level * 1.5f + Random.Range(0, 2)),
            enemySprite
        );

        return newEnemy;
    }

}
