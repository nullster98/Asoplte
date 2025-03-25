using PlayerScript;
using UnityEngine.Serialization;

namespace Item
{
    [System.Serializable]  // Unity 직렬화 가능하도록 설정
    public abstract class ItemEffect : IEffect //후에 특수 기믹추가가능
    {
        public abstract void ApplyEffect(IUnit target);
    }


    [System.Serializable]
    public class PoisonEffect : ItemEffect
    {
        public int poisonDamage;
        public int duration;

        public PoisonEffect(int poisonDamage, int duration)
        {
            this.poisonDamage = poisonDamage;
            this.duration = duration;
        }

        public override void ApplyEffect(IUnit target)
        {
            //player.ApplyPoison(PoisonDamage, Duration);
        }
    
        public void RemoveEffect(IUnit target)
        {

        }
    }

    public class DoubleDamageEffect : IRemovableEffect
    {
        public void ApplyEffect(IUnit target)
        {
            
        }

        public void RemoveEffect(IUnit target)
        {
            
        }
    }
}