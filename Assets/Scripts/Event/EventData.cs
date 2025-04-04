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
        public bool isRecycle;
        public bool isUsed;
    }

    [Serializable]
    public class EventPhase
    {
        public string phaseName;
        public string backgroundImageName;
        public Sprite backgroundImage;
        public List<DialogueBlock> dialouges;
        public EventOutcome phaseOutcome;

   
        
    }

    [Serializable]
    public class DialogueBlock
    {
        public string dialogueText;
        public List<EventChoice> choices;
        public EventOutcome outcome;
        public bool waitForInteraction;
    }

    
    [Serializable]
    public class EventOutcome
    {
        //전투관련
        public bool startBattle; //즉시 전투 발생
        public bool spawnEntity;
        public string entityID;
        public float spawnChance = 1.0f;
    
        //보상관련    
        public bool giveReward; //보상
        public AcquisitionType? rewardType;
        public string rewardID;
        
        //상태변화
        public bool affectPlayerState;
        public List<StatModifier> modifyStat;
        public List<string> addTrait;
        public List<string> removeTrait;
    }
    
    public class StatModifier
    {
        public string stat;
        public int amount;
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
    
}