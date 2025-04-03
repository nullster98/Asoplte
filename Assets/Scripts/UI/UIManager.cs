using System;
using System.Collections.Generic;
using PlayerScript;
using TMPro;
using Trait;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class TraitHoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        // TraitTextBox (예: 팝업 창)와 그 내부 TMP_Text 컴포넌트의 참조를 인스펙터에서 할당하거나 코드에서 설정
        public GameObject traitTextBox;
        public TMP_Text traitText;

        // 해당 특성의 설명을 저장합니다.
        public string traitDescription;

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (traitTextBox != null && traitText != null)
            {
                traitText.text = traitDescription; // 특성 설명을 표시
                traitTextBox.SetActive(true);        // 텍스트 박스 활성화
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (traitTextBox != null)
            {
                traitTextBox.SetActive(false);       // 텍스트 박스 숨김
            }
        }
    }//특성 이미지 마우스 호버 기능

    public class UIManager : MonoBehaviour
    {
        private static UIManager Instance { get; set; }

        [Header("플레이어 UI")]
        [SerializeField] private Image hp; //HP바
        [SerializeField] private Image mp; //Mp바
        [SerializeField] private int currentProgress; //진행도
        [SerializeField] private Slider progressSlider; //슬라이더
        [SerializeField] private TMP_Text currentGold;
        [SerializeField] private TMP_Text currentFaithPoint;
        [SerializeField] private Image playerImage;

        [Header("정보창 UI")]
        [SerializeField] private GameObject playerInfoBox; // 플레이어 정보 박스
        [SerializeField] private Image infoImg;
        [SerializeField] private TMP_Text characterName;
        [SerializeField] private TMP_Text infoStats;

        [SerializeField] private GameObject traitTextBox; // 마우스 오버 시 표시될 텍스트 박스
        [SerializeField] private TMP_Text traitText; // 특성 설명 텍스트
        [SerializeField] private Transform traitContainer;
        [SerializeField] private GameObject traitPrefab; //특성이미지 프리팹

        public Action<int?> OnChoiceSelected;


        public void UpdatePlayerUI()
        {
            Player player = Player.Instance;
            Debug.Log($"UI 업데이트: {player.playerImg} -> {playerImage.sprite}"); // 디버깅 코드 추가


            //플레이어 데이터 동기화
            playerImage.sprite = player.playerImg;
            currentGold.text = player.GetStat("Gold").ToString();
            currentFaithPoint.text = player.GetStat("FaithPoint").ToString();
        }

        private void UpdatePlayerInfo()
        {
            Player player = Player.Instance;

            infoImg.sprite = player.playerImg;
            //CharacterName.text = player.Race.ToString();
            infoStats.text = $@"
            체력 : {player.GetStat("CurrentHP")} / {player.GetStat("HP")}
            마나 : {player.GetStat("CurrentMP")} / {player.GetStat("MP")}
            공격력 : {player.GetStat("Atk")} 방어력 : {player.GetStat("Def")}
            골드 : {player.GetStat("Gold")}
            신앙 포인트 : {player.GetStat("FaithStat")}";

            UpdateTraitsUI();
        }

        public void UpdateHpui()
        {
            if (Player.Instance != null)
                // ReSharper disable once PossibleLossOfFraction
                hp.fillAmount = Player.Instance.GetStat("CurrentHP") / Player.Instance.GetStat("HP");
        }

        private void UpdateTraitsUI()
        {
            List<TraitData> traits = Player.Instance.selectedTraits;

            foreach (Transform child in traitContainer) // 기존 UI 삭제
            {
                Destroy(child.gameObject);
            }

            foreach (TraitData trait in traits)
            {
                GameObject traitObj = Instantiate(traitPrefab, traitContainer); // UI 생성
                Image traitImage = traitObj.GetComponent<Image>();
                traitImage.sprite = trait.traitImage;

                // 마우스 호버 기능 추가
                TraitHoverHandler hoverHandler = traitObj.AddComponent<TraitHoverHandler>();
                hoverHandler.traitTextBox = traitTextBox;
                hoverHandler.traitText = traitText;
                hoverHandler.traitDescription = trait.traitDescription;
            }
        }

        //  플레이어 정보 박스를 열 때만 특성 UI를 갱신하도록 설정
        public void OpenPlayerInfoBox()
        {
            UpdatePlayerInfo();
            playerInfoBox.SetActive(true);
        }

        public void ClosePlayerInfoBox()
        {
            playerInfoBox.SetActive(false);
        }



        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

    }
}