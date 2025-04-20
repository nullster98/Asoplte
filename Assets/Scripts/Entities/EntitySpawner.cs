using Entities;
using Event;
using UnityEngine;

public static class EntitySpawner
{
    // 적 또는 NPC를 Canvas 위에 생성하고 초기화하는 함수
    public static GameObject Spawn(EntitiesData baseEnemy, int floor)
    {
        // baseEnemy 데이터를 바탕으로 능력치/효과가 반영된 EntitiesData 생성
        EntitiesData rolled = EntitiesCreator.GenerateRandomEnemy(baseEnemy, floor);
        
        // 프리팹 로드
        GameObject prefab = Resources.Load<GameObject>($"Prefab/EntityObject");
        if (baseEnemy == null)
        {
            Debug.LogError("EntitySpawner: 전달된 EntitiesData가 null입니다!");
        }
        
        Debug.Log("Spawner실행");

        // Canvas 오브젝트 찾기
        var canvas = GameObject.Find("Canvas");
        if (canvas == null)
        {
            Debug.LogError("Canvas 오브젝트를 찾을 수 없습니다!");
        }

        // Canvas 하위에 프리팹 생성
        GameObject entityObj = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity, canvas.transform);

        // UI 위치 및 크기 설정
        ApplyUISpriteStandardLayout(entityObj, 500f, 500f, 350f);

        // EntityObject 컴포넌트 초기화
        EntityObject entity = entityObj.GetComponent<EntityObject>();
        if (entity != null)
        {
            // 스탯/이미지 적용
            entity.Initialize(rolled);

            // 랜덤 특성/아이템/신앙 부여
            entity.InitializeRandomLoadout(rolled.EntityType, floor);

            // 효과 적용 (IEffect 기반)
            entity.ApplySelectedData();
        }

        // 전역 이벤트 매니저에 현재 스폰된 적 등록
        EventManager.Instance.currentSpawnedEnemy = entityObj;

        return entityObj;
    }

    // UI Image 컴포넌트 기준으로 사이즈와 위치 조정
    private static void ApplyUISpriteStandardLayout(GameObject entityObj, float width = 500f, float height = 500f, float yOffset = 350f)
    {
        // 이미지 크기 설정
        var image = entityObj.GetComponentInChildren<UnityEngine.UI.Image>();
        if (image != null)
        {
            image.rectTransform.sizeDelta = new Vector2(width, height);
            Debug.Log($"[Spawner] Image sizeDelta set to ({width}, {height})");
        }

        // 위치 설정 (캔버스 정중앙에서 위로 yOffset만큼)
        var rect = entityObj.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(0f, yOffset);
            Debug.Log($"[Spawner] anchoredPosition set to Y: {yOffset}");
        }
    }
}
