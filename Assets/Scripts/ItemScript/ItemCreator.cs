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
        CreateEquipment(1001, "�Ѽհ�",  1500, 800, EquipmentType.LeftHand,
            new List<ItemEffect>(),10,0);           

        // ��� ������ (���� ��)
        CreateEquipment(1002, "���� ��", 2000, 1200, EquipmentType.LeftHand,
            new List<ItemEffect>
            {
                new PoisonEffect(4, 3) // �� �� 4 ������, 3�� ����
            }, 7, 0);

        // �Ҹ�ǰ ������ (HP ����)
        CreateConsumable(2001, "HP ����",  500, 250,
            new List<ItemEffect>(), 50, 0, ItemTraget.Player);

        // �Ҹ�ǰ ������ (�� ���� - ����)
        CreateConsumable(2002, "�� ����", 800, 400,
            new List<ItemEffect>
            {
                new PoisonEffect(5, 3)
            }, 0, 0, ItemTraget.Enemy);

        Debug.Log("��� �������� �����Ǿ����ϴ�.");
    }

    // ��� ������ ���� �Լ�
    private static void CreateEquipment(int id, string name, int purchasePrice, int salePrice,
        EquipmentType slot, List<ItemEffect> effects, float attack, float defense)
    {
        Debug.Log($"��� ������ ����: {name}");

        Equipment newItem = new Equipment(id, name, purchasePrice, salePrice, slot, attack, defense, effects);
        AddItemToDatabase(newItem);
    }

    // �Ҹ�ǰ ������ ���� �Լ�
    private static void CreateConsumable(int id, string name, int purchasePrice, int salePrice,
         List<ItemEffect> effects, float healAmount, float manaRestore, ItemTraget target)
    {
        Debug.Log($"�Ҹ�ǰ ������ ����: {name}");


        Consumable newItem = new Consumable(id, name, purchasePrice, salePrice, healAmount, manaRestore, target, effects);
        AddItemToDatabase(newItem);
    }


    // �������� �����ͺ��̽��� �߰��ϴ� �Լ�
    private static void AddItemToDatabase(Item item)
    {
        if (DatabaseManager.Instance.itemDatabase != null)
        {
            DatabaseManager.Instance.itemDatabase.ItemList.Add(item);
            EditorUtility.SetDirty(DatabaseManager.Instance.itemDatabase);
            AssetDatabase.SaveAssets();
            Debug.Log($"{item.ItemName}��(��) �����ͺ��̽��� �߰��Ǿ����ϴ�!");
        }
        else
        {
            Debug.LogError("ItemDatabase�� ã�� �� �����ϴ�! Resources ���� �� ��θ� Ȯ���ϼ���.");
        }
    }
}

