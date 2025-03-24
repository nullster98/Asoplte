using PlayerScript;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

namespace Trait
{
    public abstract class TraitEffect : IEffect
    {
        public abstract void ApplyEffect(Player player);
    }

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
        public int traitID;
        public int cost;
        public bool isUnlock;

        public Sprite GetTraitImage()
        {
            return LoadTraitSprite(fileName);
        }

        private Sprite LoadTraitSprite(string imageName)
        {
            string path = PnN switch
            {
                TraitPnN.Positive => $"Trait/Positive/Images/{imageName}",
                TraitPnN.Negative => $"Trait/Negative/Images/{imageName}",
                _ => $"Trait/Images/{imageName}"
            };
            
            Sprite traitSprite = Resources.Load<Sprite>(path);

            
            if (!traitSprite)
            {
                return Resources.Load<Sprite>("Trait/default");
            }

            return traitSprite;
        }

        public string GetDescription()
        { 
            string path= PnN switch
            {
                TraitPnN.Positive => $"Trait/Descriptions/Positive/{fileName}",
                TraitPnN.Negative => $"Trait/Descriptions/Negative/{fileName}",
                _ => $"Trait/Descriptions/{fileName}"
            };
                
            TextAsset description = Resources.Load<TextAsset>(path);
            return description != null ? description.text : "설명 없음";
        }

        public string GetUnlockHint()
        {
            string path= PnN switch
            {
                TraitPnN.Positive => $"Trait/Descriptions/Positive/{fileName}_Hint",
                TraitPnN.Negative => $"Trait/Descriptions/Negative/{fileName}_Hint",
                _ => $"Trait/Descriptions/{fileName}_Hint"
            };
                
            TextAsset description = Resources.Load<TextAsset>(path);
            return description != null ? description.text : "설명 없음";
        }

        public bool CanUnlock()
        {
            // TODO: 신앙값 또는 해금 조건 추가 필요
            return true;
        }

        public TraitData(string traitName, string fileName, TraitPnN pnN, int traitID, int cost, bool isUnlock)
        {
            this.traitName = traitName;
            this.fileName = fileName;
            this.PnN = pnN;
            this.traitID = traitID;
            this.cost = cost;
            this.isUnlock = isUnlock;
        }
    }
}
