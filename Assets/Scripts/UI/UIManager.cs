using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Build.Player;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
    public static UIManager Instance { get; private set; }

    [Header("플레이어 UI")]
    [SerializeField] private Image Hp; //HP바
    [SerializeField] private Image MP; //Mp바
    [SerializeField] private int currentProgress = 0; //진행도
    [SerializeField] private Slider ProgressSlider; //슬라이더
    [SerializeField] private TMP_Text CurrentGold;
    [SerializeField] private TMP_Text CurrentFaithPoint;
    [SerializeField] private Image playerImage;

    [Header("정보창 UI")]
    [SerializeField] private GameObject PlayerInfoBox; // 플레이어 정보 박스
    [SerializeField] Image InfoImg;
    [SerializeField] TMP_Text CharacterName;
    [SerializeField] TMP_Text InfoStats;

    [SerializeField] private GameObject traitTextBox; // 마우스 오버 시 표시될 텍스트 박스
    [SerializeField] private TMP_Text traitText; // 특성 설명 텍스트
    [SerializeField] Transform TraitContainer;
    [SerializeField] GameObject TraitPrefab; //특성이미지 프리팹

    public Action<int> OnChoiceSelected;


    public void UpdatePlayerUI(Player player)
    {
        player = Player.Instance;
        Debug.Log($"UI 업데이트: {player.PlayerImg} -> {playerImage.sprite}"); // 디버깅 코드 추가


        //플레이어 데이터 동기화
        playerImage.sprite = player.PlayerImg;
        CurrentGold.text = player.GetStat("Gold").ToString();
        CurrentFaithPoint.text = player.GetStat("FaithPoint").ToString();
    }

    public void UpdatePlayerInfo(Player player)
    {
        player = Player.Instance;

        InfoImg.sprite = player.PlayerImg;
        CharacterName.text = player.Race.ToString();
        InfoStats.text = $@"
        체력 : {player.GetStat("CurrentHP")} / {player.GetStat("HP")}
        마나 : {player.GetStat("CurrentMP")} / {player.GetStat("MP")}
        공격력 : {player.GetStat("Atk")} 방어력 : {player.GetStat("Def")}
        골드 : {player.GetStat("Gold")}
        신앙 포인트 : {player.GetStat("FaithStat")}";

        UpdateTraitsUI();
    }

    public void UpdateHPUI()
    {
        if (Player.Instance != null)
            Hp.fillAmount = Player.Instance.GetStat("CurrentHP") / Player.Instance.GetStat("HP");
    }

    public void UpdateTraitsUI()
    {
        List<Trait> traits = Player.Instance.selectedTraits;

        foreach (Transform child in TraitContainer) // 기존 UI 삭제
        {
            Destroy(child.gameObject);
        }

        foreach (Trait trait in traits)
        {
            GameObject traitObj = Instantiate(TraitPrefab, TraitContainer); // UI 생성
            Image traitImage = traitObj.GetComponent<Image>();
            traitImage.sprite = trait.TraitImg;

            // 마우스 호버 기능 추가
            TraitHoverHandler hoverHandler = traitObj.AddComponent<TraitHoverHandler>();
            hoverHandler.traitTextBox = traitTextBox;
            hoverHandler.traitText = traitText;
            hoverHandler.traitDescription = trait.TraitDescription;
        }
    }

    //  플레이어 정보 박스를 열 때만 특성 UI를 갱신하도록 설정
    public void OpenPlayerInfoBox()
    {
        UpdatePlayerInfo(Player.Instance);
        PlayerInfoBox.SetActive(true);
    }

    public void ClosePlayerInfoBox()
    {
        PlayerInfoBox.SetActive(false);
    }



    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {

    }


    // Update is called once per frame
    void Update()
    {
        UpdatePlayerUI(Player.Instance);
    }
}
