using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkilDatabase", menuName = "Game/Skil Database")]
public class SkillDatabase : ScriptableObject
{
   public List<SkillData> SkillList = new List<SkillData>();

    public void ResetSkillData()
    {
        SkillList.Clear();
    }
}
