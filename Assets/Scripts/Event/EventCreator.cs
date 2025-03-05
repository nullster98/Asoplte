using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventCreator
{
    public static void HandleNextEvent(EventHandler eventManager, string nextEventName)
    {
        if(!string.IsNullOrEmpty(nextEventName))
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

    public static void GenerateEvents(EventDatabase eventDatabase)
    {
        if(eventDatabase == null) 
        {
            Debug.LogError("EventDatabase�� ����ֽ��ϴ�.");
            return;
        }

        Debug.Log("�̺�Ʈ ���� ����");

        List<EventData> newEvents = new List<EventData>
        {
            new EventData // �����̺�Ʈ
            {
                EventName = "�����̺�Ʈ",
                EventType = EventTag.None,
                Phases = new List<EventPhase>
                {
                    new EventPhase
                    {
                        PhaseName = "�����̺�Ʈ",
                        Script = "���� ó�����۽� ����Ǵ� ��ũ��Ʈ �Դϴ�.",
                        EventImage = Resources.Load<Sprite>("images/Events/Start1"),
                        Choices = new List<EventChoice>
                        {
                            new EventChoice
                            {
                                ChoiceName = "��������",
                                NextEventName = "END"
                            },

                            new EventChoice
                            {
                                ChoiceName = "����������",
                                NextPhaseIndex = 1
                            }
                        }
                    },
                            
                    new EventPhase
                    {
                        PhaseName = "�����̺�Ʈ2",
                        Script = "���� ������� �� �Ѿ�� �����ϴ�..",
                        EventImage = Resources.Load<Sprite>("images/Events/Start2"),
                        Choices = new List<EventChoice>
                        {
                            new EventChoice
                            {
                                ChoiceName = "��������",
                                NextEventName = "END"
                            },

                            new EventChoice
                            {
                                ChoiceName = "�䱸Ư��:�ž�",
                                RequiredTraits = "������ �ž�",
                                NextEventName = "END"
                            }
                        }
                    }
                }
            }
        };


        foreach (var newEvent in newEvents)
        {
            if (!eventDatabase.events.Exists(e => e.EventName == newEvent.EventName))
            {
                eventDatabase.events.Add(newEvent);
                Debug.Log($"���ο� �̺�Ʈ �߰���: {newEvent.EventName}");
            }
        }

        SaveEventDatabase(eventDatabase);
    }
    private static void SaveEventDatabase(EventDatabase eventDatabase)
    {
        Debug.Log("EventDatabase ���� ��...");
        UnityEditor.EditorUtility.SetDirty(eventDatabase);
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
        Debug.Log("EventDatabase ���� �Ϸ�!");
    }
}
