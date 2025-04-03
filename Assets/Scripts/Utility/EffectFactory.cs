using God;
using Item;
using Trait;
using UnityEngine;

namespace Utility
{
    public static class EffectFactory
    {
        public static IEffect Create(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                Debug.LogWarning("[EffectFactory] 키가 null 또는 공백입니다.");
                return null;
            }

            if (EffectRegistry.Contains(key))
            {
                return EffectRegistry.Create(key);
            }

            if (StatEffectParser.CanParse(key))
            {
                return StatEffectParser.Parse(key);
            }

            Debug.LogWarning($"[EffectFactory] 등록되지 않은 키: {key}");
            return null;
        }
    }
}
