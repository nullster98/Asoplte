using System.Collections.Generic;
using Item;
using PlayerScript;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Serialization;
using Utility;

namespace Trait
{
    public enum TraitPnN
    {
        Positive,
        Negative
    }
    [System.Serializable]
    public class TraitData
    {
        public string traitName;
        public string fileName;
        public TraitPnN PnN;
        public Rarity rarity;
        public string traitID;
        public int cost;
        public bool isUnlock;
        public string EffectKey;
        public List<IEffect> traitEffect = new ();

        public Sprite traitImage;
        public string codexText;
        public string codexPath;
        public string imagePath;
        public string unlockHint;
        public string summary;
        
        public bool CanUnlock()
        {
            // TODO: 신앙값 또는 해금 조건 추가 필요
            return true;
        }

        public void initializeEffect()
        {
            traitEffect.Clear();
            if (string.IsNullOrWhiteSpace(EffectKey))
            {
                Debug.LogWarning($"[TraitData] '{traitID}'의 EffectKey가 비어 있음.");
                return;
            }
            string[] effectKeys = EffectKey.Split('|');
            foreach (var key in effectKeys)
            {
                var trimmedKey = key.Trim();
                var effect = EffectFactory.Create(trimmedKey);
        
                if (effect != null)
                {
                    Debug.Log($"[✅ 추가됨] {trimmedKey} → {effect.GetType().Name}");
                    traitEffect.Add(effect);
                }
                else
                {
                    Debug.LogWarning($"[❌ 생성 실패] {trimmedKey}");
                }
            }
            if (traitEffect.Count == 0)
            {
                Debug.LogWarning($"[TraitData ⚠️] '{traitID}'에 등록된 효과가 하나도 없습니다.");
            }
        }
        
    }
}
