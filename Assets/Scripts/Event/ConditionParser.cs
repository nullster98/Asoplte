using System;
using System.Collections.Generic;

public static class ConditionParser
{
    // DSL 문자열을 파싱하여 IConditionNode 트리로 변환
    public static IConditionNode Parse(string dsl)
    {
        // 추후 확장 가능: 괄호 처리, 우선순위 처리 등
        var tokens = Tokenize(dsl);
        return ParseTokens(tokens);
    }

    // 문자열을 토큰 리스트로 변환 (공백 기준 분리 + 괄호 처리)
    private static List<string> Tokenize(string input)
    {
        var split = input
            .Replace("(", " ( ")
            .Replace(")", " ) ")
            .Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return new List<string>(split);
    }

    // 토큰 리스트를 AND/OR 조건 중심의 노드 트리로 변환
    private static IConditionNode ParseTokens(List<string> tokens)
    {
        // 첫 조건 노드 파싱
        IConditionNode current = ParseSingle(tokens);

        // AND / OR 구문 반복 처리
        while (tokens.Count > 0)
        {
            string op = tokens[0];
            if (op == "AND" || op == "OR")
            {
                tokens.RemoveAt(0);
                var next = ParseSingle(tokens);

                // 왼쪽 → 오른쪽으로 순차적으로 트리 구성
                current = op == "AND"
                    ? new AndConditionNode(current, next)
                    : new OrConditionNode(current, next);
            }
            else
            {
                break;
            }
        }

        return current;
    }

    // 단일 조건 구문을 파싱하여 해당 노드 반환
    private static IConditionNode ParseSingle(List<string> tokens)
    {
        string keyword = tokens[0];
        tokens.RemoveAt(0);

        switch (keyword)
        {
            case "HAS_TRAIT":
                // HAS_TRAIT traitID
                string traitID = tokens[0];
                tokens.RemoveAt(0);
                return new HasTraitNode(traitID);

            case "FAITH":
                // FAITH >= 5
                string faithOp = tokens[0];
                tokens.RemoveAt(0);
                int faithValue = int.Parse(tokens[0]);
                tokens.RemoveAt(0);
                return new FaithConditionNode(faithOp, faithValue);

            case "STAT":
                // STAT HP >= 10
                string statName = tokens[0];
                tokens.RemoveAt(0);
                string statOp = tokens[0];
                tokens.RemoveAt(0);
                int statVal = int.Parse(tokens[0]);
                tokens.RemoveAt(0);
                return new StatConditionNode(statName, statOp, statVal);

            case "GOD":
                // GOD IS GOD_ID
                tokens.RemoveAt(0); // "IS"
                string godID = tokens[0];
                tokens.RemoveAt(0);
                return new GodIsNode(godID);

            case "RACE":
                // RACE IS RACE_ID
                tokens.RemoveAt(0); // "IS"
                string raceID = tokens[0];
                tokens.RemoveAt(0);
                return new RaceIsNode(raceID);

            case "FLOOR":
                // FLOOR >= 3
                string floorOp = tokens[0];
                tokens.RemoveAt(0);
                int floorVal = int.Parse(tokens[0]);
                tokens.RemoveAt(0);
                return new FloorConditionNode(floorOp, floorVal);

            case "NOT":
                // NOT 조건 (단일 노드를 부정)
                var inner = ParseSingle(tokens);
                return new NotConditionNode(inner);

            default:
                throw new Exception($"지원하지 않는 조건 명령어: {keyword}");
        }
    }
}
