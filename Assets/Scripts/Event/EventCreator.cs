using System.Collections.Generic;
using UnityEngine;

namespace Event
{
    public abstract class EventCreator
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
                    Debug.Log($"랜덤 이벤트 실행 : {randomEvent.eventName}");
                    eventManager.StartEvent(randomEvent.eventName);
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
                    CreatePhase("시작이벤트", 
                        CreateChoice("다음랜덤", nextEvent: "END"),
                        CreateChoice("다음페이즈", nextPhaseIndex: 1)
                    ),
                    CreatePhase("시작이벤트2",
                        CreateChoice("다음랜덤", nextEvent: "END"),
                        CreateChoice("요구특성:신앙", requiredTrait: "굳건한 신앙", nextEvent: "END")
                    )
                ),

                CreateEvent("전투 이벤트", EventTag.Battle,
                    CreatePhase("전투 이벤트", 
                        CreateChoice("전투 시작", battleTrigger: true, fixedID: 1)
                    )
                ),

                CreateEvent("보상 이벤트", EventTag.Positive,
                    CreatePhase("보상 이벤트",
                        CreateChoice("상자를 연다", nextEvent: "END")
                    )
                )
            };


            foreach (var newEvent in newEvents)
            {
                if (!DatabaseManager.Instance.eventDatabase.events.Exists(e => e.eventName == newEvent.eventName))
                {
                    DatabaseManager.Instance.eventDatabase.events.Add(newEvent);
                    Debug.Log($"새로운 이벤트 추가됨: {newEvent.eventName}");
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
                eventName = name,
                eventType = type,
                phases = new List<EventPhase>(phases)
            };
        }

        //이벤트 페이즈 생성 헬퍼 함수
        private static EventPhase CreatePhase(string phaseName, params EventChoice[] choices)
        {
            EventPhase newEventPhase = new EventPhase
            {
                phaseName = phaseName,          
                choices = new List<EventChoice>(choices)
            };

            newEventPhase.LoadEventImage();
            newEventPhase.GetDescription();
            return newEventPhase;
        }

        //선택지 생성 헬퍼 함수
        private static EventChoice CreateChoice(
            string choiceName, string nextEvent = null, int? nextPhaseIndex = null,
            string requiredTrait = null, bool battleTrigger = false, int? fixedID = null,
            bool acquisitionTrigger = false, AcquisitionType? acqType = null, int? acqID = null)
        {
            return new EventChoice
            {
                choiceName = choiceName,
                nextEventName = nextEvent,
                nextPhaseIndex = (int)nextPhaseIndex,
                requiredTraits = requiredTrait,
                battleTrigger = battleTrigger,
                FixedID = fixedID,
                acquisitionTrigger = acquisitionTrigger,
                AcqType = acqType,
                AcqID = acqID

            };
        }

  
    }
}

