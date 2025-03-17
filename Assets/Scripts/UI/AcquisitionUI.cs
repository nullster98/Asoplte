using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public enum AcquisitionType
{
    Item,   // ������
    Equipment,  // ���
    Trait,  // Ư��
    Skill   // ��ų
}

public class AcquisitionUI : MonoBehaviour
{
    [SerializeField]private ItemDatabase itemDatabase;

    [SerializeField] GameObject AcquistPannel;
    [SerializeField] Image GetImage;
    [SerializeField] TMP_Text GetName;
    [SerializeField] TMP_Text GetDescription;
    [SerializeField] Button confirmButton; // Ȯ�� ��ư (��: ����, ���, ����)
    [SerializeField] Button cancelButton; // ��� ��ư

    private AcquisitionType currentType; // ���� UI�� ǥ�õ� Ÿ��
    private object currentObject; // ���� UI�� ǥ�õ� ������ (Item, Trait, Skill ��)

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

        // ������ Ÿ�Ժ� UI ������Ʈ
        switch (type)
        {
            case AcquisitionType.Item:
                UpdateItemUI((Item)data);
                break;
            case AcquisitionType.Equipment:
                UpdateItemUI((Item)data);
                break;
            /*case AcquisitionType.Trait:
                UpdateTraitUI((Trait)data);
                break;
            case AcquisitionType.Skill:
                UpdateSkillUI((Skill)data);
                break;*/
        }
    }


    private void UpdateItemUI(Item item)
    {
        GetImage.sprite = item.ItemImg;
        GetName.text = item.ItemName;
        GetDescription.text = item.ItemDescription;

        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() =>
        {
            if (item is Equipment equipment)  //  `item`�� `Equipment` Ÿ������ Ȯ��
            {
                Player.Instance.EquipItem(equipment); // ĳ���� �� ��� ����
   
                
            }
            else if (item is Consumable consumable)  //  `item`�� `Consumable` Ÿ������ Ȯ��
            {
                //Player.Instance.UseItem(consumable); //  ĳ���� �� ������ ���
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

    public void CloseUI()
    {
        AcquistPannel.SetActive(false);
    }


    public void GetTestSword()
    {
            
            Item testItem = itemDatabase.GetItemByID(1001);
            UpdateItemUI(testItem);
        
    }
}
