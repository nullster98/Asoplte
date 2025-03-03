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

        CreateItem(1001, "�Ѽհ�", "���� �����ϸ� �� ���ظ� �ݴϴ�.", 1500, 800, Equipment.LeftHand, "�Ѽհ�",
            new List<ItemEffect>
            {
                new EquipEffect(new List<EquipEffect.StatModifier> { new EquipEffect.StatModifier { StatName = "Atk", Value = 5 } }),
                new PoisonEffect(3, 2)  // �� �� 3 ������, 2�� ����
            });

        CreateItem(1002, "���� ��", "�� �Ӽ� ���ظ� �ݴϴ�.", 2000, 1200, Equipment.LeftHand, "FireSword",
            new List<ItemEffect>
            {
                new EquipEffect(new List<EquipEffect.StatModifier> { new EquipEffect.StatModifier { StatName = "Atk", Value = 7 } }),
                new PoisonEffect(4, 3)  // �� �� 4 ������, 3�� ����
            });

        Debug.Log("��� �������� �����Ǿ����ϴ�.");
    }

    private static void CreateItem(int id, string name, string description, int purchasePrice, int salePrice,
     Equipment slot, string imageName, List<ItemEffect> effects)
    {
        Debug.Log($"������ ����: {name}");

        // ������ �̹��� �ε� (������ �⺻ �̹��� ���)
        Sprite itemSprite = Resources.Load<Sprite>($"Images/Items/Weapon/{imageName}");
        if (itemSprite == null)
        {
            Debug.LogWarning($"�̹��� {imageName}��(��) ã�� �� �����ϴ�! �⺻ �̹����� ����.");
            itemSprite = Resources.Load<Sprite>("Images/Items/Weapon/default");  // �⺻ �̹��� ����

            if (itemSprite == null)
            {
                Debug.LogError("�⺻ �̹����� ã�� �� �����ϴ�!");
            }
        }

        // Item����
        Item newItem = new Item(id, name, description, slot, purchasePrice, salePrice,
           itemSprite, effects);

        // ItemDatabase�� �߰�
        ItemDatabase database = Resources.Load<ItemDatabase>("Database/ItemDatabase");
        if (database != null)
        {
            database.ItemList.Add(newItem);  // 'ItemInfo'�� �߰� (���� �ذ�)
            EditorUtility.SetDirty(database);
            AssetDatabase.SaveAssets();
            Debug.Log($"{name}��(��) �����ͺ��̽��� �߰��Ǿ����ϴ�!");
        }
        else
        {
            Debug.LogError("ItemDatabase�� ã�� �� �����ϴ�! Resources ���� �� ��θ� Ȯ���ϼ���.");
        }
    }
}

