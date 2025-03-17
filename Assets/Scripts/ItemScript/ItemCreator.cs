using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class ItemCreator : MonoBehaviour
{
    [MenuItem("Game/Create All Items")]
    public static void CreateAllItems()
    {
        Debug.Log("모든 아이템 생성 시작...");

        // 장비 아이템 (한손검)
        CreateEquipment(1001, "한손검", "적을 공격하면 독 피해를 줍니다.", 1500, 800, EquipmentType.LeftHand, "한손검",
            new List<ItemEffect>(),10,0);           

        // 장비 아이템 (불의 검)
        CreateEquipment(1002, "불의 검", "불 속성 피해를 줍니다.", 2000, 1200, EquipmentType.LeftHand, "FireSword",
            new List<ItemEffect>
            {
                new PoisonEffect(4, 3) // 매 턴 4 데미지, 3턴 지속
            }, 7, 0);

        // 소모품 아이템 (HP 포션)
        CreateConsumable(2001, "HP 포션", "체력을 50 회복합니다.", 500, 250, "HP_Potion",
            new List<ItemEffect>(), 50, 0, ItemTraget.Player);

        // 소모품 아이템 (독 포션 - 적용)
        CreateConsumable(2002, "독 포션", "적에게 독을 부여합니다.", 800, 400, "Poison_Potion",
            new List<ItemEffect>
            {
                new PoisonEffect(5, 3)
            }, 0, 0, ItemTraget.Enemy);

        Debug.Log("모든 아이템이 생성되었습니다.");
    }

    // 장비 아이템 생성 함수
    private static void CreateEquipment(int id, string name, string description, int purchasePrice, int salePrice,
        EquipmentType slot, string imageName, List<ItemEffect> effects, float attack, float defense)
    {
        Debug.Log($"장비 아이템 생성: {name}");

        Sprite itemSprite = LoadItemSprite(imageName, ItemType.Equipment);

        Equipment newItem = new Equipment(id, name, description, purchasePrice, salePrice, itemSprite, slot, attack, defense, effects);
        AddItemToDatabase(newItem);
    }

    // 소모품 아이템 생성 함수
    private static void CreateConsumable(int id, string name, string description, int purchasePrice, int salePrice,
        string imageName, List<ItemEffect> effects, float healAmount, float manaRestore, ItemTraget target)
    {
        Debug.Log($"소모품 아이템 생성: {name}");

        Sprite itemSprite = LoadItemSprite(imageName, ItemType.Consumable);

        Consumable newItem = new Consumable(id, name, description, purchasePrice, salePrice, itemSprite, healAmount, manaRestore, target, effects);
        AddItemToDatabase(newItem);
    }

    // 공용 아이템 이미지 로드 함수
    private static Sprite LoadItemSprite(string imageName, ItemType type)
    {
        string folderPath = type switch
        { ItemType.Equipment => "Images/Items/Equipment",
          ItemType.Consumable => "Images/Items/Consumables",
          ItemType.Totem => "Images/Items/Totems",
          ItemType.Valuable => "Images/Items/Valuables",
            _ => "Images/Default" // 기본 폴더



        };

        Sprite itemSprite = Resources.Load<Sprite>($"{folderPath}/{imageName}");
        if (itemSprite == null)
        {
            Debug.LogWarning($"이미지 {imageName}을(를) 찾을 수 없습니다! 기본 이미지로 설정.");
            itemSprite = Resources.Load<Sprite>("Images/default");
        }
        return itemSprite;
    }

    // 아이템을 데이터베이스에 추가하는 함수
    private static void AddItemToDatabase(Item item)
    {
        ItemDatabase database = Resources.Load<ItemDatabase>("Database/ItemDatabase");
        if (database != null)
        {
            database.ItemList.Add(item);
            EditorUtility.SetDirty(database);
            AssetDatabase.SaveAssets();
            Debug.Log($"{item.ItemName}이(가) 데이터베이스에 추가되었습니다!");
        }
        else
        {
            Debug.LogError("ItemDatabase를 찾을 수 없습니다! Resources 폴더 내 경로를 확인하세요.");
        }
    }
}

