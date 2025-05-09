using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Utility;

namespace Item
{
    public enum ItemTarget //아이템 적용 대상
    {
        Player,
        Enemy,
        Both
    }

    public enum Rarity //희귀도
    {
        Common,
        Rare,
        Epic,
        Legendary,
        Absolute
    }

    public enum EquipmentType //장비 부위
    {
        None = -1,
        Helmet = 0,
        LeftHand = 1,
        RightHand = 2,
        UpperBody = 3,
        LowerBody = 4,
        Shoes = 5
    }

    public enum ItemType //아이템 분류
    {
        None,
        Consumable,
        Totem,
        Valuable,
        Equipment
    }

    [System.Serializable]
    public class ItemData // 아이템 구조
    {
        [Header("기본정보")]
        public string itemName;
        public string itemID;
        public ItemType itemType;
        public Rarity rarity;
        public Sprite itemImage;
        public string imagePath;
        public string codexText;
        public string codexPath;
        public List<IEffect> effects = new();
        public string EffectKey;
        public string summary;
        
        [Header("장비정보")] 
        public bool isEquipable;
        public EquipmentType equipSlot = EquipmentType.None;

        [Header("상점 정보")] 
        public int purchasePrice;
        public int salePrice;

        public void initializeEffect() // EffectKey 문자열 기반으로 효과 생성
        {
            if (string.IsNullOrWhiteSpace(EffectKey)) return;
            
            string[] effectKeys = EffectKey.Split('|');
            
            foreach (var key in effectKeys)
            {
                var trimmedKey = key.Trim();
                if (string.IsNullOrWhiteSpace(trimmedKey)) continue;
                
                var effect = EffectFactory.Create(trimmedKey);
        
                if (effect != null)
                {
                    Debug.Log($"[✅ 추가됨] {trimmedKey} → {effect.GetType().Name}");
                    effects.Add(effect);
                }
                else
                {
                    Debug.LogWarning($"[❌ 생성 실패] {trimmedKey}");
                }
            }
        }
        
        public ItemData Clone() // 아이템 복제 (효과 리스트 포함)
        {
            var copy = new ItemData
            {
                itemID = this.itemID,
                itemName = this.itemName,
                itemType = this.itemType,
                rarity = this.rarity,
                isEquipable = this.isEquipable,
                equipSlot = this.equipSlot,
                purchasePrice = this.purchasePrice,
                salePrice = this.salePrice,
                itemImage = this.itemImage, // Sprite는 공유 가능
                codexPath = this.codexPath,
                EffectKey = this.EffectKey
            };

            copy.initializeEffect(); // 효과 리스트는 새로 생성해야 함 (List<IEffect>는 참조 타입)
            return copy;
        }


    }

    
}