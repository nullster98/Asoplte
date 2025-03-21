using System.Collections.Generic;
using UnityEngine;

namespace Event
{
    [CreateAssetMenu(fileName = "EventDatabase", menuName = "Game/Event Database")]
    public class EventDatabase : ScriptableObject
    {
        public List<EventData> events = new List<EventData>();

        public EventData GetEventByName(string eventName)
        {
        
            EventData eventData = events.Find(e => e.eventName == eventName);

            if (eventData == null)
            {
                Debug.LogError($"이벤트 데이터베이스에서 '{eventName}'을 찾을 수 없습니다!");
            }

            return eventData;
        }

        public void ResetDatabase()
        {
            events.Clear();
        }
    }
}
