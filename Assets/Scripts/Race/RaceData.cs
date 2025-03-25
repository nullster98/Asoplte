using System.Collections.Generic;
using PlayerScript;
using UnityEngine;

namespace Race
{
    public abstract class RaceEffect : IEffect
    {
        public abstract void ApplyEffect(IUnit target);
    }

    [System.Serializable]
  public class RaceData //종족 기본 뼈대(대분류)
    {
        public string raceName;
        public string fileName;
        public List<SubRaceData> subRace;
        public RaceEffect raceEffect;

        public Sprite GetRaceImage()
        {
            return LoadRaceSprite(fileName);
        }

        private Sprite LoadRaceSprite(string imageName)
        {
            string path = $"Race/Images/{imageName}";
            Sprite tribeSprite = Resources.Load<Sprite>(path);

            if (tribeSprite == null)
            {
                return Resources.Load<Sprite>("Race/default");
            }

            return tribeSprite;
        }

        public string GetDescription()
        {
            TextAsset description = Resources.Load<TextAsset>($"Race/Descriptions/{fileName}");
            return description != null ? description.text : "설명 없음";
        }
    }
[System.Serializable]
    public class SubRaceData
    {
        public string subRaceName;
        public string fileName;
        public string unlockHint;
        public int requireFaith;
        public bool isUnlocked;
        public RaceEffect subRaceEffect;
        
        public Sprite GetSubRaceImage()
        {
            return LoadSubRaceSprite(fileName);
        }

        private Sprite LoadSubRaceSprite(string imageName)
        {
            string path = $"Race/SubRace/Images/{imageName}";
            Sprite subRaceSprite = Resources.Load<Sprite>(path);

            if (subRaceSprite == null)
            {
                return Resources.Load<Sprite>("Race/default");
            }

            return subRaceSprite;
        }

        public string GetDescription()
        {
            TextAsset description = Resources.Load<TextAsset>($"Race/SubRace/Descriptions/{fileName}");
            return description != null ? description.text : "설명 없음";
        }

        public string GetUnlockHint()
        {
            TextAsset UnlockHint = Resources.Load<TextAsset>($"Race/SubRace/Descriptions/{fileName}_Hint");
            return UnlockHint != null ? UnlockHint.text : "준비중 입니다.";
        }
        
        public bool CanUnlock(int playerFaith)
        {
            return playerFaith >= requireFaith;
        }
        
    }
}
