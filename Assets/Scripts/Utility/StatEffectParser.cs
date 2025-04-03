using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public static class StatEffectParser
{
   private static readonly Regex pattern = new(@"Stat:(\w+)([+-]\d+)", RegexOptions.Compiled);
   
   public static bool CanParse(string key)
   {
      return pattern.IsMatch(key);
   }

   public static IEffect Parse(string key)
   {
      var match = pattern.Match(key);
      if (!match.Success) return null;

      string stat = match.Groups[1].Value;           // "Atk"
      int value = int.Parse(match.Groups[2].Value);  // "+3" or "-2" → 자동 부호 포함

      return new StatModifierEffect(stat, value);
   }
}
