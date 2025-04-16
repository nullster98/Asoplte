using System.Collections.Generic;
using PlayerScript;
using UnityEngine;
using UnityEngine.Serialization;
using Utility;

namespace Race
{

    [System.Serializable]
  public class RaceData //종족 기본 뼈대(대분류)
    {
        public string raceName;
        public string fileName;
        public string raceID;
        public List<SubRaceData> subRace;
        public IEffect raceEffect;
        public string EffectKey;

        public Sprite raceImage;
        public string imagePath;
        public string codexText;
        public string codexPath;
        public string summary;
        public string unlockHint;
        
        public void InitializeRaceEffect()
        {
            var effect = EffectFactory.Create(EffectKey);
            if (effect != null)
            {
                raceEffect = effect;
                Debug.Log($"[✅ 종족 효과 적용] {EffectKey} → {effect.GetType().Name}");
            }
            else
            {
                Debug.LogWarning($"[❌ 종족 효과 생성 실패] {EffectKey}");
            }
        }

    }
    [System.Serializable]
    public class SubRaceData
    {
        public string subRaceName;
        public string fileName;
        public string parentRaceID;
        public string subRaceID;
        public int requireFaith;
        public bool isUnlocked;
        public List<IEffect> subRaceEffect = new();
        public string EffectKey;

        public Sprite subRaceImage;
        public string imagePath;
        public string codexText;
        public string codexPath;
        public string unlockHint;
        public string summary;
        
        public void initializeSubRaceEffect(IEffect baseRaceEffect = null)
        {
            subRaceEffect.Clear();

            // 1. 대종족 효과 먼저 상속
            if (baseRaceEffect != null)
            {
                subRaceEffect.Add(baseRaceEffect);
                Debug.Log($"[🧬 상속] 대종족 효과 {baseRaceEffect.GetType().Name} 상속됨");
            }

            // 2. 소종족 효과 처리
            if (!string.IsNullOrWhiteSpace(EffectKey))
            {
                string[] effectKeys = EffectKey.Split('|');

                foreach (var key in effectKeys)
                {
                    var trimmedKey = key.Trim();
                    var effect = EffectFactory.Create(trimmedKey);

                    if (effect != null)
                    {
                        subRaceEffect.Add(effect);
                        Debug.Log($"[✅ 추가됨] {trimmedKey} → {effect.GetType().Name}");
                    }
                    else
                    {
                        Debug.LogWarning($"[❌ 생성 실패] {trimmedKey} in {subRaceName}");
                    }
                }
            }
        }
        
        public bool CanUnlock(int playerFaith)
        {
            return playerFaith >= requireFaith;
        }
        
    }
}
