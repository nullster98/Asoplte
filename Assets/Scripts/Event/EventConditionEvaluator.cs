using System;
using System.Collections.Generic;
using Event;
using PlayerScript;
using UnityEditor;
using UnityEngine;

public static class EventConditionEvaluator// 이벤트 조건 판별기: DSL 기반 조건문 평가 클래스
{
    // 주어진 조건이 플레이어에게 만족되는지를 평가
    public static bool IsConditionMet(EventCondition condition, Player player)
    {
        // DSL 표현식이 존재할 경우 → 파싱 후 평가
        if(!string.IsNullOrEmpty(condition.dslExpression))
        {
            try
            {
                // DSL을 파싱해 조건 트리 생성
                var node = ConditionParser.Parse(condition.dslExpression);
                return node.Evaluate(player);// 트리 평가 → true or false 반환
            }
            catch (Exception e)
            {
                Debug.LogError($"[DSL] 조건 파싱 실패 : {condition.dslExpression} -> {e.Message}");
                return false;
            }
        }

        return true;// DSL이 비어 있을 경우 조건 무조건 충족
    }
}

[Serializable]
public class EventCondition// DSL 조건 표현을 담는 클래스
{
    public string dslExpression;
}