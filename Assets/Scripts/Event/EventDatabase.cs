using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EventDatabase", menuName = "Game/Event Database")]
public class EventDatabase : ScriptableObject
{
   public List<EventData> events = new List<EventData>();

    public EventData GetEventByName(string eventName)
    {
        
        EventData eventData = events.Find(e => e.EventName == eventName);

        if (eventData == null)
        {
            Debug.LogError($"�̺�Ʈ �����ͺ��̽����� '{eventName}'�� ã�� �� �����ϴ�!");
        }

        return eventData;
    }

    public void ResetDatabase()
    {
        events.Clear();
    }
}
