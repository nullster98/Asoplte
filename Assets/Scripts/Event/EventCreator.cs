using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventCreator
{
    public static void HandleNextEvent(EventHandler eventManager, string nextEventName)
    {
        if (!string.IsNullOrEmpty(nextEventName))
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

    public static void GenerateEvents()
    {
        if (DatabaseManager.Instance.eventDatabase == null)
        {
            Debug.LogError("EventDatabase가 비어있습니다.");
            return;
        }

        Debug.Log("이벤트 생성 시작");

        List<EventData> newEvents = new List<EventData>
        {
             CreateEvent("시작이벤트", EventTag.None,
                CreatePhase("시작이벤트", "게임 처음 시작 시 실행되는 스크립트 입니다.", "Start1",
                    CreateChoice("다음랜덤", nextEvent: "END"),
                    CreateChoice("다음페이즈", nextPhaseIndex: 1)
                ),
                CreatePhase("시작이벤트2", "다음 페이즈로 잘 넘어왔습니다.", "Start2",
                    CreateChoice("다음랜덤", nextEvent: "END"),
                    CreateChoice("요구특성:신앙", requiredTrait: "굳건한 신앙", nextEvent: "END")
                )
            ),

            CreateEvent("전투 이벤트", EventTag.Battle,
                CreatePhase("전투 이벤트", "전투 발생 이벤트입니다.", "Battle1",
                    CreateChoice("전투 시작", battleTrigger: true, fixedID: 1)
                )
            ),

            CreateEvent("보상 이벤트", EventTag.Positive,
                CreatePhase("보상 이벤트", "랜덤 보상 이벤트입니다.", "Bonus1",
                    CreateChoice("상자를 연다", nextEvent: "END")
                 )
            )
        };


        foreach (var newEvent in newEvents)
        {
            if (!DatabaseManager.Instance.eventDatabase.events.Exists(e => e.EventName == newEvent.EventName))
            {
                DatabaseManager.Instance.eventDatabase.events.Add(newEvent);
                Debug.Log($"새로운 이벤트 추가됨: {newEvent.EventName}");
            }
        }

        SaveEventDatabase();
    }
    private static void SaveEventDatabase()
    {
        Debug.Log("EventDatabase 저장 중...");
        UnityEditor.EditorUtility.SetDirty(DatabaseManager.Instance.eventDatabase);
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
        Debug.Log("EventDatabase 저장 완료!");
    }

    //이벤트 생성 헬퍼 함수
    private static EventData CreateEvent(string name, EventTag type, params EventPhase[] phases)
    {
        return new EventData
        {
            EventName = name,
            EventType = type,
            Phases = new List<EventPhase>(phases)
        };
    }

    //이벤트 페이즈 생성 헬퍼 함수
    private static EventPhase CreatePhase(string phaseName, string script, string imageName, params EventChoice[] choices)
    {
        return new EventPhase
        {
            PhaseName = phaseName,
            Script = script,
            EventImage = LoadEventImage(imageName),
            Choices = new List<EventChoice>(choices)
        };
    }

    //선택지 생성 헬퍼 함수
    private static EventChoice CreateChoice(
        string choiceName, string nextEvent = null, int? nextPhaseIndex = null,
        string requiredTrait = null, bool battleTrigger = false, int? fixedID = null,
        bool acquisitionTrigger = false, AcquisitionType? acqType = null, int? acqID = null)
    {
        return new EventChoice
        {
            ChoiceName = choiceName,
            NextEventName = nextEvent,
            NextPhaseIndex = (int)nextPhaseIndex,
            RequiredTraits = requiredTrait,
            BattleTrigger = battleTrigger,
            FixedID = fixedID,
            AcquisitionTrigger = acquisitionTrigger,
            AcqType = acqType,
            AcqID = acqID

        };
    }

    //이벤트 이미지 로드 헬퍼 함수
    private static Sprite LoadEventImage(string imageName)
    {
        return Resources.Load<Sprite>($"images/Events/{imageName}");
    }
}

