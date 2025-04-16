using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Event;
using UI;
using UnityEngine;

public class OutcomeParser 
{
    public class ParsedOutcome
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

    public static ParsedOutcome Parse(string outcomeScript)
    {
        var parsed = new ParsedOutcome();
        if(string.IsNullOrWhiteSpace(outcomeScript)) return parsed;
        
        var commands = outcomeScript.Split('|', StringSplitOptions.RemoveEmptyEntries);
        foreach (var rawCommand in commands)
        {
            var command = rawCommand.Trim();
            
            if(command.StartsWith("Stat:", StringComparison.OrdinalIgnoreCase))
            {
                var match = Regex.Match(command, @"^Stat:(\w+)([+-]\d+)");
                if (match.Success)
                {
                    parsed.statModifiers.Add(new StatModifier
                    {
                        stat = match.Groups[1].Value,
                        amount = int.Parse(match.Groups[2].Value),
                    });
                }
            }
            else if (command.StartsWith("Trait:add_"))
            {
                var trait = command.Replace("Trait:add_", "").Trim();
                parsed.addTraits.Add(trait);
            }
            else if (command.StartsWith("Trait:remove_"))
            {
                var trait = command.Replace("Trait:remove_", "").Trim();
                parsed.removeTraits.Add(trait);
            }
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
            else if (command.StartsWith("Entity:", StringComparison.OrdinalIgnoreCase))
            {
                parsed.entityID = command.Replace("Entity:", "").Trim();
            }
            else if (command.StartsWith("Chance:", StringComparison.OrdinalIgnoreCase))
            {
                if (float.TryParse(command.Replace("Chance:", "").Trim(), out var result))
                {
                    parsed.spawnChance = result;
                }
            }
            else if (command.Equals("Trigger:StartBattle", StringComparison.OrdinalIgnoreCase))
            {
                parsed.battleTrigger = true;
            }
            else if (command.Equals("Trigger:GiveReward", StringComparison.OrdinalIgnoreCase))
            {
                parsed.rewardTrigger = true;
            }
            else if (command.Equals("Trigger:State", StringComparison.OrdinalIgnoreCase))
            {
                parsed.stateTrigger = true;
            }
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
