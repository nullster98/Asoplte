using Item;
using PlayerScript;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [System.Serializable]
    public class EquipInfo
    {
        public Button equipmentButtons; // 슬롯 클릭용 버튼
        public Image equipmentImages; // 장비 아이콘 이미지
    }

    public class EquipUI : MonoBehaviour
    {
        [Header("Equipment")]
        [SerializeField] private EquipInfo[] equipinfo = new EquipInfo[6];

        [Header("EquipDetail")]
        [SerializeField] private GameObject equipDetailUI;
        [SerializeField] private Image equipDetailImage;
        [SerializeField] private TMP_Text equipDetailText;
        [SerializeField] private TMP_Text equipDetailDescription;
        [SerializeField] private Button trashButton;

        private Player player; // 플레이어 참조

        private void Start()
        {
      
            player = Player.Instance;
            if(player == null )
            {
                Debug.LogError("플레이어를 찾을수 없습니다. by.Equipinfo");
                return;
            }

            player.SetEquipUI(this);

            for (int i = 0; i < equipinfo.Length; i++)
            {
                int slotIndex = i; // 람다 표현식에서 안전한 슬롯 저장
                equipinfo[i].equipmentButtons.onClick.AddListener(() => 
                    OpenEquipDetailUI((EquipmentType)slotIndex));
            }

            UpdateEquipmentUI(); // 장비창 UI 초기 업데이트
            Debug.Log("장비창 초기UI업데이트 완료");

        }

        // 장비창 UI 업데이트 함수
        public void UpdateEquipmentUI()
        {
            foreach (EquipmentType slot in System.Enum.GetValues(typeof(EquipmentType)))
            {
                if(slot == EquipmentType.None) continue;
                
                ItemData equippedItem = player.GetEquippedItem(slot);
                UpdateSlot(slot, equippedItem);
            }
        }

        // 특정 슬롯의 장비 데이터를 UI에 반영하는 함수
        private void UpdateSlot(EquipmentType slot, ItemData equippedItem)
        {
            int slotIndex = (int)slot;

            if (slotIndex < 0 || slotIndex >= equipinfo.Length)
            {
                Debug.LogWarning($"[EquipUI] 잘못된 슬롯 인덱스 접근: {slot}({slotIndex})");
                return;
            }

            equipinfo[slotIndex].equipmentImages.sprite = equippedItem != null
                ? equippedItem.itemImage
                : Resources.Load<Sprite>($"Images/Items/Equipment/default");
        }

        //슬롯 클릭 시 장비 상세 UI 열기
        private void OpenEquipDetailUI(EquipmentType slot)
        {
            Debug.Log($"OpenEquipDetailUI 호출됨: {slot}");
            
            if(slot == EquipmentType.None) return;

            if (equipDetailUI == null)
            {
                Debug.LogError("equipDetailUI가 할당되지 않았습니다! Unity에서 확인하세요.");
                return;
            }

            ItemData equippedItem = player.GetEquippedItem(slot);
            if (equippedItem == null)
            {
                Debug.Log("해당 슬롯에 장착된 장비가 없습니다.");
                return;
            }

            equipDetailUI.SetActive(true); //  장비 상세 UI 활성화
            equipDetailImage.sprite = equippedItem.itemImage;
            equipDetailText.text = ($"{equippedItem.itemName} [{equippedItem.rarity}] \n {equippedItem.equipSlot}");
            equipDetailDescription.text = equippedItem.summary;

            trashButton.onClick.RemoveAllListeners();

            trashButton.onClick.AddListener(() =>
            {
                TrashEquip(slot);
                CloseEquipDetail();
                UpdateEquipmentUI();
            });
        }

        // 장비 상세 UI 닫기
        public void CloseEquipDetail()
        {
            equipDetailUI.SetActive(false);
        }

        // 장비 버리기(해제 처리)
        private void TrashEquip(EquipmentType slot)
        {
            if(slot == EquipmentType.None) return;
            
            ItemData equippedItem = player.GetEquippedItem(slot);
            if (equippedItem == null)
            {
                Debug.LogWarning($"슬롯 {slot}에 장착된 장비가 없습니다.");
                return;
            }
            player.RemoveEquip(equippedItem);
        }
    }
}