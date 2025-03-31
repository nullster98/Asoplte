using System;
using System.Collections.Generic;
using Game;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Item
{
    public class InventoryManager : MonoBehaviour
    {
        public static InventoryManager Instance { get; private set; }
        
        private ItemData[] inventory = new ItemData[4]; // 4칸 고정 인벤토리
        [SerializeField] private GameObject inventoryContainer; // 인벤토리 UI 패널
        [SerializeField] private GameObject equipContainer; // 인벤토리 UI 패널
        [SerializeField] private Transform inventoryContainerTransform; // 아이템 UI 위치
        [SerializeField] private GameObject itemPrefab; // 아이템 프리팹
        [SerializeField] private Sprite defaultSlotImage;

        private List<GameObject> itemUIList = new List<GameObject>(); // UI 아이템 리스트

        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else Destroy(gameObject);
        }

        public void AddItem(int itemID)
        {
            int slot = -1;
            for (int i = 0; i < inventory.Length; i++)
            {
                if (inventory[i] == null)
                {
                    slot = i;
                    break;
                }
            }

            if (slot == -1)
            {
                Debug.Log("모든 슬롯이 꽉 찼습니다!");
                return;
            }

            ItemData itemDataInfo = DatabaseManager.Instance.itemDatabase.GetItemByID(itemID);
            if (itemDataInfo == null)
            {
                Debug.Log("아이템 정보 없음");
                return;
            }

            if (itemDataInfo is Equipment)
            {
                Debug.Log("장비 아이템은 인벤토리에 추가할 수 없습니다.");
                return;
            }

            ItemData newItemData = null;

            if (itemDataInfo is Consumable consumable)
            {
                newItemData = new Consumable(consumable.ItemID, consumable.ItemName,
                    consumable.PurchasePrice, consumable.SalePrice, consumable.HealAmount,
                    consumable.ManaRestore, consumable.Target, consumable.Effects);
            }
            else if (itemDataInfo is Totem totem)
            {
                newItemData = new Totem(totem.ItemID, totem.ItemName,
                    totem.PurchasePrice, totem.SalePrice, totem.AttackPoint, totem.DefensePoint, totem.Effects);
            }
            else if (itemDataInfo is Valuable valuable)
            {
                newItemData = new Valuable(valuable.ItemID, valuable.ItemName,
                    valuable.PurchasePrice, valuable.SalePrice, valuable.Effects);
            }

            if (newItemData == null)
            {
                Debug.LogError("아이템 타입을 확인할 수 없음!");
                return;
            }

            inventory[slot] = newItemData;
            Debug.Log($"{slot} 슬롯에 {newItemData.ItemName}이 추가됨");

            UpdateInventoryUI();
        }

        public void RemoveItem(int slot)
        {
            if (slot < 0 || slot >= inventory.Length || inventory[slot] == null)
            {
                Debug.Log("빈 슬롯 없음, 인덱스 오류");
                return;
            }

            Debug.Log($"{slot}슬롯에서 {inventory[slot].ItemName}이 제거됨");

            // 인벤토리에서 아이템 제거
            inventory[slot] = null;

            // UI 갱신
            UpdateInventoryUI();
        }

        public void PrintInventory()
        {
            for (int i = 0; i < inventory.Length; i++)
            {
                string itemName = inventory[i] != null ? inventory[i].ItemName : "빈 슬롯";
                Debug.Log($"슬롯 {i}: {itemName}");
            }
        }

        public void ToggleInventory()
        {
            inventoryContainer.SetActive(!inventoryContainer.activeSelf);
        }

        public void ToggleEquipment()
        { 
            equipContainer.SetActive(!equipContainer.activeSelf);
        }

        //  아이템 UI 자동 생성 및 제거
        // ReSharper disable Unity.PerformanceAnalysis
        private void UpdateInventoryUI()
        {
            for (int i = 0; i < inventory.Length; i++)
            {
                if (i >= itemUIList.Count)  // 기존 UI가 부족하면 새로운 슬롯 추가
                {
                    GameObject newItemUI = Instantiate(itemPrefab, inventoryContainerTransform);
                    itemUIList.Add(newItemUI);
                }

                Image itemImage = itemUIList[i].GetComponentInChildren<Image>();  // UI에서 이미지 가져오기

                if (inventory[i] != null)
                {
                    itemImage.sprite = inventory[i].ItemImg;  // 아이템이 있으면 이미지 적용
                    itemImage.enabled = true;
                }
                else
                {
                    itemImage.sprite = defaultSlotImage;  // 빈 슬롯이면 기본 이미지 유지
                    itemImage.enabled = true;
                }
            }
        }

        void Start()
        {
            for(int i = 0; i < inventory.Length;i++)
            {
                inventory[i] = null;
            }
            UpdateInventoryUI(); // 시작할 때 인벤토리 UI 초기화
        }
        

    }
}