using System.Collections;
using System.Collections.Generic;
using Entities;
using Event;
using UnityEngine;

public static class EntitySpawner
{
    public static GameObject Spawn(EntitiesData baseEnemy, int floor)
    {
        EntitiesData rolled = EntitiesCreator.GenerateRandomEnemy(baseEnemy, floor);
        
        GameObject prefab = Resources.Load<GameObject>($"Prefab/EntityObject");
        if (baseEnemy == null)
        {
            Debug.LogError("EntitySpawner: 전달된 EntitiesData가 null입니다!");
        }
        
        Debug.Log("Spawner실행");

        // Canvas 찾기
        var canvas = GameObject.Find("Canvas");
        if (canvas == null)
        {
            Debug.LogError("Canvas 오브젝트를 찾을 수 없습니다!");
        }
        // Instantiate under Canvas
        GameObject entityObj = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity, canvas.transform);;
        ApplyUISpriteStandardLayout(entityObj, 500f, 500f, 350f);
        // 초기화
        EntityObject entity = entityObj.GetComponent<EntityObject>();
        if (entity != null)
        {
            entity.Initialize(rolled);
            entity.InitializeRandomLoadout(rolled.EntityType, floor);
            entity.ApplySelectedData(); // IEffect 기반 적용
        }
        //SpriteRenderer spriteRenderer = entityObj.GetComponentInChildren<SpriteRenderer>();
        EventManager.Instance.currentSpawnedEnemy = entityObj;

        return entityObj;
    }

    private static void ApplyUISpriteStandardLayout(GameObject entityObj, float width = 500f, float height = 500f, float yOffset = 350f)
    {
        var image = entityObj.GetComponentInChildren<UnityEngine.UI.Image>();
        if (image != null)
        {
            image.rectTransform.sizeDelta = new Vector2(width, height);
            Debug.Log($"[Spawner] Image sizeDelta set to ({width}, {height})");
        }

        var rect = entityObj.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(0f, yOffset);
            Debug.Log($"[Spawner] anchoredPosition set to Y: {yOffset}");
        }
    }
    
}
