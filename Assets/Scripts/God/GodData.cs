using System.Collections.Generic;
using Entities;
using PlayerScript;
using Unity;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utility;

namespace God
{
    [System.Serializable]
    public sealed class GodData
    {
        public string GodName;
        public string FileName;
        public string GodID;
        public int UnlockCost;
        public string EffectKey;
       
        public List<IEffect> SpecialEffect = new();
        public bool IsUnlocked;
        public Sprite GodImage;
        public Sprite GodBackgroundImage;
        public string codexText;
        public string codexPath;
        public string imagePath;
        public string bgPath;
        public string summary;
        public string unlockHint;
        public void initializeEffect()
        {
            string[] effectKeys = EffectKey.Split('|');
            foreach (var key in effectKeys)
            {
                var trimmedKey = key.Trim();
                var effect = EffectFactory.Create(trimmedKey);
        
                if (effect != null)
                {
                    Debug.Log($"[✅ 추가됨] {trimmedKey} → {effect.GetType().Name}");
                    SpecialEffect.Add(effect);
                }
                else
                {
                    Debug.LogWarning($"[❌ 생성 실패] {trimmedKey}");
                }
            }
        }

        public Dictionary<string, int> GodStats = new Dictionary<string, int>
        {
            {"Atk", 0},
            {"Def", 0},
            {"HP", 0 },
            {"MP", 0 },
            {"MentalStat", 0 }
        };
        
    }
    
}