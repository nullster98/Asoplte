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
            Debug.Log($"{SkillName} ��� �Ұ�! (���� ����)");
            return;
        }

        // ���� �Ҹ�
        player.ChangeStat("CurrentMP", -ManaCost);
        Debug.Log($"{SkillName} ��ų ���! ���� {ManaCost} �Ҹ�.");

        // ȿ�� ����
        foreach (var effect in Effects)
        {
            effect.ApplyEffect(player);
        }
    }
}
