using System;
using System.Collections.Generic;
using Entities;
using Trait;
using UI;
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
        public string eventName;   
        public List<EventPhase> phases;
        public EventTag eventType;
        public EventEncounter encounter;
    }

    [Serializable]
    public class EventPhase
    {
        public string phaseName;
        public Sprite eventImage;
        public List<EventChoice> choices;
        public string EventDescription {  get; set; }
        public EventOutcome phaseOutcome;

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
        public string choiceName; //선택지 이름
        public string requiredTraits; //필요 특성
        public string nextEventName;
        public int nextPhaseIndex = -1;
       
        public EventOutcome outcome;
        
        public bool CanPlayerSelect(List<TraitData> playerTraits)
        {
            if (string.IsNullOrEmpty(requiredTraits)) return true; // 필요 특성이 없으면 선택 가능
            return playerTraits.Exists(trait => trait.traitName == requiredTraits); // 플레이어가 특성을 가지고 있으면 선택 가능
        }

        public bool IsEventEnd()
        {
            return (string.IsNullOrEmpty(nextEventName) || nextEventName == "END") && nextPhaseIndex == -1;
        }
    }

    [Serializable]
    public class EventOutcome
    {
        public bool startBattle; //즉시 전투 발생
        
        public bool giveReward; //보상
        public AcquisitionType? rewardType;
        public int? rewardID;
        
        public bool spawnEntity; //우선 조우
        public EntitiesType entityType; // e.g., Enemy, NPC, Boss
        public int? entityID;
    }

    [Serializable]
    public class EventEncounter
    {
        public bool hasEncounter;
        public int enemyID;
        public bool spawnImmediately;
    }
}