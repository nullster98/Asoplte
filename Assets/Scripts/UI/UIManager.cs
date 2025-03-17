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
    // TraitTextBox (��: �˾� â)�� �� ���� TMP_Text ������Ʈ�� ������ �ν����Ϳ��� �Ҵ��ϰų� �ڵ忡�� ����
    public GameObject traitTextBox;
    public TMP_Text traitText;

    // �ش� Ư���� ������ �����մϴ�.
    public string traitDescription;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (traitTextBox != null && traitText != null)
        {
            traitText.text = traitDescription; // Ư�� ������ ǥ��
            traitTextBox.SetActive(true);        // �ؽ�Ʈ �ڽ� Ȱ��ȭ
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (traitTextBox != null)
        {
            traitTextBox.SetActive(false);       // �ؽ�Ʈ �ڽ� ����
        }
    }
}//Ư�� �̹��� ���콺 ȣ�� ���

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("�÷��̾� UI")]
    [SerializeField] private Image Hp; //HP��
    [SerializeField] private Image MP; //Mp��
    [SerializeField] private int currentProgress = 0; //���൵
    [SerializeField] private Slider ProgressSlider; //�����̴�
    [SerializeField] private TMP_Text CurrentGold;
    [SerializeField] private TMP_Text CurrentFaithPoint;
    [SerializeField] private Image playerImage;

    [Header("����â UI")]
    [SerializeField] private GameObject PlayerInfoBox; // �÷��̾� ���� �ڽ�
    [SerializeField] Image InfoImg;
    [SerializeField] TMP_Text CharacterName;
    [SerializeField] TMP_Text InfoStats;

    [SerializeField] private GameObject traitTextBox; // ���콺 ���� �� ǥ�õ� �ؽ�Ʈ �ڽ�
    [SerializeField] private TMP_Text traitText; // Ư�� ���� �ؽ�Ʈ
    [SerializeField] Transform TraitContainer;
    [SerializeField] GameObject TraitPrefab; //Ư���̹��� ������

    public Action<int> OnChoiceSelected;


    public void UpdatePlayerUI(Player player)
    {
        player = Player.Instance;
        Debug.Log($"UI ������Ʈ: {player.PlayerImg} -> {playerImage.sprite}"); // ����� �ڵ� �߰�


        //�÷��̾� ������ ����ȭ
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
        ü�� : {player.GetStat("CurrentHP")} / {player.GetStat("HP")}
        ���� : {player.GetStat("CurrentMP")} / {player.GetStat("MP")}
        ���ݷ� : {player.GetStat("Atk")} ���� : {player.GetStat("Def")}
        ��� : {player.GetStat("Gold")}
        �ž� ����Ʈ : {player.GetStat("FaithStat")}";

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

        foreach (Transform child in TraitContainer) // ���� UI ����
        {
            Destroy(child.gameObject);
        }

        foreach (Trait trait in traits)
        {
            GameObject traitObj = Instantiate(TraitPrefab, TraitContainer); // UI ����
            Image traitImage = traitObj.GetComponent<Image>();
            traitImage.sprite = trait.TraitImg;

            // ���콺 ȣ�� ��� �߰�
            TraitHoverHandler hoverHandler = traitObj.AddComponent<TraitHoverHandler>();
            hoverHandler.traitTextBox = traitTextBox;
            hoverHandler.traitText = traitText;
            hoverHandler.traitDescription = trait.TraitDescription;
        }
    }

    //  �÷��̾� ���� �ڽ��� �� ���� Ư�� UI�� �����ϵ��� ����
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
