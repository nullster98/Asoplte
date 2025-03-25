using System.Collections.Generic;
using Game;
using UI;
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
                CreateEvent("시작이벤트", EventTag.None, null,
                    CreatePhase("시작이벤트", null,
                        CreateChoice("다음랜덤", nextEvent: "END", nextPhaseIndex: null, requiredTrait: null, outcome: null),
                        CreateChoice("다음페이즈", nextPhaseIndex: 1)
                    ),
                    CreatePhase("시작이벤트2", null,
                        CreateChoice("다음랜덤", nextEvent: "END"),
                        CreateChoice("요구특성:신앙", requiredTrait: "굳건한 신앙", nextEvent: "END")
                    )
                ),

                CreateEvent("전투 이벤트", EventTag.Battle, null,
                    CreatePhase("전투 이벤트", phaseOutcome: null,
                        CreateChoice("전투 시작", outcome: new EventOutcome { startBattle = true })
                    )
                ),

                CreateEvent("보상 이벤트", EventTag.Positive, null,
                    CreatePhase("보상 이벤트", null,
                        CreateChoice("상자를 연다", outcome : new EventOutcome{ rewardType = AcquisitionType.Equipment, rewardID = 1001, giveReward = true})
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
        private static EventData CreateEvent(string name, EventTag type, EventEncounter encounter = null, params EventPhase[] phases)
        {
            return new EventData
            {
                eventName = name,
                eventType = type,
                encounter = encounter,
                phases = new List<EventPhase>(phases)
            };
        }

        //이벤트 페이즈 생성 헬퍼 함수
        private static EventPhase CreatePhase(string phaseName,EventOutcome phaseOutcome = null, params EventChoice[] choices)
        {
            EventPhase newEventPhase = new EventPhase
            {
                phaseName = phaseName,          
                choices = new List<EventChoice>(choices),
                phaseOutcome = phaseOutcome
            };

            newEventPhase.LoadEventImage();
            newEventPhase.GetDescription();
            return newEventPhase;
        }

        //선택지 생성 헬퍼 함수
        private static EventChoice CreateChoice(
            string choiceName, string nextEvent = null, int? nextPhaseIndex = null,
            string requiredTrait = null, EventOutcome outcome = null)
        {
            return new EventChoice
            {
                choiceName = choiceName,
                nextEventName = nextEvent,
                nextPhaseIndex = nextPhaseIndex ?? -1,
                requiredTraits = requiredTrait,
                outcome = outcome
            };
        }

  
    }
}

