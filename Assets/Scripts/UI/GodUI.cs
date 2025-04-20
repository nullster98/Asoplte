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
        
        private GodData GetCurrentGod() // 현재 인덱스의 GodData 반환
        {
            if (DatabaseManager.Instance.godList == null || DatabaseManager.Instance.godList.Count == 0)
                return null;

            string godID = DatabaseManager.Instance.godList[currentIndex].GodID;
            return DatabaseManager.Instance.GetGodData(godID);
        }

        public void OnNextButtonPressed() // ▶ 버튼 (다음 "신"으로 이동)
        {
            if (DatabaseManager.Instance.godList.Count == 0) return;
            currentIndex = (currentIndex + 1) % DatabaseManager.Instance.godList.Count;
            UpdateUI();
        }

        public void OnPreviousButtonPressed() // ◀ 버튼 (이전 "신"으로 이동)
        {
            if (DatabaseManager.Instance.godList.Count == 0) return;
            currentIndex = (currentIndex - 1 + DatabaseManager.Instance.godList.Count) % DatabaseManager.Instance.godList.Count;
            UpdateUI();
        }

        private void UpdateUI() // UI 전체 갱신
        {
            if (DatabaseManager.Instance.godList.Count == 0)
            {
                Debug.LogWarning("갓 리스트가 비어 있음!");
                return;
            }

            GodData currentImage = GetCurrentGod();
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

            string leftID = DatabaseManager.Instance.godList[leftIndex].GodID;
            string rightID = DatabaseManager.Instance.godList[rightIndex].GodID;

            leftImage.sprite = DatabaseManager.Instance.GetGodData(leftID).GodImage;
            rightImage.sprite = DatabaseManager.Instance.GetGodData(rightID).GodImage;

            // 왼쪽과 오른쪽 이미지를 흑백으로 변환
            leftImage.color = Color.gray;
            rightImage.color = Color.gray;

            var current = GetCurrentGod();

            Debug.Log($"[GodUI] 현재 godName: {current.GodName}, fileName: {current.FileName}");
            Debug.Log($"[GodUI] GetDescription(): {current.codexPath}");
        }
        
        public void InitializeUI() // 초기 신 목록 로드 및 첫 UI 갱신
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

        private string GenerateEffectText(GodData god) // 신 효과 설명 텍스트 생성
        {
            var sb = new System.Text.StringBuilder();

            // 1. 이름
            sb.AppendLine($"<size=100>{god.GodName}</size>");
            sb.AppendLine();

            // 2. 설명
            sb.AppendLine($"{god.codexText}");
            sb.AppendLine();

            // 3. 고유 효과 먼저 출력
            bool hasEffect = false;
            foreach (var effect in god.SpecialEffect)
            {
                if (effect is not StatModifierEffect)
                {
                    sb.AppendLine("- [고유 효과] " + effect.ToString());
                    hasEffect = true;
                }
            }

            // 4. 스탯 효과 출력
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

            // 5. 아무 효과도 없을 경우
            if (!hasEffect)
            {
                sb.AppendLine("- 효과 없음");
            }

            return sb.ToString();
        }

        public void GodSelectToCharacterSelect()// 다음 버튼 (종족선택으로)
        {
            godSelect.SetActive(false);
            characterSelect.SetActive(true);
            player.selectedGod = GetCurrentGod();
        }

        public void PreviousButton() // 이전 버튼 (캐릭터 선택으로 되돌아가기)
        {
            godSelect.SetActive(false);
            gameMenu.SetActive(false);
            selectMenu.SetActive(true);
            Player.Instance.selectedGod = null;
        }
    }
}