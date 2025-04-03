using System.Collections.Generic;
using Entities;
using PlayerScript;
using Unity;
using UnityEngine;
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
        public string GodDescription;
        public void initializeEffect()
        {
            string[] effectKeys = EffectKey.Split(',');
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

        public void LoadGodData()
        {
            GodImage = Resources.Load<Sprite>($"God/Images/{FileName}");
            GodBackgroundImage = Resources.Load<Sprite>($"God/Backgrounds/{FileName}_BG");
            TextAsset description = Resources.Load<TextAsset>($"God/Descriptions/{FileName}");
            GodDescription = description != null ? description.text : "설명 안적음";
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