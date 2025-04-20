using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Game;
using God;
using Item;
using Trait;
using UnityEngine;

namespace Utility
{
    public static class EffectFactory
    {
        // 정규식 패턴 정의
        private static readonly Regex statPattern = new(@"Stat:(\w+)([+-]\d+)", RegexOptions.Compiled);
        private static readonly Regex healPattern = new(@"Heal:HP\+?(\d+)(%)?", RegexOptions.Compiled);
        
        // DSL 문자열을 기반으로 IEffect 객체 생성
        public static IEffect Create(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return null;

            // Stat:ATK+5
            if (statPattern.IsMatch(key))
            {
                var match = statPattern.Match(key);
                string stat = match.Groups[1].Value;
                int value = int.Parse(match.Groups[2].Value);
                return new StatModifierEffect(stat, value);
            }

            // Heal:HP+30 또는 Heal:HP+30%
            if (healPattern.IsMatch(key))
            {
                var match = healPattern.Match(key);
                int value = int.Parse(match.Groups[1].Value);
                bool isPercent = match.Groups[2].Success;
                return new HealEffect(value, isPercent);
            }
            
            // 등록된 고유 효과
            if (EffectRegistry.Contains(key))
            {
                return EffectRegistry.Create(key);
            }

            Debug.LogWarning($"[EffectFactory] Unknown effect key: {key}");
            return null;
        }
    }
}
