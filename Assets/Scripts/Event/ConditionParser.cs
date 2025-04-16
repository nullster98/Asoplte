using System;
using System.Collections.Generic;

public static class ConditionParser
{
    public static IConditionNode Parse(string dsl)
    {
        // 추후 확장 가능: 괄호 처리, 우선순위, 다중 연산자
        var tokens = Tokenize(dsl);
        return ParseTokens(tokens);
    }

    private static List<string> Tokenize(string input)
    {
        var split = input.Replace("(", " ( ").Replace(")", " ) ").Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return new List<string>(split);
    }

    private static IConditionNode ParseTokens(List<string> tokens)
    {
        // 간단한 순차 파싱만 지원 (AND/OR만)
        IConditionNode current = ParseSingle(tokens);

        while (tokens.Count > 0)
        {
            string op = tokens[0];
            if (op == "AND" || op == "OR")
            {
                tokens.RemoveAt(0);
                var next = ParseSingle(tokens);
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

    private static IConditionNode ParseSingle(List<string> tokens)
    {
        string keyword = tokens[0];
        tokens.RemoveAt(0);

        switch (keyword)
        {
            case "HAS_TRAIT":
                string traitID = tokens[0];
                tokens.RemoveAt(0);
                return new HasTraitNode(traitID);

            case "FAITH":
                string faithOp = tokens[0];
                tokens.RemoveAt(0);
                int faithValue = int.Parse(tokens[0]);
                tokens.RemoveAt(0);
                return new FaithConditionNode(faithOp, faithValue);

            case "STAT":
                string statName = tokens[0];
                tokens.RemoveAt(0);
                string statOp = tokens[0];
                tokens.RemoveAt(0);
                int statVal = int.Parse(tokens[0]);
                tokens.RemoveAt(0);
                return new StatConditionNode(statName, statOp, statVal);

            case "GOD":
                tokens.RemoveAt(0); // IS
                string godID = tokens[0];
                tokens.RemoveAt(0);
                return new GodIsNode(godID);

            case "RACE":
                tokens.RemoveAt(0); // IS
                string raceID = tokens[0];
                tokens.RemoveAt(0);
                return new RaceIsNode(raceID);

            case "FLOOR":
                string floorOp = tokens[0];
                tokens.RemoveAt(0);
                int floorVal = int.Parse(tokens[0]);
                tokens.RemoveAt(0);
                return new FloorConditionNode(floorOp, floorVal);

            case "NOT":
                var inner = ParseSingle(tokens);
                return new NotConditionNode(inner);

            default:
                throw new Exception($"지원하지 않는 조건 명령어: {keyword}");
        }
    }
}
