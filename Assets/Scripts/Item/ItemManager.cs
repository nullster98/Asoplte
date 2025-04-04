using Game;
using UnityEngine;

namespace Item
{
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

        public ItemData GiveItemToPlayer(string itemID)
        {
            ItemData itemData = DatabaseManager.Instance.GetItemData(itemID);
            if (itemData == null)
            {
                Debug.LogError("해당 ID의 아이템을 찾을 수 없습니다!");
                return null;
            }
            return itemData;
        }
    }
}
