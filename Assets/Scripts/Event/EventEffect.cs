using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEventEffect
{
    void ApplyEffect();
}

[System.Serializable]
public class BattleEffect : IEventEffect
{
    public int EnemyLevel;

    public void ApplyEffect()
    {
        Debug.Log("전투 발생. 절 레벨 : {EnemyLevel}");
    }
}

[System.Serializable]
public class ItemGainEffect : IEventEffect
{
    public string ItemName;

    public void ApplyEffect()
    {

    }
}

[System.Serializable]
public class StatChangeEffect : IEventEffect
{
    public string StatName;
    public int Amount;

    public void ApplyEffect()
    {

    }
}

public class EventEffect : MonoBehaviour
{

}
