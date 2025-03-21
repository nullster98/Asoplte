using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Game/Item Database")]
public class ItemDatabase : ScriptableObject
{
    public List<ItemData> ItemList = new List<ItemData>();

    public ItemData GetItemByID(int itemID)
    {
        return ItemList.Find(item => item.ItemID == itemID);
    }

    public void ResetDatabase()
    {
        ItemList.Clear();
    }
}



