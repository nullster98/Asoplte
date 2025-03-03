using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    private Item[] inventory = new Item[4]; // 4ĭ ���� �κ��丮
    [SerializeField] private GameObject InventoryContainer; // �κ��丮 UI �г�
    [SerializeField] private GameObject EquipContainer; // �κ��丮 UI �г�
    [SerializeField] private Transform InventoryContainerTransform; // ������ UI ��ġ
    [SerializeField] private GameObject ItemPrefab; // ������ ������
    [SerializeField] private ItemDatabase itemDatabase;
    [SerializeField] private Sprite defaultSlotImage;

    private List<GameObject> itemUIList = new List<GameObject>(); // UI ������ ����Ʈ

    public void AddItem(int slot, int itemID)
    {
        if (slot < 0 || slot >= inventory.Length)
        {
            Debug.Log("�߸��� ����");
            return;
        }

        if (inventory[slot] != null)
        {
            Debug.Log("���� ����");
            return;
        }

        Item itemInfo = itemDatabase.GetItemByID(itemID);
        if (itemInfo == null)
        {
            Debug.Log("������ ���� ����");
            return;
        }

        Item newItem = new Item(itemInfo.ItemID, itemInfo.ItemName, itemInfo.ItemDescription, itemInfo.Slot,
                            itemInfo.PurchasePrice, itemInfo.SalePrice, itemInfo.ItemImg, itemInfo.Effects);

        // �κ��丮�� ������ �߰�
        inventory[slot] = newItem;
        Debug.Log($"{slot}���Կ� {itemInfo.ItemName}�� �߰���");

        // UI ����
        UpdateInventoryUI();
    }

    public void RemoveItem(int slot)
    {
        if (slot < 0 || slot >= inventory.Length || inventory[slot] == null)
        {
            Debug.Log("�� ���� ����, �ε��� ����");
            return;
        }

        Debug.Log($"{slot}���Կ��� {inventory[slot].ItemName}�� ���ŵ�");

        // �κ��丮���� ������ ����
        inventory[slot] = null;

        // UI ����
        UpdateInventoryUI();
    }

    public void PrintInventory()
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            string itemName = inventory[i] != null ? inventory[i].ItemName : "�� ����";
            Debug.Log($"���� {i}: {itemName}");
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

    //  ������ UI �ڵ� ���� �� ����
    private void UpdateInventoryUI()
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (i >= itemUIList.Count)  // ���� UI�� �����ϸ� ���ο� ���� �߰�
            {
                GameObject newItemUI = Instantiate(ItemPrefab, InventoryContainerTransform);
                itemUIList.Add(newItemUI);
            }

            Image itemImage = itemUIList[i].GetComponentInChildren<Image>();  // UI���� �̹��� ��������

            if (inventory[i] != null)
            {
                itemImage.sprite = inventory[i].ItemImg;  // �������� ������ �̹��� ����
                itemImage.enabled = true;
            }
            else
            {
                itemImage.sprite = defaultSlotImage;  // �� �����̸� �⺻ �̹��� ����
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
        UpdateInventoryUI(); // ������ �� �κ��丮 UI �ʱ�ȭ
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
            Debug.LogError("ItemDatabase�� ã�� �� �����ϴ�! ����Ȯ��");
            return;
        }

        database.ItemList.RemoveAll(item => item == null);

        Debug.Log($"���� �����ͺ��̽��� ��ϵ� ������ ���� : {database.ItemList.Count}");

        if (database.ItemList.Count == 0)
        {
            Debug.Log("������ �����ͺ��̽��� ��� ����. ��� �������� �����մϴ�.");
            ItemCreator.CreateAllItems();
        }
        else
        {
            Debug.Log("������ �����ͺ��̽��� �̹� �ʱ�ȭ�Ǿ� �ֽ��ϴ�.");
        }

        EditorUtility.SetDirty(database);
    }

    private void ResetItemDatabase()
    {
        if (itemDatabase != null)
        {
            itemDatabase.ResetDatabase();
            Debug.Log("���� ���� �� ItemDatabase �ʱ�ȭ �Ϸ�!");
        }
        else
        {
            Debug.LogError("ItemDatabase�� ã�� �� �����ϴ�!");
        }
    }
}