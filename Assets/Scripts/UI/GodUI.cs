using Game;
using God;
using PlayerScript;
using TMPro;
using Unity;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GodUI : MonoBehaviour
    {
        private int currentIndex;
        public Image mainImage; // 가운데 메인 이미지
        public Image leftImage; // 왼쪽 이미지
        public Image rightImage; // 오른쪽 이미지
        public TMP_Text nameText; // 이름 텍스트
        public TMP_Text infoText; // 정보 텍스트
        public Player player;

        [SerializeField] private GameObject godSelect;
        [SerializeField] private Image godBackground;
        [SerializeField] private GameObject characterSelect;
        [SerializeField] private GameObject selectMenu;
        [SerializeField] private GameObject gameMenu;

        private void Start()
        {
            if (DatabaseManager.Instance.godList == null || DatabaseManager.Instance.godList.Count == 0)
            {
                Debug.LogWarning("GodDatabase가 비어 있습니다! 인스펙터에서 설정하세요.");
                return;
            }
            
        }

        public void OnNextButtonPressed()
        {
            if (DatabaseManager.Instance.godList.Count == 0) return;
            currentIndex = (currentIndex + 1) % DatabaseManager.Instance.godList.Count;
            UpdateUI();
        }

        public void OnPreviousButtonPressed()
        {
            if (DatabaseManager.Instance.godList.Count == 0) return;
            currentIndex = (currentIndex - 1 + DatabaseManager.Instance.godList.Count) % DatabaseManager.Instance.godList.Count;
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (DatabaseManager.Instance.godList.Count == 0)
            {
                Debug.LogWarning("갓 리스트가 비어 있음!");
                return;
            }

            GodData currentImage = DatabaseManager.Instance.GetGodByIndex(currentIndex);
            if (currentImage == null)
            {
                Debug.LogError($"currentImage is null! Index: {currentIndex}");
                return;
            }

            Debug.Log($"[GodUI] 현재 신: {currentImage.GodName}, ID: {currentImage.GodID}");
            
            mainImage.sprite = currentImage.GodImage;
            nameText.text = currentImage.GodName;
            infoText.text = GenerateEffectText(currentImage);
            godBackground.sprite = currentImage.GodBackgroundImage;
            
            Debug.Log(infoText.text);

            // 왼쪽과 오른쪽 이미지 갱신
            int leftIndex = (currentIndex - 1 + DatabaseManager.Instance.godList.Count) % DatabaseManager.Instance.godList.Count;
            int rightIndex = (currentIndex + 1) % DatabaseManager.Instance.godList.Count;

            leftImage.sprite = DatabaseManager.Instance.GetGodByIndex(leftIndex).GodImage;
            rightImage.sprite = DatabaseManager.Instance.GetGodByIndex(rightIndex).GodImage;

            // 왼쪽과 오른쪽 이미지를 흑백으로 변환
            leftImage.color = Color.gray;
            rightImage.color = Color.gray;
            
            var current = DatabaseManager.Instance.GetGodByIndex(currentIndex);

            Debug.Log($"[GodUI] 현재 godName: {current.GodName}, fileName: {current.FileName}");
            Debug.Log($"[GodUI] GetDescription(): {current.GodDescription}");
        }
        
        public void InitializeUI()
        {
            if (DatabaseManager.Instance.godList == null)
            {
                Debug.LogError("[GodUI] godDatabase가 null입니다!");
                return;
            }

            if (DatabaseManager.Instance.godList == null || DatabaseManager.Instance.godList.Count == 0)
            {
                Debug.LogError("[GodUI] godDatabase.godList가 비어있습니다!");
                return;
            }

            Debug.Log($"[GodUI] godList 로드 성공! 총 {DatabaseManager.Instance.godList.Count}개");
    
            currentIndex = 0;
            UpdateUI();
        }

        private string GenerateEffectText(GodData god)
        {
            var sb = new System.Text.StringBuilder();

            // 🟢 1. 이름
            sb.AppendLine($"<size=100>{god.GodName}</size>");
            sb.AppendLine();

            // 🟢 2. 설명
            sb.AppendLine($"{god.GodDescription}");
            sb.AppendLine();

            // 🟢 3. 고유 효과 먼저 출력
            bool hasEffect = false;
            foreach (var effect in god.SpecialEffect)
            {
                if (effect is not StatModifierEffect)
                {
                    sb.AppendLine("- [고유 효과] " + effect.ToString());
                    hasEffect = true;
                }
            }

            // 🟢 4. 스탯 효과 출력
            foreach (var effect in god.SpecialEffect)
            {
                Debug.Log($"[디버그] 효과 타입: {effect.GetType().Name}");
                if (effect is StatModifierEffect stat)
                {
                    Debug.Log($"[디버그] 스탯 적용 확인됨: {stat.StatName} {stat.Amount}");
                    string sign = stat.Amount >= 0 ? "+" : "";
                    sb.AppendLine($"- {stat.StatName} {sign}{stat.Amount}");
                    hasEffect = true;
                }
            }

            // 🟢 5. 아무 효과도 없을 경우
            if (!hasEffect)
            {
                sb.AppendLine("- 효과 없음");
            }

            return sb.ToString();
        }

        public void GodSelectToCharacterSelect()//신앙선택에서 종족선택으로 넘어가는 버튼
        {
            godSelect.SetActive(false);
            characterSelect.SetActive(true);
            player.SelectedGod(DatabaseManager.Instance.GetGodByIndex(currentIndex));
        }

        public void PreviousButton()
        {
            gameMenu.SetActive(false);
            selectMenu.SetActive(true);
        }
    }
}