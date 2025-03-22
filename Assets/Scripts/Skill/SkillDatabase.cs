using System.Collections.Generic;
using UnityEngine;

namespace Skill
{
    [CreateAssetMenu(fileName = "SkilDatabase", menuName = "Game/Skil Database")]
    public class SkillDatabase : ScriptableObject
    {
        public List<SkillData> skillList = new List<SkillData>();

        public void ResetSkillData()
        {
            skillList.Clear();
        }
    }
}
