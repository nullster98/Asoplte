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
                CreatePhase("�����̺�Ʈ", "���� ó�� ���� �� ����Ǵ� ��ũ��Ʈ �Դϴ�.", "Start1",
                    CreateChoice("��������", nextEvent: "END"),
                    CreateChoice("����������", nextPhaseIndex: 1)
                ),
                CreatePhase("�����̺�Ʈ2", "���� ������� �� �Ѿ�Խ��ϴ�.", "Start2",
                    CreateChoice("��������", nextEvent: "END"),
                    CreateChoice("�䱸Ư��:�ž�", requiredTrait: "������ �ž�", nextEvent: "END")
                )
            ),

            CreateEvent("���� �̺�Ʈ", EventTag.Battle,
                CreatePhase("���� �̺�Ʈ", "���� �߻� �̺�Ʈ�Դϴ�.", "Battle1",
                    CreateChoice("���� ����", battleTrigger: true, fixedID: 1)
                )
            ),

            CreateEvent("���� �̺�Ʈ", EventTag.Positive,
                CreatePhase("���� �̺�Ʈ", "���� ���� �̺�Ʈ�Դϴ�.", "Bonus1",
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

    //�̺�Ʈ �̹��� �ε� ���� �Լ�
    private static Sprite LoadEventImage(string imageName)
    {
        return Resources.Load<Sprite>($"images/Events/{imageName}");
    }
}

