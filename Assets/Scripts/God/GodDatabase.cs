using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GodDatabase", menuName = "Game/God Database")]
public class GodDatabase : ScriptableObject
{
    public List<GodData> GodData = new List<GodData>();

    public GodData GetGodByID(int godID)
    {
        return GodData.Find(god => god.GodID == godID);
    }

    public void ResetDatabase()
    {
        GodData.Clear();
    }

}
