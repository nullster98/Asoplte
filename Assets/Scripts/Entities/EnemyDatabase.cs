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
            Debug.LogError("EnemyDatabase: 추가하려는 EnemyData가 null입니다!");
            return;
        }

        if (!(enemy is EnemyData))
        {
            Debug.LogError("EnemyDatabase: 추가하려는 데이터가 EnemyData 타입이 아닙니다! 현재 타입: " + enemy.GetType());
            return;
        }

        EnemyList.Add(enemy);
        Debug.Log($"적 추가 완료: {enemy.Name} (ID: {enemy.EnemyID})");
    }

    public void ResetEnemyData()
    {
        EnemyList.Clear();
    }

    public EnemyData GetRandomEnemy()
    {
        if(EnemyList.Count == 0)
        {
            Debug.LogError("데이터베이스에 적이 없음");
            return null;
        }

        int randomIndex = Random.Range(0, EnemyList.Count);
        return EnemyList[randomIndex];
    }

}
