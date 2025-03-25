using System.Collections.Generic;
using Entities;
using PlayerScript;
using UnityEngine;

namespace God
{
    public abstract class GodEffect : IEffect
    {
        public abstract void ApplyEffect(IUnit target);
    }

    [System.Serializable]
    public sealed class GodData
    {
        public string GodName;
        public string FileName;
        public int GodID;
        public bool IsUnlocked;
        public int UnlockCost;
        public GodEffect SpecialEffect;
        public Sprite GetBackgroundImage()
        {
            return LoadBackgroundSprite(FileName);
        }

        private Sprite LoadBackgroundSprite(string imageName)
        {
            Sprite backgroundSprite = Resources.Load<Sprite>($"God/Backgrounds/{imageName}_BG");
            return backgroundSprite != null ? backgroundSprite : Resources.Load<Sprite>("God/default_BG");
        }
        public Sprite GetGodImage()
        {
            return LoadGodSprite(FileName);
        }

        private Sprite LoadGodSprite(string imageName)
        {
            string path = $"God/Images/{imageName}";
            Sprite godSprite = Resources.Load<Sprite>(path);

            if (godSprite == null)
            {
                Debug.LogWarning($"[LoadGodSprite] ❌ 이미지 로드 실패! 경로: {path}.png 혹은 확장자 없음");
                return Resources.Load<Sprite>("God/default");
            }

            Debug.Log($"[LoadGodSprite] ✅ 성공적으로 이미지 로드됨: {path}");
            return godSprite;
        }

        public string GetDescription()
        {
            TextAsset description = Resources.Load<TextAsset>($"God/Descriptions/{FileName}");
            return description != null ? description.text : "설명 없음";
        }

        public Dictionary<string, int> GodStats = new Dictionary<string, int>
        {
            {"Atk", 0},
            {"Def", 0},
            {"HP", 0 },
            {"MP", 0 },
            {"MentalStat", 0 }
        };

        public GodData(string godName, string fileName, int godID, bool isUnlock, int unlockCost,
            GodEffect specialEffect)
        {
            this.GodName = godName;
            this.FileName = fileName;
            this.GodID = godID;
            IsUnlocked = isUnlock;
            this.UnlockCost = unlockCost;
            this.SpecialEffect = specialEffect;

            Sprite godImage = GetGodImage();
            Sprite background = GetBackgroundImage();
            string description = GetDescription();
            
            if (godImage == null)
                Debug.LogWarning($"[GodData] {godName}의 메인 이미지가 없습니다! 경로: God/Images/{fileName}");
            if (background == null)
                Debug.LogWarning($"[GodData] {godName}의 배경 이미지가 없습니다! 경로: God/Backgrounds/{fileName}_BG");
            if (description == "설명 없음")
                Debug.LogWarning($"[GodData] {godName}의 설명 텍스트를 찾을 수 없습니다! 경로: God/Descriptions/{fileName}");
        }
    }

    public class LibertyGodEffect :GodEffect
    {
        public override void ApplyEffect(IUnit target)
        {
            /*switch (target.UnitType)
            {
                case UnitType.Player ;
                    if(target is Player player)
                    break;
                case UnitType.Enemy ;
                    (if target is Enemy enemy)
                    break;
                case UnitType.NPC ;
                    break;
                
            }*/
        }
    }
}