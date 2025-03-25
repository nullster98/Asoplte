using System.Collections.Generic;
using Game;
using UnityEditor;
using UnityEngine;

namespace Item
{
    public class ItemCreator : MonoBehaviour
    {
        [MenuItem("Game/Create All Items")]
        public static void CreateAllItems()
        {
            Debug.Log("모든 아이템 생성 시작...");

            // 장비 아이템 (한손검)
            CreateEquipment(1001, "한손검",  1500, 800, EquipmentType.LeftHand,
                new List<IEffect>(),10,0);           

            // 장비 아이템 (불의 검)
            CreateEquipment(1002, "불의 검", 2000, 1200, EquipmentType.LeftHand,
                new List<IEffect>
                {
                    new PoisonEffect(4, 3) // 매 턴 4 데미지, 3턴 지속
                }, 7, 0);

            // 소모품 아이템 (HP 포션)
            CreateConsumable(2001, "HP 포션",  500, 250,
                new List<IEffect>(), 50, 0, ItemTarget.Player);

            // 소모품 아이템 (독 포션 - 적용)
            CreateConsumable(2002, "독 포션", 800, 400,
                new List<IEffect>
                {
                    new PoisonEffect(5, 3)
                }, 0, 0, ItemTarget.Enemy);

            Debug.Log("모든 아이템이 생성되었습니다.");
        }

        // 장비 아이템 생성 함수
        private static void CreateEquipment(int id, string name, int purchasePrice, int salePrice,
            EquipmentType slot, List<IEffect> effects, int attack, int defense)
        {
            Debug.Log($"장비 아이템 생성: {name}");

            Equipment newItem = new Equipment(id, name, purchasePrice, salePrice, slot, attack, defense, effects);
            AddItemToDatabase(newItem);
        }

        // 소모품 아이템 생성 함수
        private static void CreateConsumable(int id, string name, int purchasePrice, int salePrice,
            List<IEffect> effects, float healAmount, float manaRestore, ItemTarget target)
        {
            Debug.Log($"소모품 아이템 생성: {name}");


            Consumable newItem = new Consumable(id, name, purchasePrice, salePrice, healAmount, manaRestore, target, effects);
            AddItemToDatabase(newItem);
        }


        // 아이템을 데이터베이스에 추가하는 함수
        private static void AddItemToDatabase(ItemData itemData)
        {
            if (DatabaseManager.Instance.itemDatabase != null)
            {
                DatabaseManager.Instance.itemDatabase.itemList.Add(itemData);
                EditorUtility.SetDirty(DatabaseManager.Instance.itemDatabase);
                AssetDatabase.SaveAssets();
                Debug.Log($"{itemData.ItemName}이(가) 데이터베이스에 추가되었습니다!");
            }
            else
            {
                Debug.LogError("ItemDatabase를 찾을 수 없습니다! Resources 폴더 내 경로를 확인하세요.");
            }
        }
    }
}

