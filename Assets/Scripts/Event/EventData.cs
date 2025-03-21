using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Event
{
    [Flags]
    public enum EventTag //이벤트 태그들
    {
        None = 0,
        Start = 1 << 0, // 1
        Positive = 1 << 1, // 2
        Negative = 1 << 2, // 4
        Battle = 1 << 3, // 8
        Chaos = 1 << 4, // 16
        Encounter = 1 << 5, // 32
        Rest = 1 << 6, // 64
        Boss = 1 << 7, // 128

        Sequential = 1 << 8 // 이벤트 후속

    }

    [Serializable]
    public class EventData //이벤트 기본 뼈대
    {
        [FormerlySerializedAs("EventName")] public string eventName;   
        [FormerlySerializedAs("Phases")] public List<EventPhase> phases;
        [FormerlySerializedAs("EventType")] public EventTag eventType;
    }

    [Serializable]
    public class EventPhase
    {
        [FormerlySerializedAs("PhaseName")] public string phaseName;
        [FormerlySerializedAs("EventImage")] public Sprite eventImage;
        [FormerlySerializedAs("Choices")] public List<EventChoice> choices;
        public string EventDescription {  get; set; }

        public void GetDescription()
        {
            TextAsset textAsset = Resources.Load<TextAsset>($"Event/Descriptions/{phaseName}");
            EventDescription = textAsset != null ? textAsset.text : "설명 없음";
        }

        //이벤트 이미지 로드 헬퍼 함수
        public void LoadEventImage()
        {
            eventImage = Resources.Load<Sprite>($"Event/Images/{phaseName}");
            if (eventImage == null)
            {
                Debug.LogWarning($"{phaseName} 이미지를 찾을 수 없습니다! 기본 이미지로 설정.");
                eventImage = Resources.Load<Sprite>("Event/Images/default");
            }
        }
    }

    [Serializable]
    public class EventChoice
    {
        [FormerlySerializedAs("ChoiceName")] public string choiceName; //선택지 이름
        [FormerlySerializedAs("RequiredTraits")] public string requiredTraits; //필요 특성
        [FormerlySerializedAs("NextEventName")] public string nextEventName;
        [FormerlySerializedAs("NextPhaseIndex")] public int nextPhaseIndex = -1;
        [FormerlySerializedAs("BattleTrigger")] public bool battleTrigger;
        public int? FixedID = -1; //기본값 -1(조우 없음), 0(랜덤 적), ID(특정 조우)

        [FormerlySerializedAs("AcquisitionTrigger")] public bool acquisitionTrigger; //보상 트리거
        public AcquisitionType? AcqType; //획득하게할 타입
        public int? AcqID; //획득할 아이템의 ID

        public bool CanPlayerSelect(List<Trait> playerTraits)
        {
            if (string.IsNullOrEmpty(requiredTraits)) return true; // 필요 특성이 없으면 선택 가능
            return playerTraits.Exists(trait => trait.TraitName == requiredTraits); // 플레이어가 특성을 가지고 있으면 선택 가능
        }

        public bool IsEventEnd()
        {
            return (string.IsNullOrEmpty(nextEventName) || nextEventName == "END") && nextPhaseIndex == -1;
        }
    }
}