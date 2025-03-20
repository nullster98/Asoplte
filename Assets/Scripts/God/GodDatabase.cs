using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GodDatabase", menuName = "Game/God Database")]
public class GodDatabase : ScriptableObject
{
    public List<GodData> GodList = new List<GodData>();

    public GodData GetGodByIndex(int index)
    {
        if (index < 0 || index >= GodList.Count)
        {
            Debug.LogError("인덱스 범위를 초과하였습니다.");
            return null;
        }
        return GodList[index];
    }

    public void ResetDatabase()
    {
        GodList.Clear();
    }

}
