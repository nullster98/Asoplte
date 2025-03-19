using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodData : MonoBehaviour
{
    public string GodName;
    public string Description;
    public float GodID;
    public bool IsUnlocked = false;
    public int UnlockCost;
    public Sprite GodImage;

    public Dictionary<string, float> GodStats = new Dictionary<string, float>
    {
        {"Atk", 0},
        {"Def", 0},
        {"HP", 0 },
        {"MP", 0 },
        {"MentalStat", 0 }
    };
}
