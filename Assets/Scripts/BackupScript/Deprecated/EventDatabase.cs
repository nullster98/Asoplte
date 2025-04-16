using System.Collections.Generic;
using UnityEngine;

namespace Event
{
    /*[CreateAssetMenu(fileName = "EventDatabase", menuName = "Game/Event Database")]
    public class EventDatabase : ScriptableObject
    {
        public List<EventData> events = new List<EventData>();

        public void LoadFromJson(string JsonText)
        {
            var wrapper = JsonUtility.FromJson<>(JsonText);
            events = wrapper.events;
        }

        public EventData GetEventByName(string name)
        {
            return events.Find(e => e.eventName == name);
        }
    }*/

}
