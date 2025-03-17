using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class ItemCreator : MonoBehaviour
{
    [MenuItem("Game/Create All Items")]
    public static void CreateAllItems()
    {
        Debug.Log("��� ������ ���� ����...");

        // ��� ������ (�Ѽհ�)
        CreateEquipment(1001, "�Ѽհ�", "���� �����ϸ� �� ���ظ� �ݴϴ�.", 1500, 800, EquipmentType.LeftHand, "�Ѽհ�",
            new List<ItemEffect>(),10,0);           

        // ��� ������ (���� ��)
        CreateEquipment(1002, "���� ��", "�� �Ӽ� ���ظ� �ݴϴ�.", 2000, 1200, EquipmentType.LeftHand, "FireSword",
            new List<ItemEffect>
            {
                new PoisonEffect(4, 3) // �� �� 4 ������, 3�� ����
            }, 7, 0);

        // �Ҹ�ǰ ������ (HP ����)
        CreateConsumable(2001, "HP ����", "ü���� 50 ȸ���մϴ�.", 500, 250, "HP_Potion",
            new List<ItemEffect>(), 50, 0, ItemTraget.Player);

        // �Ҹ�ǰ ������ (�� ���� - ����)
        CreateConsumable(2002, "�� ����", "������ ���� �ο��մϴ�.", 800, 400, "Poison_Potion",
            new List<ItemEffect>
            {
                new PoisonEffect(5, 3)
            }, 0, 0, ItemTraget.Enemy);

        Debug.Log("��� �������� �����Ǿ����ϴ�.");
    }

    // ��� ������ ���� �Լ�
    private static void CreateEquipment(int id, string name, string description, int purchasePrice, int salePrice,
        EquipmentType slot, string imageName, List<ItemEffect> effects, float attack, float defense)
    {
        Debug.Log($"��� ������ ����: {name}");

        Sprite itemSprite = LoadItemSprite(imageName, ItemType.Equipment);

        Equipment newItem = new Equipment(id, name, description, purchasePrice, salePrice, itemSprite, slot, attack, defense, effects);
        AddItemToDatabase(newItem);
    }

    // �Ҹ�ǰ ������ ���� �Լ�
    private static void CreateConsumable(int id, string name, string description, int purchasePrice, int salePrice,
        string imageName, List<ItemEffect> effects, float healAmount, float manaRestore, ItemTraget target)
    {
        Debug.Log($"�Ҹ�ǰ ������ ����: {name}");

        Sprite itemSprite = LoadItemSprite(imageName, ItemType.Consumable);

        Consumable newItem = new Consumable(id, name, description, purchasePrice, salePrice, itemSprite, healAmount, manaRestore, target, effects);
        AddItemToDatabase(newItem);
    }

    // ���� ������ �̹��� �ε� �Լ�
    private static Sprite LoadItemSprite(string imageName, ItemType type)
    {
        string folderPath = type switch
        { ItemType.Equipment => "Images/Items/Equipment",
          ItemType.Consumable => "Images/Items/Consumables",
          ItemType.Totem => "Images/Items/Totems",
          ItemType.Valuable => "Images/Items/Valuables",
            _ => "Images/Default" // �⺻ ����



        };

        Sprite itemSprite = Resources.Load<Sprite>($"{folderPath}/{imageName}");
        if (itemSprite == null)
        {
            Debug.LogWarning($"�̹��� {imageName}��(��) ã�� �� �����ϴ�! �⺻ �̹����� ����.");
            itemSprite = Resources.Load<Sprite>("Images/default");
        }
        return itemSprite;
    }

    // �������� �����ͺ��̽��� �߰��ϴ� �Լ�
    private static void AddItemToDatabase(Item item)
    {
        ItemDatabase database = Resources.Load<ItemDatabase>("Database/ItemDatabase");
        if (database != null)
        {
            database.ItemList.Add(item);
            EditorUtility.SetDirty(database);
            AssetDatabase.SaveAssets();
            Debug.Log($"{item.ItemName}��(��) �����ͺ��̽��� �߰��Ǿ����ϴ�!");
        }
        else
        {
            Debug.LogError("ItemDatabase�� ã�� �� �����ϴ�! Resources ���� �� ��θ� Ȯ���ϼ���.");
        }
    }
}

