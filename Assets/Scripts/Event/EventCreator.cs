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
                Debug.Log($"���� �̺�Ʈ ���� : {randomEvent.EventName}");
                eventManager.StartEvent(randomEvent.EventName);
            }
            else
            {
                Debug.Log("�� �̻� ��� ������ �̺�Ʈ�� �����ϴ�");
            }
        }
    }

    public static void GenerateEvents()
    {
        if (DatabaseManager.Instance.eventDatabase == null)
        {
            Debug.LogError("EventDatabase�� ����ֽ��ϴ�.");
            return;
        }

        Debug.Log("�̺�Ʈ ���� ����");

        List<EventData> newEvents = new List<EventData>
        {
             CreateEvent("�����̺�Ʈ", EventTag.None,
                CreatePhase("�����̺�Ʈ", 
                    CreateChoice("��������", nextEvent: "END"),
                    CreateChoice("����������", nextPhaseIndex: 1)
                ),
                CreatePhase("�����̺�Ʈ2",
                    CreateChoice("��������", nextEvent: "END"),
                    CreateChoice("�䱸Ư��:�ž�", requiredTrait: "������ �ž�", nextEvent: "END")
                )
            ),

            CreateEvent("���� �̺�Ʈ", EventTag.Battle,
                CreatePhase("���� �̺�Ʈ", 
                    CreateChoice("���� ����", battleTrigger: true, fixedID: 1)
                )
            ),

            CreateEvent("���� �̺�Ʈ", EventTag.Positive,
                CreatePhase("���� �̺�Ʈ",
                    CreateChoice("���ڸ� ����", nextEvent: "END")
                 )
            )
        };


        foreach (var newEvent in newEvents)
        {
            if (!DatabaseManager.Instance.eventDatabase.events.Exists(e => e.EventName == newEvent.EventName))
            {
                DatabaseManager.Instance.eventDatabase.events.Add(newEvent);
                Debug.Log($"���ο� �̺�Ʈ �߰���: {newEvent.EventName}");
            }
        }

        SaveEventDatabase();
    }
    private static void SaveEventDatabase()
    {
        Debug.Log("EventDatabase ���� ��...");
        UnityEditor.EditorUtility.SetDirty(DatabaseManager.Instance.eventDatabase);
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
        Debug.Log("EventDatabase ���� �Ϸ�!");
    }

    //�̺�Ʈ ���� ���� �Լ�
    private static EventData CreateEvent(string name, EventTag type, params EventPhase[] phases)
    {
        return new EventData
        {
            EventName = name,
            EventType = type,
            Phases = new List<EventPhase>(phases)
        };
    }

    //�̺�Ʈ ������ ���� ���� �Լ�
    private static EventPhase CreatePhase(string phaseName, params EventChoice[] choices)
    {
        EventPhase newEventPhase = new EventPhase
        {
            PhaseName = phaseName,          
            Choices = new List<EventChoice>(choices)
        };

        newEventPhase.LoadEventImage();
        newEventPhase.GetDescription();
        return newEventPhase;
    }

    //������ ���� ���� �Լ�
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

  
}

