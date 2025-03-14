using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Flags]
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

[System.Serializable]
public class EventData //이벤트 기본 뼈대
{
    public string EventName;   
    public List<EventPhase> Phases;
    public EventTag EventType;
}

[System.Serializable]
public class EventPhase
{
    public string PhaseName;
    public string Script;
    public Sprite EventImage;
    public List<EventChoice> Choices;
}

[System.Serializable]
public class EventChoice
{
    public string ChoiceName; //선택지 이름
    public string RequiredTraits; //필요 특성
    public string NextEventName;
    public int NextPhaseIndex = -1;
    public bool BattleTrigger = false;
    public int FixedID = -1; //기본값 -1(조우 없음), 0(랜덤 적), ID(특정 조우)

    public bool CanPlayerSelect(List<Trait> playerTraits)
    {
        if (string.IsNullOrEmpty(RequiredTraits)) return true; // 필요 특성이 없으면 선택 가능
        return playerTraits.Exists(trait => trait.TraitName == RequiredTraits); // 플레이어가 특성을 가지고 있으면 선택 가능
    }

    public bool IsEventEnd()
    {
        return (string.IsNullOrEmpty(NextEventName) || NextEventName == "END") && NextPhaseIndex == -1;
    }
}
