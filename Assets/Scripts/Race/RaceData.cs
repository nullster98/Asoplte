using System.Collections.Generic;
using PlayerScript;
using UnityEngine;
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
        public string raceDescription;
        
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

        public void LoadRaceData()
        {
            raceImage = Resources.Load<Sprite>($"Race/Images/{fileName}");
            if (raceImage == null)
                Debug.LogWarning($"[❌] Race 이미지 로드 실패: Race/Images/{fileName}");
            TextAsset description = Resources.Load<TextAsset>($"Race/Descriptions/{fileName}");
            raceDescription = description != null ? description.text : "설명 없음";
        }

    }
    [System.Serializable]
    public class SubRaceData
    {
        public string subRaceName;
        public string fileName;
        public string subRaceID;
        public int requireFaith;
        public bool isUnlocked;
        public List<IEffect> subRaceEffect = new();
        public string EffectKey;

        public Sprite subRaceImage;
        public string subRaceDescription;
        public string unlockHint;
        
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
                string[] effectKeys = EffectKey.Split(',');

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

        public void LoadSubRaceData()
        {
            subRaceImage = Resources.Load<Sprite>($"Race/SubRace/Images/{fileName}");
            if (subRaceImage == null)
                Debug.LogWarning($"[❌] SubRace 이미지 로드 실패: Race/SubRace/Images/{fileName}");
            TextAsset description = Resources.Load<TextAsset>($"Race/SubRace/Descriptions/{fileName}");
            subRaceDescription = description != null ? description.text : "설명 없음";
            TextAsset UnlockHint = Resources.Load<TextAsset>($"Race/SubRace/Descriptions/{fileName}_Hint");
            unlockHint = UnlockHint != null ? UnlockHint.text : "준비중 입니다.";
        }
        
        
        public bool CanUnlock(int playerFaith)
        {
            return playerFaith >= requireFaith;
        }
        
    }
}
