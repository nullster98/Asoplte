using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RaceDatabase", menuName = "Game/Race Database")]
public class RaceDatabase : ScriptableObject
{
    public List<RaceData> RaceList = new List<RaceData>();

    public RaceData GetGodByIndex(int index)
    {
        if (index < 0 || index >= RaceList.Count)
        {
            Debug.LogError("인덱스 범위를 초과하였습니다.");
            return null;
        }
        return RaceList[index];
    }

    public void ResetDatabase()
    {
        RaceList.Clear();
    }
}
