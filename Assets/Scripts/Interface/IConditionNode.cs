using System.Collections;
using System.Collections.Generic;
using Event;
using PlayerScript;
using UnityEngine;

public interface IConditionNode
{
    bool Evaluate(Player player);
}

// 특성 보유 여부
public class HasTraitNode : IConditionNode
{
    private string traitID;

    public HasTraitNode(string traitID)
    {
        this.traitID = traitID;
    }

    public bool Evaluate(Player player)
    {
        bool result = player.HasTrait(traitID);
        if (!result) Debug.Log($"조건 불충족: HAS_TRAIT {traitID}");
        return result;
    }
}

// 신앙심 비교
public class FaithConditionNode : IConditionNode
{
    private string op;
    private int value;

    public FaithConditionNode(string op, int value)
    {
        this.op = op;
        this.value = value;
    }

    public bool Evaluate(Player player)
    {
        int faith = player.FaithStat;
        bool result = EvaluateComparison(faith, op, value);
        if (!result) Debug.Log($"조건 불충족: FAITH {op} {value} (현재: {faith})");
        return result;
    }

    private bool EvaluateComparison(int stat, string op, int val)
    {
        return op switch
        {
            ">=" => stat >= val,
            "<" => stat < val,
            "==" => stat == val,
            _ => false
        };
    }
}

// 스탯 비교
public class StatConditionNode : IConditionNode
{
    private string statName;
    private string op;
    private int value;

    public StatConditionNode(string statName, string op, int value)
    {
        this.statName = statName;
        this.op = op;
        this.value = value;
    }

    public bool Evaluate(Player player)
    {
        int stat = player.GetStat(statName);
        bool result = EvaluateComparison(stat, op, value);
        if (!result) Debug.Log($"조건 불충족: STAT {statName} {op} {value} (현재: {stat})");
        return result;
    }

    private bool EvaluateComparison(int stat, string op, int val)
    {
        return op switch
        {
            ">=" => stat >= val,
            "<" => stat < val,
            "==" => stat == val,
            _ => false
        };
    }
}

// 신 확인
public class GodIsNode : IConditionNode
{
    private string godID;

    public GodIsNode(string godID)
    {
        this.godID = godID;
    }

    public bool Evaluate(Player player)
    {
        bool result = player.selectedGod?.GodID == godID;
        if (!result) Debug.Log($"조건 불충족: GOD IS {godID} (현재: {player.selectedGod?.GodID})");
        return result;
    }
}

// 종족 확인
public class RaceIsNode : IConditionNode
{
    private string raceID;

    public RaceIsNode(string raceID)
    {
        this.raceID = raceID;
    }

    public bool Evaluate(Player player)
    {
        bool result = player.HasRace(raceID);
        if (!result) Debug.Log($"조건 불충족: RACE IS {raceID} (현재: {player.fullRaceID})");
        return result;
    }
}

// 층수 비교
public class FloorConditionNode : IConditionNode
{
    private string op;
    private int value;

    public FloorConditionNode(string op, int value)
    {
        this.op = op;
        this.value = value;
    }

    public bool Evaluate(Player player)
    {
        int floor = EventManager.Instance.floor;
        bool result = EvaluateComparison(floor, op, value);
        if (!result) Debug.Log($"조건 불충족: FLOOR {op} {value} (현재: {floor})");
        return result;
    }

    private bool EvaluateComparison(int stat, string op, int val)
    {
        return op switch
        {
            ">=" => stat >= val,
            "<" => stat < val,
            "==" => stat == val,
            _ => false
        };
    }
}

// 부정 조건
public class NotConditionNode : IConditionNode
{
    private IConditionNode inner;

    public NotConditionNode(IConditionNode inner)
    {
        this.inner = inner;
    }

    public bool Evaluate(Player player)
    {
        bool result = !inner.Evaluate(player);
        if (!result) Debug.Log($"조건 불충족: NOT 조건 내부가 true");
        return result;
    }
}

// AND 조건
public class AndConditionNode : IConditionNode
{
    private IConditionNode left, right;

    public AndConditionNode(IConditionNode left, IConditionNode right)
    {
        this.left = left;
        this.right = right;
    }

    public bool Evaluate(Player player)
    {
        bool resultL = left.Evaluate(player);
        bool resultR = right.Evaluate(player);
        if (!resultL || !resultR)
            Debug.Log("조건 불충족: AND 조건 둘 중 하나 이상 실패");
        return resultL && resultR;
    }
}

// OR 조건
public class OrConditionNode : IConditionNode
{
    private IConditionNode left, right;

    public OrConditionNode(IConditionNode left, IConditionNode right)
    {
        this.left = left;
        this.right = right;
    }

    public bool Evaluate(Player player)
    {
        bool resultL = left.Evaluate(player);
        bool resultR = right.Evaluate(player);
        if (!resultL && !resultR)
            Debug.Log("조건 불충족: OR 조건 둘 다 실패");
        return resultL || resultR;
    }
}