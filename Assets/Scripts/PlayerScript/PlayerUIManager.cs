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

public class PlayerUIManager : MonoBehaviour
{
    [Header("플레이어 UI")]
    [SerializeField] private Image playerImage;
    [SerializeField] private TMP_Text playerRace;
    [SerializeField] private TMP_Text playerStats;
    [SerializeField] private Transform traitsContainer;
    [SerializeField] private GameObject traitsPrefab;

    [Header("정보창 UI")]
    [SerializeField] private GameObject traitTextBox; // 마우스 오버 시 표시될 텍스트 박스
    [SerializeField] private TMP_Text traitText; // 특성 설명 텍스트
    [SerializeField] private GameObject PlayerInfoBox; // 플레이어 정보 박스

    public void UpdatePlayerUI(Player player)
    {
        player = Player.Instance;
        Debug.Log($"UI 업데이트: {player.PlayerImg} -> {playerImage.sprite}"); // 디버깅 코드 추가


        //플레이어 데이터 동기화
        playerImage.sprite = player.PlayerImg;
        playerRace.text = player.Race.ToString();
        playerStats.text = $@"
        체력 : {player.GetStat("CurrentHP")} / {player.GetStat("HP")}
        마나 : {player.GetStat("CurrentMP")} / {player.GetStat("MP")}
        골드 : {player.GetStat("Gold")}
        신앙 포인트 : {player.GetStat("FaithPoint")}";

        UpdateTraitsUI();

    }

    /*public void UpdateTraits(List<Trait> traits)
    {
        //기존 특성 오브젝트 제거
        foreach (Transform child in traitsContainer)
        {
            Destroy(child.gameObject);
        }

        //다시 추가
        foreach (var trait in traits)
        {
            GameObject traitObj = Instantiate(traitsPrefab, traitsContainer);
            Image traitImage = traitObj.GetComponent<Image>();
            traitImage.sprite = trait.TraitImg;

            if (traitsPrefab == null)
            {
                Debug.LogError("Traits Prefab이 연결되지 않았습니다!");
                return;
            }
        }
    }*/

    public void UpdateTraitsUI()
    {
        List<Trait> traits = Player.Instance.selectedTraits;

        foreach (Transform child in traitsContainer) // 기존 UI 삭제
        {
            Destroy(child.gameObject);
        }

        foreach (Trait trait in traits)
        {
            GameObject traitObj = Instantiate(traitsPrefab, traitsContainer); // UI 생성
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
        UpdatePlayerUI(Player.Instance);
        PlayerInfoBox.SetActive(true);       
    }

    public void ClosePlayerInfoBox()
    {
        PlayerInfoBox.SetActive(false);
    }


    void Start()
    {
        //UpdatePlayerUI(Player.Instance);
    }

   /* public void RefreshTraitsUI()
    {
        //특성 리스트 업데이트
        UpdateTraits(Player.Instance.selectedTraits);
    }*/

    // Update is called once per frame
    void Update()
    {
        //UpdatePlayerUI(Player.Instance);
    }
}
