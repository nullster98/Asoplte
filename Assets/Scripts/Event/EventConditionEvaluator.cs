using System;
using System.Collections.Generic;
using Event;
using PlayerScript;
using UnityEditor;
using UnityEngine;

public static class EventConditionEvaluator
{
    public static bool IsConditionMet(EventCondition condition, Player player)
    {
        if(!string.IsNullOrEmpty(condition.dslExpression))
        {
            try
            {
                var node = ConditionParser.Parse(condition.dslExpression);
                return node.Evaluate(player);
            }
            catch (Exception e)
            {
                Debug.LogError($"[DSL] 조건 파싱 실패 : {condition.dslExpression} -> {e.Message}");
                return false;
            }
        }

        return true;
    }
}

[Serializable]
public class EventCondition
{
    public string dslExpression;
}