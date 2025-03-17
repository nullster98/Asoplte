using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    private Item[] inventory = new Item[4]; // 4칸 고정 인벤토리
    [SerializeField] private GameObject InventoryContainer; // 인벤토리 UI 패널
    [SerializeField] private GameObject EquipContainer; // 인벤토리 UI 패널
    [SerializeField] private Transform InventoryContainerTransform; // 아이템 UI 위치
    [SerializeField] private GameObject ItemPrefab; // 아이템 프리팹
    [SerializeField] private ItemDatabase itemDatabase;
    [SerializeField] private Sprite defaultSlotImage;

    private List<GameObject> itemUIList = new List<GameObject>(); // UI 아이템 리스트

    public void AddItem(int slot, int itemID)
    {
        if (slot < 0 || slot >= inventory.Length)
        {
            Debug.Log("잘못된 슬롯");
            return;
        }

        if (inventory[slot] != null)
        {
            Debug.Log("슬롯 꽉참");
            return;
        }

        Item itemInfo = itemDatabase.GetItemByID(itemID);
        if (itemInfo == null)
        {
            Debug.Log("아이템 정보 없음");
            return;
        }

        if (itemInfo is Equipment)
        {
            Debug.Log("장비 아이템은 인벤토리에 추가할 수 없습니다.");
            return;
        }

        Item newItem = null;

        // 소모품 (`Consumable`) 처리
        if (itemInfo is Consumable consumable)
        {
            newItem = new Consumable(consumable.ItemID, consumable.ItemName, consumable.ItemDescription,
                consumable.PurchasePrice, consumable.SalePrice, consumable.ItemImg, consumable.HealAmount,
                consumable.ManaRestore, consumable.Target, consumable.Effects);
        }
        // 토템 (`Totem`) 처리
        else if (itemInfo is Totem totem)
        {
            newItem = new Totem(totem.ItemID, totem.ItemName, totem.ItemDescription,
                totem.PurchasePrice, totem.SalePrice, totem.ItemImg, totem.AttackPoint, totem.DefensePoint, totem.Effects);
        }
        // 귀중품 (`Valuable`) 처리
        else if (itemInfo is Valuable valuable)
        {
            newItem = new Valuable(valuable.ItemID, valuable.ItemName, valuable.ItemDescription,
                valuable.PurchasePrice, valuable.SalePrice, valuable.ItemImg, valuable.Effects);
        }

        if (newItem == null)
        {
            Debug.LogError("아이템 타입을 확인할 수 없음!");
            return;
        }

        //  인벤토리에 아이템 추가
        inventory[slot] = newItem;
        Debug.Log($"{slot} 슬롯에 {newItem.ItemName}이 추가됨");

        // UI 갱신
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
        InventoryContainer.SetActive(!InventoryContainer.activeSelf);
    }

    public void ToggleEquipment()
    { 
        EquipContainer.SetActive(!EquipContainer.activeSelf);
    }

    //  아이템 UI 자동 생성 및 제거
    private void UpdateInventoryUI()
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (i >= itemUIList.Count)  // 기존 UI가 부족하면 새로운 슬롯 추가
            {
                GameObject newItemUI = Instantiate(ItemPrefab, InventoryContainerTransform);
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

    public void GetItem()
    {
        if(Input.GetKeyDown(KeyCode.W))
        {
            AddItem(1,1001);
        }
    }

    void Start()
    {
        for(int i = 0; i < inventory.Length;i++)
        {
            inventory[i] = null;
        }
        UpdateInventoryUI(); // 시작할 때 인벤토리 UI 초기화
        ResetItemDatabase();
        InitializeItemDatabse();
    }

    private void Update()
    {
        GetItem();
    }

    private void InitializeItemDatabse()
    {
        ItemDatabase database = Resources.Load<ItemDatabase>("Database/ItemDatabase");
        if (database == null)
        {
            Debug.LogError("ItemDatabase를 찾을 수 없습니다! 폴더확인");
            return;
        }

        database.ItemList.RemoveAll(item => item == null);

        Debug.Log($"현재 데이터베이스에 등록된 아이템 갯수 : {database.ItemList.Count}");

        if (database.ItemList.Count == 0)
        {
            Debug.Log("아이템 데이터베이스가 비어 있음. 모든 아이템을 생성합니다.");
            ItemCreator.CreateAllItems();
        }
        else
        {
            Debug.Log("아이템 데이터베이스가 이미 초기화되어 있습니다.");
        }

        EditorUtility.SetDirty(database);
    }

    private void ResetItemDatabase()
    {
        if (itemDatabase != null)
        {
            itemDatabase.ResetDatabase();
            Debug.Log("게임 시작 시 ItemDatabase 초기화 완료!");
        }
        else
        {
            Debug.LogError("ItemDatabase를 찾을 수 없습니다!");
        }
    }
}