using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class EntityJsonLine
{
    public string Name;
    public string Type;
    public string EntityID;
    public int MaxHP;
    public int MaxMP;
    public int Attack;
    public int Defense;
    public List<int> SpawnFloor;
    public bool IsEventOnly;
    public string Effect;
}
