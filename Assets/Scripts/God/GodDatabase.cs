using System.Collections.Generic;
using UnityEngine;

namespace God
{
    [CreateAssetMenu(fileName = "GodDatabase", menuName = "Game/God Database")]
    public class GodDatabase : ScriptableObject
    {
        public List<GodData> godList = new List<GodData>();

        public GodData GetGodByIndex(int index)
        {
            if (index >= 0 && index < godList.Count) return godList[index];
            Debug.LogError("인덱스 범위를 초과하였습니다.");
            return null;
        }

        public void ResetDatabase()
        {
            godList.Clear();
        }

    }
}
