using System.Collections.Generic;
using System.Linq;
using PlayerScript;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Trait
{
    public enum TraitPnN
    {
        Positive,
        Negative
    }

    public enum TraitAnP
    {
        Active,
        Passive,
        Both
    }

    [System.Serializable]
    public class Trait // 특성 클래스
    {
        public string traitName; //특성 이름
        public TraitPnN pnN; //긍정 및 부정 구분
        public TraitAnP anP; //패시브 및 액티브 구분
        public Sprite traitImg; //특성 이미지
        [TextArea]
        public string traitDescription; //특성 설명
        public int cost; // 특성 코스트

    }

    public class TraitScript : MonoBehaviour
    {
        [Header("ObjectControl")]
        [SerializeField] private GameObject characterSelect;
        [SerializeField] private GameObject postivePannel;
        [SerializeField] private GameObject negativePannel;
        [SerializeField] private Button positiveBtn;
        [SerializeField] private Button negativeBtn;

        [Header("TraitControl")]
        [SerializeField] private List<Trait> traitsList = new List<Trait>();
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
        private List<Trait> selectedTraits = new List<Trait>(); // 선택된 특성들을 관리하는 리스트

        // Start is called before the first frame update
        void Start()
        {

            originalSelectedSprite = selectButtonPrefab.image.sprite;
            // 초기 설정 - 시작 시 두 버튼 모두 기본 설정으로
            //SetButtonColors(PostiveBtn, Color.black, Color.white); // Positive 버튼을 흰색 배경, 검은색 글자로 설정
            //SetButtonColors(NegavieBtn, Color.white, Color.black); // Negative 버튼을 흰색 배경, 검은색 글자로 설정

            traitsList = traitsList.OrderBy(t => t.cost).ToList();
            CreateTraitButtons();
            CreateSelectButtons();


        }

        // Update is called once per frame
        void Update()
        {
            costArea.text = "Cost : " + totalCost.ToString() + " / " + Player.Instance.MaxCost;
        }

        public void PostiveBtnAction() //Positive 버튼을 눌렀을 때
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
            if (traitsList == null || traitsList.Count == 0)
            {
                Debug.LogWarning("Race list is empty!");
                return;
            }

            for (int i = 0; i < traitsList.Count; i++)
            {
                // Positive와 Negative에 따라 다른 Container에 추가
                Transform targetContainer = traitsList[i].pnN == TraitPnN.Positive ? pbuttonContainer : nbuttonContainer;

                // 버튼 프리팹을 Content에 추가
                Button button = Instantiate(buttonPrefab, targetContainer);

                // 버튼의 이미지 컴포넌트에 특성 이미지 설정
                Image buttonImage = button.GetComponent<Image>();
                buttonImage.sprite = traitsList[i].traitImg;

                // 버튼 클릭 이벤트 연결 (기존 리스너 제거 후 추가)
                int index = i;
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => PushTrait(traitsList[index]));
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

      private void RemoveTrait(int index)
        {
            if (index < selectedTraits.Count)
            {
                Trait trait = selectedTraits[index];

                int traitCost = selectedTraits[index].cost;
                // 선택 취소
                selectedTraits.RemoveAt(index);
                if (trait.pnN == TraitPnN.Positive)
                {
                    totalCost -= traitCost;
                }
                else if (trait.pnN == TraitPnN.Negative)
                {
                    totalCost += traitCost;
                }

                // 버튼을 원래 이미지로 복구
                ReorganizeSelectButtons();
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
                Button selectButton = selectButtonList[i];
                selectButton.image.sprite = selectedTraits[i].traitImg;
            }
        }

        private void PushTrait(Trait selectedTrait)
        {
            // 특성 설명 업데이트
            descriptionArea.text = $"{selectedTrait.traitName}\n[Cost: {selectedTrait.cost}]\n\n{selectedTrait.traitDescription}";
            //중복 선택 확인
            for (int i = 0; i < selectButtonList.Count; i++)
            {
                if (selectedTraits.Contains(selectedTrait))
                {
                    Debug.Log("중복선택");
                    return;
                }
            }
            // 선택 가능한 빈 자리가 있는지 확인
            if (selectedTraits.Count < selectButtonList.Count)
            {
                if (Player.Instance.MaxCost < totalCost + selectedTrait.cost)
                {
                    Debug.Log("코스트 부족");
                    return;
                }
                // 특성을 선택 리스트에 추가 (적용은 나중에)
                selectedTraits.Add(selectedTrait);

                // 특성 비용을 TotalCost에 추가
                if (selectedTrait.pnN == TraitPnN.Positive)
                {
                    totalCost += selectedTrait.cost;
                }

                else if (selectedTrait.pnN == TraitPnN.Negative)
                {
                    totalCost -= selectedTrait.cost;
                }
                // 선택된 특성의 이미지를 하단 버튼에 반영
                for (int i = 0; i < selectedTraits.Count; i++)
                {
                    Button selectButton = selectButtonList[i];
                    selectButton.image.sprite = selectedTraits[i].traitImg; // 선택된 특성 이미지 설정
                }

                // 하단 버튼을 선택된 특성의 이미지로 업데이트 후, 나머지는 원래 상태로 복구
                ReorganizeSelectButtons();
            }
            else
            {
                Debug.Log("더 이상 선택할 수 있는 버튼이 없습니다.");
            }


        }

        public void OnNextButtonPressed()
        {
            // 선택된 특성들을 플레이어에게 적용
            Player.Instance.ApplySelectedTraits(selectedTraits);


            // 다음 화면으로 전환 (예: 특성 선택이 끝난 후 캐릭터 생성 화면)
            SceneManager.LoadScene("GameScene");
        }

        public void PreviousButtion()
        {
            gameObject.SetActive(false);
            characterSelect.SetActive(true);
        }

    }
}