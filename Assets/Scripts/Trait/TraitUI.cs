using System.Collections.Generic;
using System.Linq;
using Game;
using PlayerScript;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Trait
{
  public class TraitUI : MonoBehaviour
  {
        [Header("ObjectControl")] 
        [SerializeField] private GameObject traitSelect;
        [SerializeField] private GameObject characterSelect;
        [SerializeField] private GameObject postivePannel;
        [SerializeField] private GameObject negativePannel;
        [SerializeField] private Button positiveBtn;
        [SerializeField] private Button negativeBtn;

        [Header("TraitControl")]
        [SerializeField] private Transform pbuttonContainer; // Scroll View의 Content
        [SerializeField] private Transform nbuttonContainer; // Scroll View의 Content
        [SerializeField] private Button buttonPrefab; //버튼 프리팹
        [SerializeField] private TMP_Text descriptionArea; // 특성 설명창
        [SerializeField] private TMP_Text costArea; //코스트 표기창

        [Header("SelectControl")]
        [SerializeField] private Button selectButtonPrefab;
        [SerializeField] private Transform selectContainer;
        [SerializeField] private int totalCost;
        [SerializeField] private int maxCost;
        private List<Button> selectButtonList = new List<Button>(); // 선택된 버튼들을 관리할 리스트
        //private int nextSelectIndex = 0; // 선택된 버튼의 다음 빈 자리 인덱스
        private Sprite originalSelectedSprite;
        private List<string> selectedTraits = new (); // 선택된 특성들을 관리하는 리스트

        // Start is called before the first frame update
        void Start()
        {

            originalSelectedSprite = selectButtonPrefab.image.sprite;
            // 초기 설정 - 시작 시 두 버튼 모두 기본 설정으로
            //SetButtonColors(PostiveBtn, Color.black, Color.white); // Positive 버튼을 흰색 배경, 검은색 글자로 설정
            //SetButtonColors(NegavieBtn, Color.white, Color.black); // Negative 버튼을 흰색 배경, 검은색 글자로 설정
            
            //비용순 정렬 후 생성
            DatabaseManager.Instance.traitList = DatabaseManager.Instance.traitList.OrderBy(t => t.cost).ToList();
            CreateTraitButtons();
            CreateSelectButtons();


        }
        
        void Update() // 리팩토링 시 이것도 따로 뺴서 함수로 만들기
        {
            costArea.text = "Cost : " + totalCost.ToString() + " / " + Player.Instance.MaxCost;
        }

        public void PositiveBtnAction() //Positive 버튼을 눌렀을 때
        {
            // Positive 버튼 색상: 검은 배경, 흰 글자
            //SetButtonColors(PostiveBtn, Color.black, Color.white);

            // Negative 버튼 색상: 흰 배경, 검은 글자
            //SetButtonColors(NegavieBtn, Color.white, Color.black);

            postivePannel.SetActive(true);
            negativePannel.SetActive(false);
            positiveBtn.transform.SetAsLastSibling();
            postivePannel.transform.SetAsLastSibling();

        }

        public void NegativeBtnAction() //Negative 버튼을 눌렀을 때
        {
            // Negative 버튼 색상: 검은 배경, 흰 글자
            //SetButtonColors(NegavieBtn, Color.black, Color.white);

            // Positive 버튼 색상: 흰 배경, 검은 글자
            //SetButtonColors(PostiveBtn, Color.white, Color.black);

            postivePannel.SetActive(false);
            negativePannel.SetActive(true);
            negativeBtn.transform.SetAsLastSibling();
            negativePannel.transform.SetAsLastSibling();
        }



        /*private void SetButtonColors(Button button, Color backgroundColor, Color textColor)
    {
        // 버튼의 배경 이미지 색상 설정
        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.color = backgroundColor;
        }
        else
        {
            Debug.LogWarning("Button에 Image 컴포넌트가 없습니다.");
        }

        // 버튼의 자식 오브젝트에 있는 TMP_Text 컴포넌트의 색상 설정
        TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
        if (buttonText != null)
        {
            buttonText.color = textColor;
        }
        else
        {
            Debug.LogWarning("Button에 TMP_Text 컴포넌트가 없습니다.");
        }
    }*/


        // 특성 버튼 생성 및 초기화
        private void CreateTraitButtons()
        {
            // 리스트가 비어 있는지 체크
            if (DatabaseManager.Instance == null)
            {
                Debug.LogWarning("특성 list is empty!");
                return;
            }

            for (int i = 0; i < DatabaseManager.Instance.traitList.Count; i++)
            {
                // Positive와 Negative에 따라 다른 Container에 추가
                Transform targetContainer = DatabaseManager.Instance.traitList[i].PnN == TraitPnN.Positive ? pbuttonContainer : nbuttonContainer;

                // 버튼 프리팹을 Content에 추가
                if (DatabaseManager.Instance.traitList[i].isUnlock)
                {
                    Button button = Instantiate(buttonPrefab, targetContainer);


                    // 버튼의 이미지 컴포넌트에 특성 이미지 설정
                    Image buttonImage = button.GetComponent<Image>();
                    buttonImage.sprite = DatabaseManager.Instance.traitList[i].traitImage;

                    // 버튼 클릭 이벤트 연결 (기존 리스너 제거 후 추가)
                    int index = i;
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() => PushTrait(DatabaseManager.Instance.traitList[index]));
                }
            }

        }

        // Select 버튼을 생성하고 원래 이미지로 복구할 수 있도록 설정
        private void CreateSelectButtons()
        {
            for (int i = 0; i < 10; i++)
            {
                // Select 버튼을 생성
                Button button = Instantiate(selectButtonPrefab, selectContainer);
                selectButtonList.Add(button); // 리스트에 추가

                // 클릭 시 원래 이미지로 복구하는 기능 추가
                int index = i; // 인덱스를 저장
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => RemoveTrait(index)); // 선택 취소
            }
        }

      private void RemoveTrait(int index) //이미 선택한 특성 다시 빼기
        {
            if (index < selectedTraits.Count)
            {
                string trait = selectedTraits[index];
                TraitData traitData = DatabaseManager.Instance.GetTraitData(trait);

                if (traitData != null)
                {
                    // 코스트 되돌리기
                    int costChange = (traitData.PnN == TraitPnN.Positive) ? traitData.cost : -traitData.cost;
                    totalCost -= costChange; // PushTrait에서 했던 계산을 반대로

                    // 선택된 ID 리스트에서 제거
                    selectedTraits.RemoveAt(index); // 인덱스로 제거

                    // 선택된 특성 UI 업데이트
                    ReorganizeSelectButtons();

                    // 설명 창 클리어 또는 기본 메시지 표시 (선택적)
                    descriptionArea.text = "특성을 선택하거나 선택된 특성을 눌러 설명을 확인하라냥.";
                }
                else
                {
                    Debug.LogWarning($"제거하려는 특성 ID '{trait}' 데이터를 찾을 수 없다냥!");
                }
            }
        }

        // 선택된 Trait들을 앞에서부터 다시 정리
        private void ReorganizeSelectButtons()
        {
            // 모든 SelectButton의 이미지를 원래 이미지로 복구
            foreach (var button in selectButtonList)
            {
                button.image.sprite = originalSelectedSprite;
            }

            // 선택된 특성들을 다시 앞에서부터 차례대로 배치
            for (int i = 0; i < selectedTraits.Count; i++)
            {
                string traitID = selectedTraits[i];
                TraitData traitData = DatabaseManager.Instance.GetTraitData(traitID);
                Button selectButton = selectButtonList[i];
                selectButton.image.sprite = traitData.traitImage;
            }
        }

        private void PushTrait(TraitData selectedTrait) //특성 눌렀을때
        {
            // 특성 설명 업데이트
            descriptionArea.text = $"{selectedTrait.traitName} [{selectedTrait.rarity}]\n[Cost: {selectedTrait.cost}]\n\n{selectedTrait.codexText}";

            if (selectedTraits.Contains(selectedTrait.traitID))
            {
                Debug.Log("중복선택");
                return;
            }

            // 선택 가능한 슬롯이 있는지 확인
            if (selectedTraits.Count >= selectButtonList.Count)
            {
                Debug.Log("더 이상 선택할 수 있는 버튼이 없습니다.");
                return;
            }

            // 코스트 검사 먼저
            if (selectedTrait.PnN == TraitPnN.Positive &&
                totalCost + selectedTrait.cost > Player.Instance.MaxCost)
            {
                Debug.Log("코스트가 충분하지 않습니다!");
                return;
            }

            // 조건 통과 후 
            selectedTraits.Add(selectedTrait.traitID);

            if (selectedTrait.PnN == TraitPnN.Positive)
            {
                totalCost += selectedTrait.cost;
            }
            else if (selectedTrait.PnN == TraitPnN.Negative)
            {
                totalCost -= selectedTrait.cost;
            }

            ReorganizeSelectButtons();
        }

        public void OnNextButtonPressed()
        {
            foreach (string traitID in selectedTraits)
            {
                Player.Instance.AddTrait(traitID);
            }

            Player.Instance.SelectedGod(Player.Instance.selectedGod);
            Player.Instance.SelectedRace(Player.Instance.selectedRace, Player.Instance.selectedSubRace);
            // 다음 화면으로 전환 (예: 특성 선택이 끝난 후 캐릭터 생성 화면)
            SceneManager.LoadScene("GameScene");
        }

        public void PreviousButtion()
        {
            traitSelect.SetActive(false);
            characterSelect.SetActive(true);
            Player.Instance.selectedRace = null;
        }

    }
}