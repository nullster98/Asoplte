using System.Collections;
using System.Collections.Generic;
using Entities;
using UnityEngine;

public static class EntitySpawner
{
    public static GameObject Spawn(EntitiesData baseEnemy, int floor)
    {
        EntitiesData rolled = EntitiesCreator.GenerateRandomEnemy(baseEnemy, floor);
        
        GameObject prefab = Resources.Load<GameObject>($"Prefab/EnemyObject");
        if (baseEnemy == null)
        {
            Debug.LogError("EnemySpawner: 전달된 EntitiesData가 null입니다!");
        }
        
        Debug.Log("Spawner실행");

        // Canvas 찾기
        var canvas = GameObject.Find("Canvas");
        if (canvas == null)
        {
            Debug.LogError("Canvas 오브젝트를 찾을 수 없습니다!");
        }

        // Instantiate under Canvas
        GameObject enemyObj = Object.Instantiate(prefab, canvas.transform);

        // 초기화
        Enemy enemy = enemyObj.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.Initialize(rolled);
        }

        return enemyObj;
    }
}
