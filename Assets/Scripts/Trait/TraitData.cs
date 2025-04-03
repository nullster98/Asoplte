using System.Collections.Generic;
using PlayerScript;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
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
        public string traitID;
        public int cost;
        public bool isUnlock;
        public string EffectKey;
        public List<IEffect> traitEffect = new ();

        public Sprite traitImage;
        public string traitDescription;
        public string unlockHint;


        public void LoadTraitData()
        {
            string path = PnN switch
            {
                TraitPnN.Positive => $"Trait/Positive/Images/{fileName}",
                TraitPnN.Negative => $"Trait/Negative/Images/{fileName}",
                _ => $"Trait/default"
            };
            traitImage = Resources.Load<Sprite>(path);

            string Dpath = PnN switch
            {
                TraitPnN.Positive => $"Trait/Positive/Descriptions/{fileName}",
                TraitPnN.Negative => $"Trait/Negative/Descriptions/{fileName}",
                _ => $"Trait/default"
            };
            var descAsset = Resources.Load<TextAsset>(Dpath);
            traitDescription = descAsset != null ? descAsset.text : "(설명 없음)";
            if (descAsset == null) Debug.LogWarning($"[TraitData] 설명 파일 없음: {Dpath}");
            
            string Hpath= PnN switch
            {
                TraitPnN.Positive => $"Trait/Descriptions/Positive/{fileName}_Hint",
                TraitPnN.Negative => $"Trait/Descriptions/Negative/{fileName}_Hint",
                _ => $"Trait/default"
            };
            var hintAsset = Resources.Load<TextAsset>(Hpath);
            unlockHint = hintAsset != null ? hintAsset.text : "(힌트 없음)";
            if (hintAsset == null) Debug.LogWarning($"[TraitData] 힌트 파일 없음: {Hpath}");
        }

        public bool CanUnlock()
        {
            // TODO: 신앙값 또는 해금 조건 추가 필요
            return true;
        }

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
                    traitEffect.Add(effect);
                }
                else
                {
                    Debug.LogWarning($"[❌ 생성 실패] {trimmedKey}");
                }
            }
        }
        
    }
}
