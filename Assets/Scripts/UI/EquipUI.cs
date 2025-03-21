using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class EquipInfo
{
    public Button equipmentButtons;
    public Image equipmentImages;
}

public class EquipUI : MonoBehaviour
{
    [Header("Equipment")]
    [SerializeField] EquipInfo[] equipinfo = new EquipInfo[6];

    [Header("EquipDetail")]
    [SerializeField] GameObject equipDetailUI;
    [SerializeField] Image equipDetailImage;
    [SerializeField] TMP_Text equipDetailText;
    [SerializeField] TMP_Text equipDetailDescription;
    [SerializeField] Button TrashButton;

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
            equipinfo[i].equipmentButtons.onClick.AddListener(() => OpenEquipDetailUI((EquipmentType)slotIndex));
        }

        UpdateEquipmentUI(); // 장비창 UI 초기 업데이트
        Debug.Log("장비창 초기UI업데이트 완료");

    }

    // 장비창 UI 업데이트 함수
    public void UpdateEquipmentUI()
    {
        foreach (EquipmentType slot in System.Enum.GetValues(typeof(EquipmentType)))
        {
            Equipment equippedItem = player.GetEquippedItem(slot);
            UpdateSlot(slot, equippedItem);
        }
    }

    // 특정 슬롯의 장비 데이터를 UI에 반영하는 함수
    public void UpdateSlot(EquipmentType slot, Equipment equippedItem)
    {
        int slotIndex = (int)slot;
        if (equippedItem != null)
        {
            equipinfo[slotIndex].equipmentImages.sprite = equippedItem.ItemImg;
        }
        else
        {
            equipinfo[slotIndex].equipmentImages.sprite = Resources.Load<Sprite>($"Images/Items/Equipment/default");
        }
    }

    public void OpenEquipDetailUI(EquipmentType slot)
    {
        Debug.Log($"OpenEquipDetailUI 호출됨: {slot}");

        if (equipDetailUI == null)
        {
            Debug.LogError("equipDetailUI가 할당되지 않았습니다! Unity에서 확인하세요.");
            return;
        }

        Equipment equippedItem = player.GetEquippedItem(slot);
        if (equippedItem == null)
        {
            Debug.Log("해당 슬롯에 장착된 장비가 없습니다.");
            return;
        }

        equipDetailUI.SetActive(true); //  장비 상세 UI 활성화
        equipDetailImage.sprite = equippedItem.ItemImg;
        equipDetailText.text = equippedItem.ItemName;
        equipDetailDescription.text = equippedItem.ItemDescription;

        TrashButton.onClick.RemoveAllListeners();

        TrashButton.onClick.AddListener(() =>
            {
                trashEquip(slot);
                CloseEquipDetail();
                UpdateEquipmentUI();
            });
    }

    public void CloseEquipDetail()
    {
        equipDetailUI.SetActive(false);
    }

    public void trashEquip(EquipmentType slot)
    {
        Equipment equippedItem = player.GetEquippedItem(slot);
        if (equippedItem == null)
        {
            Debug.LogWarning($"슬롯 {slot}에 장착된 장비가 없습니다.");
            return;
        }
        player.RemoveEquip(equippedItem);
    }
}
