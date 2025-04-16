using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using Entities;
using Event;
using PlayerScript;
using UnityEngine;
using UnityEngine.EventSystems;

public class EntityObjectInteractionHandler : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    public EntityObject entity;
    public GameObject infoPanel;
    public EventHandler eventHandler;

    private float holdTime = 0.5f;
    private bool isHolding = false;

    void Awake()
    {
        if (eventHandler == null)
        {
            eventHandler = FindObjectOfType<EventManager>()?.eventHandler;
            if (eventHandler == null)
            {
                Debug.LogError("이벤트 핸들러 없음");
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (entity.enemyData.EntityType == EntityType.Npc)
        {
            Debug.Log("[클릭] NPC → 다이얼로그 시작");

            var reaction = entity.enemyData.EvaluateReactionTo(Player.Instance);

            int phaseIndex = reaction switch
            {
                NpcReaction.Favorable => 1,
                NpcReaction.Neutral => 2,
                NpcReaction.Hostile => 3,
                _ => -1
            };

            if (phaseIndex < 0)
            {
                Debug.LogWarning($"[❌] NpcReaction {reaction}에 대응하는 PhaseIndex 없음");
                return;
            }

            if (phaseIndex < 0)
            {
                Debug.LogWarning($"[❌] NpcReaction {reaction}에 대응하는 PhaseIndex 없음");
                return;
            }

            eventHandler.StartEvent(eventHandler.currentEvent.eventID, phaseIndex);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isHolding = true;
        StartCoroutine(HoldCheck());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isHolding = false;
    }

    private IEnumerator HoldCheck()
    {
        float timer = 0f;
        while (isHolding && timer < holdTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        if (isHolding)
        {
            // 꾹 누르기 완료 → 정보창 표시
            if (infoPanel != null)
            {
                infoPanel.SetActive(true);
                Debug.Log("[꾹 누름] 정보창 활성화");
            }
        }
    }
}
