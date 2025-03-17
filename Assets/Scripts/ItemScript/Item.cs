using System.Collections;
using System.Collections.Generic;
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
public class Item
{
    public int ItemID { get; private set; }
    public string ItemName { get; private set; }
    public string ItemDescription { get; private set; }
    public ItemType Type { get; private set; }
    public int PurchasePrice { get; private set; }
    public int SalePrice { get; private set; }
    public Sprite ItemImg { get; private set; }
    public List<ItemEffect> Effects { get; private set; }

    // 한 줄로 간결화된 생성자
    protected Item(int id, string name, string description, ItemType type, int purchasePrice, int salePrice, Sprite img, List<ItemEffect> effects)
        => (ItemID, ItemName, ItemDescription, Type, PurchasePrice, SalePrice, ItemImg, Effects)
        = (id, name, description, type, purchasePrice, salePrice, img, effects ?? new List<ItemEffect>());

  
}

public class Equipment : Item
{
    public float AttackPoint { get; private set; }
    public float DefensePoint { get; private set; }
    public EquipmentType EquipmentType { get; private set; }

    public Equipment(int id, string name, string description, int purchasePrice, int salePrice, Sprite img,
        EquipmentType equipType, float attack, float defense, List<ItemEffect> effects)
        : base(id, name, description, ItemType.Equipment, purchasePrice, salePrice, img, effects)
        => (EquipmentType, AttackPoint, DefensePoint) = (equipType, attack, defense);
}

public class Consumable : Item
{
    public float HealAmount { get; private set; }
    public float ManaRestore {  get; private set; }
    public ItemTraget Target {  get; private set; }

    public Consumable(int id, string name, string description, int purchasePrice, int salePrice, Sprite img,
        float healAmount, float manaRestore, ItemTraget target ,List<ItemEffect> effects)
        : base(id, name, description, ItemType.Consumable ,purchasePrice, salePrice, img, effects)
        => (HealAmount, ManaRestore, Target) = (healAmount, manaRestore, target);
   
}

public class Totem : Item
{
    public float AttackPoint {  get; private set; }
    public float DefensePoint {  get; private set; }

    public Totem(int id, string name, string description, int purchasePrice, int salePrice, Sprite img,
        float attack, float defense, List<ItemEffect> effects)
        : base(id, name, description, ItemType.Totem, purchasePrice, salePrice, img, effects)
        =>(AttackPoint, DefensePoint) = (attack, defense);
    
}

public class Valuable : Item
{
    public Valuable(int id, string name, string description, int purchasePrice, int salePrice, Sprite img, List<ItemEffect> effects)
        : base(id, name, description, ItemType.Valuable, purchasePrice, salePrice, img, effects) { }
}

