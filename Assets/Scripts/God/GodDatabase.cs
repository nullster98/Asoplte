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
            Debug.LogError("�ε��� ������ �ʰ��Ͽ����ϴ�.");
            return null;
        }
        return GodList[index];
    }

    public void ResetDatabase()
    {
        GodList.Clear();
    }

}
