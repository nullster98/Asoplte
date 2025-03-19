using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public Item GiveItemToPlayer(int itemID)
    {
        Item item = DatabaseManager.Instance.itemDatabase.GetItemByID(itemID);
        if (item == null)
        {
            Debug.LogError("�ش� ID�� �������� ã�� �� �����ϴ�!");
            return null;
        }
        return item;
    }
}
