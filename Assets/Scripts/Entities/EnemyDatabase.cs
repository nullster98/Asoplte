using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDatabase", menuName = "Game/Enemy Database")]
public class EnemyDatabase : ScriptableObject
{
    public List<EnemyData> EnemyList = new List<EnemyData>();

    public EnemyData GetEnemyByID(int enemyID)
    {
        return EnemyList.Find(enemy => enemy.EnemyID == enemyID);
    }

    public void AddEnemy(EnemyData enemy)
    {
        if (enemy == null)
        {
            Debug.LogError("EnemyDatabase: �߰��Ϸ��� EnemyData�� null�Դϴ�!");
            return;
        }

        if (!(enemy is EnemyData))
        {
            Debug.LogError("EnemyDatabase: �߰��Ϸ��� �����Ͱ� EnemyData Ÿ���� �ƴմϴ�! ���� Ÿ��: " + enemy.GetType());
            return;
        }

        EnemyList.Add(enemy);
        Debug.Log($"�� �߰� �Ϸ�: {enemy.Name} (ID: {enemy.EnemyID})");
    }

    public void ResetEnemyData()
    {
        EnemyList.Clear();
    }

    public EnemyData GetRandomEnemy()
    {
        if(EnemyList.Count == 0)
        {
            Debug.LogError("�����ͺ��̽��� ���� ����");
            return null;
        }

        int randomIndex = Random.Range(0, EnemyList.Count);
        return EnemyList[randomIndex];
    }

}
