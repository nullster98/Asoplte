using PlayerScript;
using UnityEngine.Serialization;

namespace Item
{
    [System.Serializable]  // Unity 직렬화 가능하도록 설정
    public abstract class ItemEffect : IEffect
    {
        public abstract void ApplyEffect(Player player);
    }


    [System.Serializable]
    public class PoisonEffect : ItemEffect
    {
        [FormerlySerializedAs("PoisonDamage")] public int poisonDamage;
        [FormerlySerializedAs("Duration")] public int duration;

        public PoisonEffect(int poisonDamage, int duration)
        {
            this.poisonDamage = poisonDamage;
            this.duration = duration;
        }

        public override void ApplyEffect(Player player)
        {
            //player.ApplyPoison(PoisonDamage, Duration);
        }
    
        public void RemoveEffect(Player player)
        {

        }
    }
}