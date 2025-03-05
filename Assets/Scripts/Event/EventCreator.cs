using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventCreator
{
    public static void HandleNextEvent(EventHandler eventManager, string nextEventName)
    {
        if(!string.IsNullOrEmpty(nextEventName))
        {
            eventManager.StartEvent(nextEventName);
        }
        else
        {
            EventData randomEvent = eventManager.GetRandomEvent();
            if (randomEvent != null)
            {
                Debug.Log($"랜덤 이벤트 실행 : {randomEvent.EventName}");
                eventManager.StartEvent(randomEvent.EventName);
            }
            else
            {
                Debug.Log("더 이상 사용 가능한 이벤트가 없습니다");
            }
        }
    }

    public static void GenerateEvents(EventDatabase eventDatabase)
    {
        if(eventDatabase == null) 
        {
            Debug.LogError("EventDatabase가 비어있습니다.");
            return;
        }

        Debug.Log("이벤트 생성 시작");

        List<EventData> newEvents = new List<EventData>
        {
            new EventData // 시작이벤트
            {
                EventName = "시작이벤트",
                EventType = EventTag.None,
                Phases = new List<EventPhase>
                {
                    new EventPhase
                    {
                        PhaseName = "시작이벤트",
                        Script = "게임 처음시작시 실행되는 스크립트 입니다.",
                        EventImage = Resources.Load<Sprite>("images/Events/Start1"),
                        Choices = new List<EventChoice>
                        {
                            new EventChoice
                            {
                                ChoiceName = "다음랜덤",
                                NextEventName = "END"
                            },

                            new EventChoice
                            {
                                ChoiceName = "다음페이즈",
                                NextPhaseIndex = 1
                            }
                        }
                    },
                            
                    new EventPhase
                    {
                        PhaseName = "시작이벤트2",
                        Script = "다음 페이즈로 잘 넘어와 졌습니다..",
                        EventImage = Resources.Load<Sprite>("images/Events/Start2"),
                        Choices = new List<EventChoice>
                        {
                            new EventChoice
                            {
                                ChoiceName = "다음랜덤",
                                NextEventName = "END"
                            },

                            new EventChoice
                            {
                                ChoiceName = "요구특성:신앙",
                                RequiredTraits = "굳건한 신앙",
                                NextEventName = "END"
                            }
                        }
                    }
                }
            }
        };


        foreach (var newEvent in newEvents)
        {
            if (!eventDatabase.events.Exists(e => e.EventName == newEvent.EventName))
            {
                eventDatabase.events.Add(newEvent);
                Debug.Log($"새로운 이벤트 추가됨: {newEvent.EventName}");
            }
        }

        SaveEventDatabase(eventDatabase);
    }
    private static void SaveEventDatabase(EventDatabase eventDatabase)
    {
        Debug.Log("EventDatabase 저장 중...");
        UnityEditor.EditorUtility.SetDirty(eventDatabase);
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
        Debug.Log("EventDatabase 저장 완료!");
    }
}
