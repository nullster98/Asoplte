using UnityEngine;
using UnityEngine.Serialization;

namespace Event
{
    public interface IEventEffect
    {
        void ApplyEffect();
    }

    [System.Serializable]
    public class BattleEffect : IEventEffect
    {
        [FormerlySerializedAs("EnemyLevel")] public int enemyLevel;

        public void ApplyEffect()
        {
            Debug.Log("전투 발생. 절 레벨 : {EnemyLevel}");
        }
    }

    [System.Serializable]
    public class ItemGainEffect : IEventEffect
    {
        [FormerlySerializedAs("ItemName")] public string itemName;

        public void ApplyEffect()
        {

        }
    }

    [System.Serializable]
    public class StatChangeEffect : IEventEffect
    {
        [FormerlySerializedAs("StatName")] public string statName;
        [FormerlySerializedAs("Amount")] public int amount;

        public void ApplyEffect()
        {

        }
    }

    public class EventEffect : MonoBehaviour
    {

    }
}