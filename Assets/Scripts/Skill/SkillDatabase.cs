using System.Collections.Generic;
using UnityEngine;

namespace Skill
{
    [CreateAssetMenu(fileName = "SkilDatabase", menuName = "Game/Skil Database")]
    public class SkillDatabase : ScriptableObject
    {
        public List<SkillData> skillList = new List<SkillData>();

        public SkillData GetSkillByID(int ID)
        {
            if (ID < 0 || ID >= skillList.Count)
            {
                return null;
            }
            return skillList[ID];
        }
        public void ResetSkillData()
        {
            skillList.Clear();
        }
    }
}
