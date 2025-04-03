using Game;
using PlayerScript;
using Race;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class RaceUI : MonoBehaviour
    {
        [SerializeField] private Image mainImg; // 선택된 종족의 메인 이미지
        [SerializeField] private TMP_Text nameArea; // 선택된 종족 이름
        [SerializeField] private TMP_Text descriptionArea; // 종족 설명
        [SerializeField] private GameObject raceCollection; // 종족 선택 창
        [SerializeField] private Transform buttonContainer; // Scroll View의 Content
        [SerializeField] private Button raceButtonPrefab; // 버튼 프리팹
        [SerializeField] private GameObject traitSelect;
        [SerializeField] private GameObject characterSelect;
        [SerializeField] private GameObject godSelect;
        [SerializeField] private Button unlockButton; // 해금 버튼

        private int currentTribeIndex; // 선택된 종족의 인덱스
        private int currentSubRaceIndex; // 선택된 파생 캐릭터의 인덱스
        [SerializeField] private Sprite questionImg;
        [SerializeField] private Image leftImg;
        [SerializeField] private Image rightImg;
        [SerializeField] private Image midImg;
        [SerializeField] private Button leftButton;
        [SerializeField] private Button rightButton;
        [SerializeField] private bool isFirst;

        private void Start()
        {
            CreateRaceButtons();
            HideDerivation();
        }

        // 종족 버튼 생성 및 초기화
        private void CreateRaceButtons()
        {
            if (DatabaseManager.Instance.raceList == null)
            {
                Debug.LogWarning("종족데이터베이스가 없습니다!");
                return;
            }

            for (int i = 0; i < DatabaseManager.Instance.raceList.Count; i++)
            {
                Button button = Instantiate(raceButtonPrefab, buttonContainer);
                Image buttonImage = button.GetComponent<Image>();
                buttonImage.sprite = DatabaseManager.Instance.raceList[i].raceImage;
                int index = i;
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => SelectRace(index));
            }
        }

        // 종족 선택 메서드
        private void SelectRace(int raceIndex)
        {
            currentTribeIndex = raceIndex;
            currentSubRaceIndex = 0; // 항상 첫 번째 파생 캐릭터를 보여줌

            RaceData selectedRace = DatabaseManager.Instance.raceList[currentTribeIndex];
            mainImg.sprite = selectedRace.raceImage;
            nameArea.text = selectedRace.raceName;
            //Player.Instance.Race = selectedRace.race;
            raceCollection.SetActive(false);

            // 첫 번째 파생 캐릭터 UI 업데이트
            if (selectedRace.subRace.Count > 0)
            {
                SelectDerivedCharacter(0);
            }
        }
        

        private void SelectDerivedCharacter(int characterIndex)
        {
            currentSubRaceIndex = characterIndex;
            
            RaceData currentTribe = DatabaseManager.Instance.raceList[currentTribeIndex];
            if (currentTribe.subRace.Count == 0)
                return;

            SubRaceData selectedCharacter = currentTribe.subRace[currentSubRaceIndex];
            // 캐릭터가 해금되었는지 여부에 따라 다른 정보를 표시

            bool isUnknown = selectedCharacter.subRaceName == "Unknown";

            if(isUnknown)
            {
                mainImg.sprite = questionImg;
                nameArea.text = "???";
                descriptionArea.text = "준비중인 캐릭터입니다.";
                ConfigureButton(false, 0f, true);
            }

            else if (selectedCharacter.isUnlocked)
            {
                mainImg.sprite = selectedCharacter.subRaceImage;
                mainImg.color = Color.white; 
                nameArea.text = selectedCharacter.subRaceName;
                descriptionArea.text = $"{selectedCharacter.subRaceName} <sprite=0>\n\n{selectedCharacter.subRaceDescription}";

                // 버튼을 '다음 단계'로 설정
                ConfigureButton(false);
            }
            else
            {
                mainImg.sprite = selectedCharacter.subRaceImage; // 해금되지 않은 경우 OffImg 사용
                mainImg.color = new Color(0.5f, 0.5f, 0.5f); // 회색으로 설정
                nameArea.text = "???";
                descriptionArea.text = $"{selectedCharacter.unlockHint}\n\n해금 비용: {selectedCharacter.requireFaith} 신앙 재화";

                // 버튼을 '해금하기'로 설정
                ConfigureButton(true, selectedCharacter.requireFaith);
            }

            UpdateUI();
        }

        private void ConfigureButton(bool isUnlock, float cost = 0f, bool isUnavailable = false)
        {
            if (isUnavailable)
            {
                //선택 불가 버튼으로 변경
                unlockButton.GetComponentInChildren<TMP_Text>().text = "선택불가";
                unlockButton.onClick.RemoveAllListeners();
                unlockButton.interactable = false;
            }

            else if (isUnlock)
            {
                // 해금하기 버튼 설정
                unlockButton.GetComponentInChildren<TMP_Text>().text = "해금하기";
                unlockButton.onClick.RemoveAllListeners();
                unlockButton.onClick.AddListener(UnlockCharacter);

                // 신앙 포인트가 충분한지 확인하여 버튼 활성화/비활성화
                unlockButton.interactable = Player.Instance.GetFaithPoint() >= cost;
            }
            else
            {
                // 다음 단계 버튼 설정
                unlockButton.GetComponentInChildren<TMP_Text>().text = "다음단계";
                unlockButton.onClick.RemoveAllListeners();
                unlockButton.onClick.AddListener(ChcToTrait);
                unlockButton.interactable = true; // 항상 활성화
            }

            unlockButton.gameObject.SetActive(true);
        }
        public void UnlockCharacter()
        {
            RaceData currentTribe = DatabaseManager.Instance.raceList[currentTribeIndex];
            SubRaceData selectedCharacter = currentTribe.subRace[currentSubRaceIndex];

            if (!selectedCharacter.isUnlocked && Player.Instance.SpendFaith(selectedCharacter.requireFaith))
            {
                selectedCharacter.isUnlocked = true;
                Debug.Log($"{selectedCharacter.subRaceName}이(가) 해금되었습니다!");

                // UI 업데이트
                SelectDerivedCharacter(currentSubRaceIndex);
            }
            else
            {
                Debug.Log("신앙 재화가 부족합니다.");
            }
        }

        private void UpdateUI()
        {
            RaceData currentTribe = DatabaseManager.Instance.raceList[currentTribeIndex];
            if (currentTribe.subRace.Count == 0)
                return;

            int count = currentTribe.subRace.Count;
            int leftIndex = (currentSubRaceIndex - 1 + count) % count;
            int rightIndex = (currentSubRaceIndex + 1) % count;

            isFirst = true;
            HideDerivation();

            midImg.sprite = currentTribe.subRace[currentSubRaceIndex].subRaceImage;
            midImg.color = currentTribe.subRace[currentSubRaceIndex].isUnlocked ? Color.white : new Color(0.5f, 0.5f, 0.5f);
            leftImg.sprite = currentTribe.subRace[leftIndex].subRaceImage;
            leftImg.color = currentTribe.subRace[leftIndex].isUnlocked ? Color.white : new Color(0.5f, 0.5f, 0.5f);
            rightImg.sprite = currentTribe.subRace[rightIndex].subRaceImage;
            rightImg.color = currentTribe.subRace[rightIndex].isUnlocked ? Color.white : new Color(0.5f, 0.5f, 0.5f);
        }

        public void ShowNextCharacter()
        {
            RaceData currentTribe = DatabaseManager.Instance.raceList[currentTribeIndex];
            if (currentTribe.subRace.Count > 0)
            {
                currentSubRaceIndex = (currentSubRaceIndex + 1) % currentTribe.subRace.Count;
                SelectDerivedCharacter(currentSubRaceIndex);
            }
        }

        public void ShowPreviousCharacter()
        {
            RaceData currentTribe = DatabaseManager.Instance.raceList[currentTribeIndex];
            if (currentTribe.subRace.Count > 0)
            {
                currentSubRaceIndex = (currentSubRaceIndex - 1 + currentTribe.subRace.Count) % currentTribe.subRace.Count;
                SelectDerivedCharacter(currentSubRaceIndex);
            }
        }

        public void HideDerivation()
        {
            if (!isFirst)
            {
                leftImg.gameObject.SetActive(false);
                rightImg.gameObject.SetActive(false);
                midImg.gameObject.SetActive(false);
                leftButton.interactable = false;
                rightButton.interactable = false;
            }

            else
            {
                leftImg.gameObject.SetActive(true);
                rightImg.gameObject.SetActive(true);
                midImg.gameObject.SetActive(true);
                leftButton.interactable = true;
                rightButton.interactable = true;
            }
        }

        #region 이동버튼
        public void MainToSelect()
        {
            raceCollection.SetActive(true);
        }

        public void ChcToTrait()
        {
            Player.Instance.playerImg = mainImg.sprite;
            //Player.Instance.SelectedRace(current);
            Debug.Log($"PlayerImg 변경됨: {Player.Instance.playerImg}"); // 디버깅 코드 추가

            //FindObjectOfType<PlayerUIManager>().UpdatePlayerUI(Player.Instance); // UI 업데이트
            characterSelect.SetActive(false);
            traitSelect.SetActive(true);
        }

        public void PreviousButton()
        {
            Debug.Log("이전 버튼 클릭 종족 -> 신");
            godSelect.SetActive(true);
            isFirst = false;
            HideDerivation();
            characterSelect.SetActive(false);
        }
        #endregion
    }
}