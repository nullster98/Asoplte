using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;

    [SerializeField] private ItemDatabase itemDatabase;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public Item GiveItemToPlayer(int itemID)
    {
        Item item = itemDatabase.GetItemByID(itemID);
        if (item == null)
        {
            Debug.LogError("해당 ID의 아이템을 찾을 수 없습니다!");
            return null;
        }
        return item;
    }
}
