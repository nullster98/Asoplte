using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHandler
{

    public EventDatabase eventDatabase;
    private EventData currentEvent;
    private int currentPhaseIndex = 0;

    public EventHandler(EventDatabase dataabase)
    {
        this.eventDatabase = dataabase;
    }

    public void StartEvent(string eventName)
    {
        if (eventDatabase == null)
        {
            Debug.LogError("이벤트 데이터베이스가 존재하지 않습니다!");
            return;
        }

        currentEvent = eventDatabase.GetEventByName(eventName);

        if (currentEvent == null)
        {
            Debug.LogError($"이벤트 '{eventName}'을 찾을 수 없습니다!");
            return;
        }

        Debug.Log($"이벤트 시작: {eventName}");

        currentPhaseIndex = 0;
        ProcessPhase();
    }

    private void ProcessPhase()
    {
        if (currentEvent == null || currentEvent.Phases == null || currentEvent.Phases.Count == 0)
        {
            Debug.LogWarning("이벤트에 유효한 페이즈가 없습니다! 이벤트 종료.");
            HandleEventEnd();
            return;
        }

        if (currentPhaseIndex >= currentEvent.Phases.Count)
        {
            Debug.Log("이벤트 종료");
            HandleEventEnd();
            return;
        }

        EventPhase phase = currentEvent.Phases[currentPhaseIndex];

        Debug.Log($"이벤트 진행 중: {phase.PhaseName}");

        List<EventChoice> availableChoices = new List<EventChoice>();

        foreach (var choice in phase.Choices)
        {
            if (choice.CanPlayerSelect(Player.Instance.selectedTraits))
            {
                availableChoices.Add(choice);
            }
        }

        Debug.Log("UI 업데이트 호출");
        EventManager.Instance.UpdateEventUI(phase.Script, availableChoices, phase.EventImage);

        currentPhaseIndex++;
    }

    public EventData GetRandomEvent()
    {
        if (eventDatabase == null || eventDatabase.events.Count == 0)
        {
            Debug.LogWarning("이벤트 데이터베이스가 비어 있습니다!");
            return null;
        }

        int randomIndex = Random.Range(0, eventDatabase.events.Count);
        return eventDatabase.events[randomIndex]; // 중복 여부 상관없이 무작위 이벤트 선택
    }

    public void OnChoiceSelected(int choiceIndex)
    {
        EventPhase currentPhase = currentEvent.Phases[currentPhaseIndex - 1];
        EventChoice choice = currentPhase.Choices[choiceIndex];

        if (choice.NextEventName == "전투")
        {
            Debug.Log("전투 시작!");
            //BattleManager.Instance.StartBattle();
            return;
        }

        if (choice.NextPhaseIndex != -1) //  특정 페이즈로 이동할 경우
        {
            MoveToNextPhase(choice.NextPhaseIndex); // 기존 `StartEvent()` 대신 `MoveToNextPhase()` 사용
            return;
        }

        if (choice.IsEventEnd())
        {
            HandleEventEnd();
        }
        else
        {
            StartEvent(choice.NextEventName);
        }
    }

    private void HandleEventEnd()
    {
        Debug.Log("이벤트가 종료되었습니다. 랜덤 이벤트 실행!");

        EventData randomEvent = GetRandomEvent();
        if (randomEvent != null)
        {
            StartEvent(randomEvent.EventName);
        }
        else
        {
            Debug.Log("사용 가능한 랜덤 이벤트가 없습니다!");
        }
    }

    public void MoveToNextPhase(int nextPhaseIndex)
    {
        if (currentEvent == null)
        {
            Debug.LogError("현재 진행 중인 이벤트가 없습니다!");
            return;
        }

        if (nextPhaseIndex < 0 || nextPhaseIndex >= currentEvent.Phases.Count)
        {
            Debug.LogError($"잘못된 페이즈 인덱스: {nextPhaseIndex} (총 페이즈 개수: {currentEvent.Phases.Count})");
            return;
        }

        Debug.Log($"페이즈 이동: {currentPhaseIndex} → {nextPhaseIndex}");
        currentPhaseIndex = nextPhaseIndex; // 페이즈 인덱스를 직접 설정
        ProcessPhase(); // 다음 페이즈 실행
    }
}
