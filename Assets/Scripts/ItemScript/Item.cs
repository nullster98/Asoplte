using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Equipment
{
    None,
    Helmet,
    LeftHand,
    RightHand,
    UpperBody,
    LowerBody,
    Shoes
}

[System.Serializable]
public class Item
{
    public int ItemID;
    public string ItemName;
    public string ItemDescription;
    public Equipment Slot;
    public int PurchasePrice;
    public int SalePrice;
    public Sprite ItemImg;

    // 아이템의 효과 리스트 (각 아이템이 효과를 직접 가짐)
    public List<ItemEffect> Effects = new List<ItemEffect>();

    // 생성자
    public Item(int id, string name, string description, Equipment slot, int purchasePrice, int salePrice, Sprite itemImg, List<ItemEffect> effects)
    {
        this.ItemID = id;
        this.ItemName = name;
        this.ItemDescription = description;
        this.Slot = slot;
        this.PurchasePrice = purchasePrice;
        this.SalePrice = salePrice;
        this.ItemImg = itemImg;
        this.Effects = effects ?? new List<ItemEffect>();  // 효과가 없을 수도 있음
    }

    // 아이템 사용 시 효과 적용
    public void ApplyEffects(Player player)
    {
        foreach (var effect in Effects)
        {
            effect.ApplyEffect(player);
        }
    }
}

