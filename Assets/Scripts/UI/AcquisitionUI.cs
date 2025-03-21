using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public enum AcquisitionType
{
    Item,   // 아이템
    Equipment,  // 장비
    Trait,  // 특성
    Skill   // 스킬
}

public class AcquisitionUI : MonoBehaviour
{
    [SerializeField] GameObject AcquistPannel;
    [SerializeField] Image GetImage;
    [SerializeField] TMP_Text GetName;
    [SerializeField] TMP_Text GetDescription;
    [SerializeField] Button confirmButton; // 확인 버튼 (예: 장착, 사용, 습득)
    [SerializeField] Button cancelButton; // 취소 버튼

    private AcquisitionType currentType; // 현재 UI에 표시된 타입
    private object currentObject; // 현재 UI에 표시된 데이터 (Item, Trait, Skill 등)

    private void Start()
    {

    }



    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            GetTestSword();
        }

        if(Input.GetKeyDown(KeyCode.S))
        {
            AcquistPannel.SetActive(true);
        }
    }

    public void Setup(AcquisitionType type, object data)
    {
        currentType = type;
        currentObject = data;

        // 데이터 타입별 UI 업데이트
        switch (type)
        {
            case AcquisitionType.Item:
                UpdateItemUI((ItemData)data);
                break;
            case AcquisitionType.Equipment:
                UpdateItemUI((ItemData)data);
                break;
            /*case AcquisitionType.Trait:
                UpdateTraitUI((Trait)data);
                break;
            case AcquisitionType.Skill:
                UpdateSkillUI((Skill)data);
                break;*/
        }
    }


    private void UpdateItemUI(ItemData itemData)
    {
        GetImage.sprite = itemData.ItemImg;
        GetName.text = itemData.ItemName;
        GetDescription.text = itemData.ItemDescription;

        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() =>
        {
            if (itemData is Equipment equipment)  //  `item`이 `Equipment` 타입인지 확인
            {
                Player.Instance.EquipItem(equipment); // 캐스팅 후 장비 장착
   
                
            }
            else if (itemData is Consumable consumable)  //  `item`이 `Consumable` 타입인지 확인
            {
                //Player.Instance.UseItem(consumable); //  캐스팅 후 아이템 사용
            }

            CloseUI();
        });
    }

    /*private void UpdateTraitUI(Trait trait)
    {
        GetImage.sprite = trait.TraitImg;
        GetName.text = trait.TraitName;
        GetDescription.text = trait.TraitDescription;

        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() =>
        {
            Player.Instance.ApplyTrait(trait);
            CloseUI();
        });
    }*/

    /*private void UpdateSkillUI(Skill skill)
    {
        GetImage.sprite = skill.SkillIcon;
        GetName.text = skill.Name;
        GetDescription.text = skill.Description;

        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() =>
        {
            Player.Instance.LearnSkill(skill);
            CloseUI();
        });
    }*/

    public void OpenAcquisitionUI(AcquisitionType type, int id)
    {
        AcquistPannel.SetActive(true);
    }

    public void CloseUI()
    {
        AcquistPannel.SetActive(false);
    }


    public void GetTestSword()
    {
            
            ItemData testItemData = DatabaseManager.Instance.itemDatabase.GetItemByID(1001);
            UpdateItemUI(testItemData);
        
    }
}
