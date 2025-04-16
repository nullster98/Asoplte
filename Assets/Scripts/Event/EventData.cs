using System;
using System.Collections.Generic;
using Entities;
using PlayerScript;
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
        public string eventID;
        public List<EventPhase> phases;
        public List<int> allowedFloors;
        public EventTag eventType;
        public bool isRecycle;
        public bool isUsed;
    }

    [Serializable]
    public class EventPhase
    {
        public string phaseName;
        public string phaseID;
        public string imagePath;
        public Sprite phaseImage;
        public List<DialogueBlock> dialogues;
        public EventOutcome phaseOutcome;

        public string eventID;

    }

    [Serializable]
    public class DialogueBlock
    {
        public string dialogueText;
        public string dialoguePath;
        public string dialogueID;
        public List<EventChoice> choices;
        public EventOutcome outcome;
        public EventCondition condition;
        public string nextEventID;
        public string nextPhaseID;
        public bool waitForInteraction;
        public bool isEventEnd;

        public string phaseID;
    }

    
    [Serializable]
    public class EventOutcome
    {
        //전투관련
        public bool battleTrigger;
        public bool spawnEntity; //ture = 일단 소환, false = 습격느낌
        public string entityID;
        public float spawnChance = 1.0f;
    
        //보상관련    
        public bool rewardTrigger;
        public AcquisitionType? rewardType;
        public string rewardID;
        
        //상태변화
        public bool stateTrigger;
        public List<StatModifier> modifyStat;
        public List<string> addTrait;
        public List<string> removeTrait;
        
        //NPC관련
        public bool openShop;
        
        public bool HasEffect()
        {
            return battleTrigger
                   || rewardTrigger
                   || openShop
                   || stateTrigger
                   || spawnEntity
                   || (modifyStat != null && modifyStat.Count > 0)
                   || (addTrait != null && addTrait.Count > 0)
                   || (removeTrait != null && removeTrait.Count > 0)
                   || !string.IsNullOrEmpty(entityID)
                   || rewardType.HasValue
                   || !string.IsNullOrEmpty(rewardID);
        }
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
        public string choiceID;
        public EventCondition condition;
        public string nextEventID;
        public string nextPhaseID;
        public string nextDialogueID;
       
        public EventOutcome outcome;
        
        public string dialogueID;

        public bool CanPlayerSelect(Player player)
        {
            return EventConditionEvaluator.IsConditionMet(condition, player);
        }

        public bool IsEventEnd()
        {
            return (string.IsNullOrEmpty(nextEventID) || nextEventID == "END") && (string.IsNullOrEmpty(nextPhaseID));
        }
    }
    
}