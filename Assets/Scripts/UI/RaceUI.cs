using System.Collections.Generic;
using PlayerScript;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    [System.Serializable]
    public class DerivedCharacter
    {
        public string characterName;
        public Sprite characterImg;
        public Sprite onImg;
        public Sprite offImg;

        [TextArea]
        public string characterDescription;
        [TextArea]
        public string characterStat;

        [TextArea]
        public string unlockHint;
        public float requireFaith; // 해금 비용
        public bool isUnlocked = false; // 활성화 상태
    }

    [System.Serializable]
    public class Tribe
    {
        public Sprite raceImg;
        public Sprite offRaceImg;
        public string RaceName;
        //public Race race;
        public float RequireFaith; // 해금 비용

        // 파생 캐릭터 리스트 추가
        public List<DerivedCharacter> DerivedCharacters;

        public bool IsUnlocked = false; // 활성화 상태
    }

    public class RaceUI : MonoBehaviour
    {
        [SerializeField] private Image MainImg; // 선택된 종족의 메인 이미지
        [SerializeField] private TMP_Text NameArea; // 선택된 종족 이름
        [SerializeField] private TMP_Text DescriptionArea; // 종족 설명
        [SerializeField] private List<Tribe> raceList = new List<Tribe>(); // 종족 리스트
        [SerializeField] private GameObject RaceCollection; // 종족 선택 창
        [SerializeField] private Transform buttonContainer; // Scroll View의 Content
        [SerializeField] private Button raceButtonPrefab; // 버튼 프리팹
        [SerializeField] private GameObject TratiSelect;
        [SerializeField] private GameObject GodSelect;
        [SerializeField] private Button unlockButton; // 해금 버튼

        private int currentRaceIndex = 0; // 선택된 종족의 인덱스
        private int currentCharacterIndex = 0; // 선택된 파생 캐릭터의 인덱스
        [SerializeField] private Sprite QuestionImg;
        [SerializeField] private Image LeftImg;
        [SerializeField] private Image RightImg;
        [SerializeField] private Image MidImg;
        [SerializeField] private Button LeftButton;
        [SerializeField] private Button RightButton;
        [SerializeField] private bool isFirst = false;

        private void Start()
        {
            CreateRaceButtons();
            HideDerivation();
        }

        // 종족 버튼 생성 및 초기화
        private void CreateRaceButtons()
        {
            if (raceList == null || raceList.Count == 0)
            {
                Debug.LogWarning("Race list is empty!");
                return;
            }

            for (int i = 0; i < raceList.Count; i++)
            {
                Button button = Instantiate(raceButtonPrefab, buttonContainer);
                Image buttonImage = button.GetComponent<Image>();
                buttonImage.sprite = raceList[i].raceImg;
                int index = i;
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => SelectRace(index));
            }
        }

        // 종족 선택 메서드
        private void SelectRace(int raceIndex)
        {
            currentRaceIndex = raceIndex;
            currentCharacterIndex = 0; // 항상 첫 번째 파생 캐릭터를 보여줌

            Tribe selectedRace = raceList[currentRaceIndex];
            MainImg.sprite = selectedRace.raceImg;
            NameArea.text = selectedRace.RaceName;
            //Player.Instance.Race = selectedRace.race;
            RaceCollection.SetActive(false);

            EnsureMinimumCharacters(selectedRace);

            // 첫 번째 파생 캐릭터 UI 업데이트
            if (selectedRace.DerivedCharacters.Count > 0)
            {
                SelectDerivedCharacter(0);
            }
        }

        private void EnsureMinimumCharacters(Tribe tribe)
        {
            // 파생 캐릭터가 3개 미만이면 자동으로 추가
            while (tribe.DerivedCharacters.Count < 3)
            {
                DerivedCharacter placeholderCharacter = new DerivedCharacter
                {
                    characterName = "Unknown",
                    characterImg = QuestionImg,
                    onImg = QuestionImg,
                    offImg = QuestionImg,
                    requireFaith = 0f, // 기본값 설정
                    characterDescription = "???",
                    characterStat = "???",
                    unlockHint = "준비중 입니다."
                };

                tribe.DerivedCharacters.Add(placeholderCharacter);
            }
        }

        private void SelectDerivedCharacter(int characterIndex)
        {
            currentCharacterIndex = characterIndex;

            Tribe currentTribe = raceList[currentRaceIndex];
            if (currentTribe.DerivedCharacters.Count == 0)
                return;

            DerivedCharacter selectedCharacter = currentTribe.DerivedCharacters[currentCharacterIndex];
            // 캐릭터가 해금되었는지 여부에 따라 다른 정보를 표시

            bool isUnknown = selectedCharacter.characterName == "Unknown";

            if(isUnknown)
            {
                MainImg.sprite = QuestionImg;
                NameArea.text = "???";
                DescriptionArea.text = "준비중인 캐릭터입니다.";
                ConfigureButton(false, 0f, true);
            }

            else if (selectedCharacter.isUnlocked)
            {
                MainImg.sprite = selectedCharacter.characterImg;
                NameArea.text = selectedCharacter.characterName;
                DescriptionArea.text = $"{selectedCharacter.characterName} <sprite=0>\n\n{selectedCharacter.characterDescription}\n\n{selectedCharacter.characterStat}";

                // 버튼을 '다음 단계'로 설정
                ConfigureButton(false);
            }
            else
            {
                MainImg.sprite = selectedCharacter.offImg; // 해금되지 않은 경우 OffImg 사용
                NameArea.text = "???";
                DescriptionArea.text = $"{selectedCharacter.unlockHint}\n\n해금 비용: {selectedCharacter.requireFaith} 신앙 재화";

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
            Tribe currentTribe = raceList[currentRaceIndex];
            DerivedCharacter selectedCharacter = currentTribe.DerivedCharacters[currentCharacterIndex];

            if (!selectedCharacter.isUnlocked && Player.Instance.SpendFaith(selectedCharacter.requireFaith))
            {
                selectedCharacter.isUnlocked = true;
                Debug.Log($"{selectedCharacter.characterName}이(가) 해금되었습니다!");

                // UI 업데이트
                SelectDerivedCharacter(currentCharacterIndex);
            }
            else
            {
                Debug.Log("신앙 재화가 부족합니다.");
                return;
            }
        }

        private void UpdateUI()
        {
            Tribe currentTribe = raceList[currentRaceIndex];
            if (currentTribe.DerivedCharacters.Count == 0)
                return;

            int count = currentTribe.DerivedCharacters.Count;
            int leftIndex = (currentCharacterIndex - 1 + count) % count;
            int rightIndex = (currentCharacterIndex + 1) % count;

            isFirst = true;
            HideDerivation();

            MidImg.sprite = currentTribe.DerivedCharacters[currentCharacterIndex].characterImg;
            LeftImg.sprite = currentTribe.DerivedCharacters[leftIndex].offImg;
            RightImg.sprite = currentTribe.DerivedCharacters[rightIndex].offImg;
        }

        public void ShowNextCharacter()
        {
            Tribe currentTribe = raceList[currentRaceIndex];
            if (currentTribe.DerivedCharacters.Count > 0)
            {
                currentCharacterIndex = (currentCharacterIndex + 1) % currentTribe.DerivedCharacters.Count;
                SelectDerivedCharacter(currentCharacterIndex);
            }
        }

        public void ShowPreviousCharacter()
        {
            Tribe currentTribe = raceList[currentRaceIndex];
            if (currentTribe.DerivedCharacters.Count > 0)
            {
                currentCharacterIndex = (currentCharacterIndex - 1 + currentTribe.DerivedCharacters.Count) % currentTribe.DerivedCharacters.Count;
                SelectDerivedCharacter(currentCharacterIndex);
            }
        }

        public void HideDerivation()
        {
        

            if (!isFirst)
            {
                LeftImg.gameObject.SetActive(false);
                RightImg.gameObject.SetActive(false);
                MidImg.gameObject.SetActive(false);
                LeftButton.interactable = false;
                RightButton.interactable = false;
            }

            else
            {
                LeftImg.gameObject.SetActive(true);
                RightImg.gameObject.SetActive(true);
                MidImg.gameObject.SetActive(true);
                LeftButton.interactable = true;
                RightButton.interactable = true;
            }
        }

        #region 이동버튼
        public void MainToSelect()
        {
            RaceCollection.SetActive(true);
        }

        public void ChcToTrait()
        {
            Player.Instance.playerImg = MainImg.sprite;
            Debug.Log($"PlayerImg 변경됨: {Player.Instance.playerImg}"); // 디버깅 코드 추가

            //FindObjectOfType<PlayerUIManager>().UpdatePlayerUI(Player.Instance); // UI 업데이트
            this.gameObject.SetActive(false);
            TratiSelect.SetActive(true);
        }

        public void PreviousButton()
        {
            gameObject.SetActive(false);
            GodSelect.SetActive(true);
            isFirst = false;
            HideDerivation();
        }
        #endregion
    }
}