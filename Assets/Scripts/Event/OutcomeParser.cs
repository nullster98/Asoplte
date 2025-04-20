using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Event;
using UI;
using UnityEngine;

public class OutcomeParser 
{
    public class ParsedOutcome //결과 파싱을 담는곳
    {
        public List<StatModifier> statModifiers = new ();
        public List<string> addTraits = new ();
        public List<string> removeTraits = new ();
        public string rewardID;
        public string rewardType;
        public string entityID;
        public float? spawnChance = null;

        public bool battleTrigger;
        public bool rewardTrigger;
        public bool stateTrigger;
        public bool shopTrigger;
    }

    // Outcome DSL을 파싱하여 ParsedOutcome 구조로 변환
    public static ParsedOutcome Parse(string outcomeScript)
    {
        var parsed = new ParsedOutcome();
        if(string.IsNullOrWhiteSpace(outcomeScript)) return parsed;
        
        //구분자는 "|"
        var commands = outcomeScript.Split('|', StringSplitOptions.RemoveEmptyEntries);
        foreach (var rawCommand in commands)
        {
            var command = rawCommand.Trim();
            
            if(command.StartsWith("Stat:", StringComparison.OrdinalIgnoreCase))
            {
                var match = Regex.Match(command, @"^Stat:([a-zA-Z0-9_]+)([+-]\d+)$");
                if (match.Success)
                {
                    parsed.statModifiers.Add(new StatModifier
                    {
                        stat = match.Groups[1].Value,
                        amount = int.Parse(match.Groups[2].Value),
                    });
                }
            }
            // ────── Trait 추가 (랜덤 포함) ──────
            else if (command.StartsWith("Trait:add_?"))
            {
                // 예: Trait:add_?{Positive}
                var match = Regex.Match(command, @"Trait:add_\?\{(\w+)\}");
                if (match.Success)
                {
                    var category = match.Groups[1].Value; // Positive / Negative
                    parsed.addTraits.Add($"?{{{category}}}");
                }
                else
                {
                    parsed.addTraits.Add("?{Any}");
                }
            }

            // ────── Trait 제거 (랜덤 포함) ──────
            else if (command.StartsWith("Trait:remove_?"))
            {
                // 예: Trait:remove_?{Positive}
                var match = Regex.Match(command, @"Trait:remove_\?\{(\w+)\}");
                if (match.Success)
                {
                    var category = match.Groups[1].Value;
                    parsed.removeTraits.Add($"?{{{category}}}");
                }
                else
                {
                    parsed.removeTraits.Add("?{Any}");
                }
            }

            // ────── Trait 추가 (정해진 ID) ──────
            else if (command.StartsWith("Trait:add_"))
            {
                var trait = command.Replace("Trait:add_", "").Trim();
                parsed.addTraits.Add(trait);
            }

            // ────── Trait 제거 (정해진 ID) ──────
            else if (command.StartsWith("Trait:remove_"))
            {
                var trait = command.Replace("Trait:remove_", "").Trim();
                parsed.removeTraits.Add(trait);
            }
            // ────── 보상 트리거 ────── 
            else if (command.StartsWith("Reward:", StringComparison.OrdinalIgnoreCase))
            {
                var rewardPart = command.Replace("Reward:", "").Trim();
                var parts = rewardPart.Split(':');

                if (parts.Length == 2)
                {
                    parsed.rewardType = parts[0];    // Item, Equipment, Trait, Skill 중 하나
                    parsed.rewardID = parts[1];
                }
                else
                {
                    Debug.LogWarning($"[OutcomeParser] Reward 구문 오류: {command}");
                }
            }
            // ────── ID기반 Entity 소환 ────── 
            else if (command.StartsWith("Entity:", StringComparison.OrdinalIgnoreCase))
            {
                parsed.entityID = command.Replace("Entity:", "").Trim();
            }
            // ────── Entity가 소환된 확률 ────── 
            else if (command.StartsWith("Chance:", StringComparison.OrdinalIgnoreCase))
            {
                if (float.TryParse(command.Replace("Chance:", "").Trim(), out var result))
                {
                    parsed.spawnChance = result;
                }
            }
            // ────── 전투 시작(UI활성화) 트리거 ────── 
            else if (command.Equals("Trigger:StartBattle", StringComparison.OrdinalIgnoreCase))
            {
                parsed.battleTrigger = true;
            }
            // ────── 보상 지급(UI활성화) 트리거 ────── 
            else if (command.Equals("Trigger:GiveReward", StringComparison.OrdinalIgnoreCase))
            {
                parsed.rewardTrigger = true;
            }
            // ────── 상태 변화(UI활성화) 트리거 ────── 
            else if (command.Equals("Trigger:State", StringComparison.OrdinalIgnoreCase))
            {
                parsed.stateTrigger = true;
            }
            // ────── 상점 NPC 전용 상점 오픈 트리거 ────── 
            else if (command.Equals("Trigger:OpenShop", StringComparison.OrdinalIgnoreCase))
            {
                parsed.shopTrigger = true;
            }
            else
            {
                Debug.LogWarning($"[OutcomeParser] 알 수 없는 명령어: {command}");
            }
        }

        return parsed;
    }

    // ParsedOutcome → 실제 EventOutcome 변환
    public static EventOutcome ToEventOutcome(ParsedOutcome parsed)
    {
        return new EventOutcome
        {
            battleTrigger = parsed.battleTrigger,
            rewardTrigger = parsed.rewardTrigger,
            stateTrigger = parsed.stateTrigger,
            spawnEntity = !string.IsNullOrEmpty(parsed.entityID),
            entityID = parsed.entityID,
            spawnChance = parsed.spawnChance ?? 1f,
            rewardType = Enum.TryParse<AcquisitionType>(parsed.rewardType, true, out var acqType)
                ? acqType
                : null,
            rewardID = parsed.rewardID,
            modifyStat = parsed.statModifiers,
            addTrait = parsed.addTraits,
            removeTrait = parsed.removeTraits
        };
    }
}
