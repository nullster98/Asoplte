using System.Collections.Generic;
using PlayerScript;
using UnityEngine;

namespace Skill
{
    public class SkillData : MonoBehaviour
    {
        public string skillName;
        public int skillID;
        public string skillDescription;
        public bool isCanUseCombat;
        public float cooldown;
        public int manaCost;
        public Sprite skillImg;

        private readonly List<SkillEffect> Effects;

        public SkillData(List<SkillEffect> effects)
        {
            Effects = effects;
        }

        public void ActivateSkill(Player player)
        {
            if (player.GetStat("CurrentMP") < manaCost)
            {
                Debug.Log($"{skillName} 사용 불가! (마나 부족)");
                return;
            }

            // 마나 소모
            player.ChangeStat("CurrentMP", -manaCost);
            Debug.Log($"{skillName} 스킬 사용! 마나 {manaCost} 소모.");

            // 효과 적용
            foreach (var effect in Effects)
            {
                effect.ApplyEffect(player);
            }
        }
    }
}
