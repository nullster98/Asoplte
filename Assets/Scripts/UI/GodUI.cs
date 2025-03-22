using God;
using PlayerScript;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GodUI : MonoBehaviour
    {
        [SerializeField] private GodDatabase godDatabase;
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
            if (godDatabase == null || godDatabase.godList.Count == 0)
            {
                Debug.LogWarning("GodDatabase가 비어 있습니다! 인스펙터에서 설정하세요.");
                return;
            }
            
        }

        public void OnNextButtonPressed()
        {
            if (godDatabase.godList.Count == 0) return;
            currentIndex = (currentIndex + 1) % godDatabase.godList.Count;
            UpdateUI();
        }

        public void OnPreviousButtonPressed()
        {
            if (godDatabase.godList.Count == 0) return;
            currentIndex = (currentIndex - 1 + godDatabase.godList.Count) % godDatabase.godList.Count;
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (godDatabase.godList.Count == 0)
            {
                Debug.LogWarning("갓 리스트가 비어 있음!");
                return;
            }

            GodData currentImage = godDatabase.GetGodByIndex(currentIndex);
            if (currentImage == null)
            {
                Debug.LogError($"currentImage is null! Index: {currentIndex}");
                return;
            }

            Debug.Log($"[GodUI] 현재 신: {currentImage.GodName}, ID: {currentImage.GodID}");
            
            mainImage.sprite = currentImage.GetGodImage();
            nameText.text = currentImage.GodName;
            infoText.text = currentImage.GetDescription();
            godBackground.sprite = currentImage.GetBackgroundImage();
            
            Debug.Log(infoText.text);

            // 왼쪽과 오른쪽 이미지 갱신
            int leftIndex = (currentIndex - 1 + godDatabase.godList.Count) % godDatabase.godList.Count;
            int rightIndex = (currentIndex + 1) % godDatabase.godList.Count;

            leftImage.sprite = godDatabase.GetGodByIndex(leftIndex).GetGodImage();
            rightImage.sprite = godDatabase.GetGodByIndex(rightIndex).GetGodImage();

            // 왼쪽과 오른쪽 이미지를 흑백으로 변환
            leftImage.color = Color.gray;
            rightImage.color = Color.gray;
            
            var current = godDatabase.GetGodByIndex(currentIndex);

            Debug.Log($"[GodUI] 현재 godName: {current.GodName}, fileName: {current.FileName}");
            Debug.Log($"[GodUI] GetDescription(): {current.GetDescription()}");
        }
        
        public void InitializeUI()
        {
            if (godDatabase == null)
            {
                Debug.LogError("[GodUI] godDatabase가 null입니다!");
                return;
            }

            if (godDatabase.godList == null || godDatabase.godList.Count == 0)
            {
                Debug.LogError("[GodUI] godDatabase.godList가 비어있습니다!");
                return;
            }

            Debug.Log($"[GodUI] godList 로드 성공! 총 {godDatabase.godList.Count}개");
    
            currentIndex = 0;
            UpdateUI();
        }

        public void GodSelectToCharacterSelect()//신앙선택에서 종족선택으로 넘어가는 버튼
        {
            godSelect.SetActive(false);
            characterSelect.SetActive(true);
            player.SelectedGod = godDatabase.GetGodByIndex(currentIndex);
        }

        public void PreviousButton()
        {
            gameMenu.SetActive(false);
            selectMenu.SetActive(true);
        }
    }
}