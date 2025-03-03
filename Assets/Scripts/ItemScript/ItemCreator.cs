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

        CreateItem(1001, "한손검", "적을 공격하면 독 피해를 줍니다.", 1500, 800, Equipment.LeftHand, "한손검",
            new List<ItemEffect>
            {
                new EquipEffect(new List<EquipEffect.StatModifier> { new EquipEffect.StatModifier { StatName = "Atk", Value = 5 } }),
                new PoisonEffect(3, 2)  // 매 턴 3 데미지, 2턴 지속
            });

        CreateItem(1002, "불의 검", "불 속성 피해를 줍니다.", 2000, 1200, Equipment.LeftHand, "FireSword",
            new List<ItemEffect>
            {
                new EquipEffect(new List<EquipEffect.StatModifier> { new EquipEffect.StatModifier { StatName = "Atk", Value = 7 } }),
                new PoisonEffect(4, 3)  // 매 턴 4 데미지, 3턴 지속
            });

        Debug.Log("모든 아이템이 생성되었습니다.");
    }

    private static void CreateItem(int id, string name, string description, int purchasePrice, int salePrice,
     Equipment slot, string imageName, List<ItemEffect> effects)
    {
        Debug.Log($"아이템 생성: {name}");

        // 아이템 이미지 로드 (없으면 기본 이미지 사용)
        Sprite itemSprite = Resources.Load<Sprite>($"Images/Items/Weapon/{imageName}");
        if (itemSprite == null)
        {
            Debug.LogWarning($"이미지 {imageName}을(를) 찾을 수 없습니다! 기본 이미지로 설정.");
            itemSprite = Resources.Load<Sprite>("Images/Items/Weapon/default");  // 기본 이미지 설정

            if (itemSprite == null)
            {
                Debug.LogError("기본 이미지도 찾을 수 없습니다!");
            }
        }

        // Item생성
        Item newItem = new Item(id, name, description, slot, purchasePrice, salePrice,
           itemSprite, effects);

        // ItemDatabase에 추가
        ItemDatabase database = Resources.Load<ItemDatabase>("Database/ItemDatabase");
        if (database != null)
        {
            database.ItemList.Add(newItem);  // 'ItemInfo'를 추가 (오류 해결)
            EditorUtility.SetDirty(database);
            AssetDatabase.SaveAssets();
            Debug.Log($"{name}이(가) 데이터베이스에 추가되었습니다!");
        }
        else
        {
            Debug.LogError("ItemDatabase를 찾을 수 없습니다! Resources 폴더 내 경로를 확인하세요.");
        }
    }
}

