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

    private Player player; // �÷��̾� ����

    private void Start()
    {
      
        player = Player.Instance;
        if(player == null )
        {
            Debug.LogError("�÷��̾ ã���� �����ϴ�. by.Equipinfo");
            return;
        }

        player.SetEquipUI(this);

        for (int i = 0; i < equipinfo.Length; i++)
        {
            int slotIndex = i; // ���� ǥ���Ŀ��� ������ ���� ����
            equipinfo[i].equipmentButtons.onClick.AddListener(() => OpenEquipDetailUI((EquipmentType)slotIndex));
        }

        UpdateEquipmentUI(); // ���â UI �ʱ� ������Ʈ
        Debug.Log("���â �ʱ�UI������Ʈ �Ϸ�");

    }

    // ���â UI ������Ʈ �Լ�
    public void UpdateEquipmentUI()
    {
        foreach (EquipmentType slot in System.Enum.GetValues(typeof(EquipmentType)))
        {
            Equipment equippedItem = player.GetEquippedItem(slot);
            UpdateSlot(slot, equippedItem);
        }
    }

    // Ư�� ������ ��� �����͸� UI�� �ݿ��ϴ� �Լ�
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
        Debug.Log($"OpenEquipDetailUI ȣ���: {slot}");

        if (equipDetailUI == null)
        {
            Debug.LogError("equipDetailUI�� �Ҵ���� �ʾҽ��ϴ�! Unity���� Ȯ���ϼ���.");
            return;
        }

        Equipment equippedItem = player.GetEquippedItem(slot);
        if (equippedItem == null)
        {
            Debug.Log("�ش� ���Կ� ������ ��� �����ϴ�.");
            return;
        }

        equipDetailUI.SetActive(true); //  ��� �� UI Ȱ��ȭ
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
            Debug.LogWarning($"���� {slot}�� ������ ��� �����ϴ�.");
            return;
        }
        player.RemoveEquip(equippedItem);
    }
}
