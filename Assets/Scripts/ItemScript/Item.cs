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

    // �������� ȿ�� ����Ʈ (�� �������� ȿ���� ���� ����)
    public List<ItemEffect> Effects = new List<ItemEffect>();

    // ������
    public Item(int id, string name, string description, Equipment slot, int purchasePrice, int salePrice, Sprite itemImg, List<ItemEffect> effects)
    {
        this.ItemID = id;
        this.ItemName = name;
        this.ItemDescription = description;
        this.Slot = slot;
        this.PurchasePrice = purchasePrice;
        this.SalePrice = salePrice;
        this.ItemImg = itemImg;
        this.Effects = effects ?? new List<ItemEffect>();  // ȿ���� ���� ���� ����
    }

    // ������ ��� �� ȿ�� ����
    public void ApplyEffects(Player player)
    {
        foreach (var effect in Effects)
        {
            effect.ApplyEffect(player);
        }
    }
}

