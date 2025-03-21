using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Item;
using UnityEngine;

public enum ItemTraget
{
    Player,
    Enemy,
    Both
}

public enum EquipmentType
{
    Helmet,
    LeftHand,
    RightHand,
    UpperBody,
    LowerBody,
    Shoes
}

public enum ItemType
{
    None,
    Consumable,
    Totem,
    Valuable,
    Equipment
}

[System.Serializable]
public class ItemData
{
    public int ItemID { get; private set; }
    public string ItemName { get; private set; }
    public ItemType Type { get; private set; }
    public int PurchasePrice { get; private set; }
    public int SalePrice { get; private set; }
    public Sprite ItemImg { get; private set; }
    public List<ItemEffect> Effects { get; private set; }
    public string ItemDescription {  get; private set; }

    public string GetDescription()
    {
        string folderPath = Type switch
        {
            ItemType.Equipment => "Item/Equipment/Descriptions",
            ItemType.Consumable => "Item/Consumable/Descriptions",
            ItemType.Totem => "Item/Totem/Descriptions",
            ItemType.Valuable => "Item/Valuable/Descriptions",
            _ => "Item/Default" // 기본 폴더
        };

        TextAsset textAsset = Resources.Load<TextAsset>($"{folderPath}/{ItemName}");
        return textAsset != null ? textAsset.text : "설명 없음";
    }

    public void LoadEventImage()
    {
        string folderPath = Type switch
        {
            ItemType.Equipment => "Item/Equipment/images",
            ItemType.Consumable => "Item/Consumable/Images",
            ItemType.Totem => "Item/Totem/Images",
            ItemType.Valuable => "Item/Valuable/Images",
            _ => "Item/Default" // 기본 폴더
        };

        ItemImg = Resources.Load<Sprite>($"{folderPath}/{ItemName}");
    }

    protected ItemData(int id, string name, ItemType type, int purchasePrice, int salePrice, List<ItemEffect> effects)
    {
        (ItemID, ItemName, Type, PurchasePrice, SalePrice, Effects) =
        (id, name, type, purchasePrice, salePrice, effects ?? new List<ItemEffect>());

        ItemDescription = GetDescription(); //  설명 자동 로드
        LoadEventImage(); //  이미지 자동 로드
    }


}

public class Equipment : ItemData
{
    public int AttackPoint { get; private set; }
    public int DefensePoint { get; private set; }
    public EquipmentType EquipmentType { get; private set; }

    public Equipment(int id, string name, int purchasePrice, int salePrice,
        EquipmentType equipType, int attack, int defense, List<ItemEffect> effects)
        : base(id, name, ItemType.Equipment, purchasePrice, salePrice, effects)
        => (EquipmentType, AttackPoint, DefensePoint) = (equipType, attack, defense);
}

public class Consumable : ItemData
{
    public float HealAmount { get; private set; }
    public float ManaRestore {  get; private set; }
    public ItemTraget Target {  get; private set; }

    public Consumable(int id, string name, int purchasePrice, int salePrice,
        float healAmount, float manaRestore, ItemTraget target ,List<ItemEffect> effects)
        : base(id, name,  ItemType.Consumable ,purchasePrice, salePrice, effects)
        => (HealAmount, ManaRestore, Target) = (healAmount, manaRestore, target);
   
}

public class Totem : ItemData
{
    public float AttackPoint {  get; private set; }
    public float DefensePoint {  get; private set; }

    public Totem(int id, string name, int purchasePrice, int salePrice,
        float attack, float defense, List<ItemEffect> effects)
        : base(id, name, ItemType.Totem, purchasePrice, salePrice, effects)
        =>(AttackPoint, DefensePoint) = (attack, defense);
    
}

public class Valuable : ItemData
{
    public Valuable(int id, string name, int purchasePrice, int salePrice, List<ItemEffect> effects)
        : base(id, name, ItemType.Valuable, purchasePrice, salePrice, effects) { }
}

