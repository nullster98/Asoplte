using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Flags]
public enum EventTag //�̺�Ʈ �±׵�
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

    Sequential = 1 << 8 // �̺�Ʈ �ļ�

}

[System.Serializable]
public class EventData //�̺�Ʈ �⺻ ����
{
    public string EventName;   
    public List<EventPhase> Phases;
    public EventTag EventType;
}

[System.Serializable]
public class EventPhase
{
    public string PhaseName;
    public Sprite EventImage;
    public List<EventChoice> Choices;
    public string EventDescription {  get; set; }

    public void GetDescription()
    {
        TextAsset textAsset = Resources.Load<TextAsset>($"Event/Descriptions/{PhaseName}");
        EventDescription = textAsset != null ? textAsset.text : "���� ����";
    }

    //�̺�Ʈ �̹��� �ε� ���� �Լ�
    public void LoadEventImage()
    {
        EventImage = Resources.Load<Sprite>($"Event/Images/{PhaseName}");
        if (EventImage == null)
        {
            Debug.LogWarning($"{PhaseName} �̹����� ã�� �� �����ϴ�! �⺻ �̹����� ����.");
            EventImage = Resources.Load<Sprite>("Event/Images/default");
        }
    }
}

[System.Serializable]
public class EventChoice
{
    public string ChoiceName; //������ �̸�
    public string RequiredTraits; //�ʿ� Ư��
    public string NextEventName;
    public int NextPhaseIndex = -1;
    public bool BattleTrigger = false;
    public int? FixedID = -1; //�⺻�� -1(���� ����), 0(���� ��), ID(Ư�� ����)

    public bool AcquisitionTrigger = false; //���� Ʈ����
    public AcquisitionType? AcqType; //ȹ���ϰ��� Ÿ��
    public int? AcqID; //ȹ���� �������� ID

    public bool CanPlayerSelect(List<Trait> playerTraits)
    {
        if (string.IsNullOrEmpty(RequiredTraits)) return true; // �ʿ� Ư���� ������ ���� ����
        return playerTraits.Exists(trait => trait.TraitName == RequiredTraits); // �÷��̾ Ư���� ������ ������ ���� ����
    }

    public bool IsEventEnd()
    {
        return (string.IsNullOrEmpty(NextEventName) || NextEventName == "END") && NextPhaseIndex == -1;
    }
}
