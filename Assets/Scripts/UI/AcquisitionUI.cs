using Event;
using Game;
using Item;
using PlayerScript;
using TMPro;
using Trait;
using Unity;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public enum AcquisitionType
    {
        Item, // 아이템
        Equipment, // 장비
        Trait, // 특성
        Skill // 스킬
    }

    public class AcquisitionUI : MonoBehaviour
    {
        [SerializeField] private GameObject acquistPannel;
        [SerializeField] private Image getImage;
        [SerializeField] private TMP_Text getName;
        [SerializeField] private TMP_Text getDescription;
        [SerializeField] private Button confirmButton; // 확인 버튼 (예: 장착, 사용, 습득)
        [SerializeField] private Button cancelButton; // 취소 버튼

        private AcquisitionType currentType; // 현재 UI에 표시된 타입
        private object currentObject; // 현재 UI에 표시된 데이터 (Item, Trait, Skill 등)


        public void SetupAcquisitionUI(AcquisitionType type, string id)
        {
            EventManager.Instance.NotifyUIOpened();
            confirmButton.onClick.RemoveAllListeners();

            object data = null;

            switch (type)
            {
                case AcquisitionType.Item:
                case AcquisitionType.Equipment:
                    data = DatabaseManager.Instance.GetItemData(id);
                    break;

                case AcquisitionType.Trait:
                    data = DatabaseManager.Instance.GetTraitData(id);
                    break;

                /*case AcquisitionType.Skill:
                    data = DatabaseManager.Instance.skillDatabase.GetSkillByID(id);
                    break;*/

                // 필요 시 더 추가
            }

            if (data == null)
            {
                Debug.LogError($"[AcquisitionUI] 데이터 로드 실패: {type}, ID: {id}");
                return;
            }

            currentType = type;
            currentObject = data;

            acquistPannel.SetActive(true);

            // UI 업데이트
            switch (type)
            {
                case AcquisitionType.Item:
                case AcquisitionType.Equipment:
                    UpdateItemUI((ItemData)data);
                    break;
                case AcquisitionType.Trait:
                    UpdateTraitUI((TraitData)data);
                    break;
                /*case AcquisitionType.Skill:
                    UpdateSkillUI((SkillData)data);
                    break;*/
            }
        }


        private void UpdateItemUI(ItemData itemData)
        {
            getImage.sprite = itemData.itemImage;
            getName.text = itemData.itemName;
            var sb = new System.Text.StringBuilder();
            //1. 이름
            sb.AppendLine($"{itemData.itemName} [{itemData.rarity}]");
            sb.AppendLine($"{itemData.equipSlot}");
            sb.AppendLine();
            //2. 요약설명
            sb.AppendLine($"{itemData.summary}");
            //3. 고유효과
            bool hasEffect = false;
            foreach (var effect in itemData.effects)
            {
                if (effect is not StatModifierEffect)
                {
                    sb.AppendLine("- [고유 효과] " + effect.ToString());
                    hasEffect = true;
                }
            }
            //4.스텟효과
            foreach (var effect in itemData.effects)
            {
                if (effect is StatModifierEffect stat)
                {
                    string sign = stat.Amount >= 0 ? "+" : "";
                    sb.AppendLine($"- {stat.StatName} {sign}{stat.Amount}");
                    hasEffect = true;
                }
            }

            if (!hasEffect)
            {
                sb.AppendLine("효과 없음");
            }
            getDescription.text = sb.ToString();

            confirmButton.onClick.AddListener(ApplyReward);
        }

        private void UpdateTraitUI(TraitData trait)
        {
            getImage.sprite = trait.traitImage;
            getName.text = trait.traitName;
            getDescription.text = trait.summary;

            confirmButton.onClick.AddListener(ApplyReward);
        }

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

        private void ApplyReward()
        {
            switch (currentType)
            {
                case AcquisitionType.Item:
                    var item = currentObject as ItemData;
                    InventoryManager.Instance.AddItem(item.itemID);
                    break;
                case AcquisitionType.Equipment:
                    Player.Instance.EquipItem((ItemData)currentObject);
                    break;
                case AcquisitionType.Trait:
                    Player.Instance.ApplyTraitEffect((TraitData)currentObject);
                    break;
            }

            CloseAcquisitionUI();
        }


        public void CloseAcquisitionUI()
        {
            acquistPannel.SetActive(false);
            EventManager.Instance.NotifyUIClosed();
            EventManager.Instance.RequestHandleEventEnd();
        }
    }
}