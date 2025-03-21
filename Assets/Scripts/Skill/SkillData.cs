using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillData : MonoBehaviour
{
    public string SkillName;
    public int SkillID;
    public string SkillDescription;
    public bool isCanUseCombat;
    public float Cooldown;
    public int ManaCost;
    public Sprite SkillImg;

    public List<SkillEffect> Effects;

    public void ActivateSkill(Player player)
    {
        if (player.GetStat("CurrentMP") < ManaCost)
        {
            Debug.Log($"{SkillName} 사용 불가! (마나 부족)");
            return;
        }

        // 마나 소모
        player.ChangeStat("CurrentMP", -ManaCost);
        Debug.Log($"{SkillName} 스킬 사용! 마나 {ManaCost} 소모.");

        // 효과 적용
        foreach (var effect in Effects)
        {
            effect.ApplyEffect(player);
        }
    }
}
