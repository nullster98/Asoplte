using PlayerScript;

namespace Skill
{
    [System.Serializable]
    public abstract class SkillEffect
    {
        public abstract void ApplyEffect(Player player);
    }

    [System.Serializable]
    public class DamageEffect: SkillEffect
    {

        public override void ApplyEffect(Player player)
        {
        
        }
    }

    [System.Serializable]
    public class HealEffect : SkillEffect
    {

        public override void ApplyEffect(Player player)
        {

        }
    }

    [System.Serializable]
    public class BuffEffect : SkillEffect
    {

        public override void ApplyEffect(Player player)
        {

        }
    }
}