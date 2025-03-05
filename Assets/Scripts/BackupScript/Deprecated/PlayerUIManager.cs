using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Build.Player;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/*
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

public class PlayerUIManager : MonoBehaviour
{
    [Header("�÷��̾� UI")]
    [SerializeField] private Image playerImage;
    [SerializeField] private TMP_Text playerRace;
    [SerializeField] private TMP_Text playerStats;
    [SerializeField] private Transform traitsContainer;
    [SerializeField] private GameObject traitsPrefab;

    [Header("����â UI")]
    [SerializeField] private GameObject traitTextBox; // ���콺 ���� �� ǥ�õ� �ؽ�Ʈ �ڽ�
    [SerializeField] private TMP_Text traitText; // Ư�� ���� �ؽ�Ʈ
    [SerializeField] private GameObject PlayerInfoBox; // �÷��̾� ���� �ڽ�

    [Header("�̺�Ʈ UI")]
    [SerializeField] private TMP_Text eventDescriptionText;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private GameObject buttonPrefab;

    public void UpdatePlayerUI(Player player)
    {
        player = Player.Instance;
        Debug.Log($"UI ������Ʈ: {player.PlayerImg} -> {playerImage.sprite}"); // ����� �ڵ� �߰�


        //�÷��̾� ������ ����ȭ
        playerImage.sprite = player.PlayerImg;
        playerRace.text = player.Race.ToString();
        playerStats.text = $@"
        ü�� : {player.GetStat("CurrentHP")} / {player.GetStat("HP")}
        ���� : {player.GetStat("CurrentMP")} / {player.GetStat("MP")}
        ��� : {player.GetStat("Gold")}
        �ž� ����Ʈ : {player.GetStat("FaithPoint")}";

        UpdateTraitsUI();

    }

    public void UpdateEventUI(string eventDescription, List<EventChoice> choices)
    {
        eventDescriptionText.text = eventDescription;

        // ���� ��ư ����
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        // ���ο� ��ư ����
        foreach (var choice in choices)
        {
            GameObject newButton = Instantiate(buttonPrefab, buttonContainer);
            newButton.GetComponentInChildren<TMP_Text>().text = choice.ChoiceName;

            // ��ư Ŭ�� �̺�Ʈ �߰�
            Button buttonComponent = newButton.GetComponent<Button>();
            buttonComponent.onClick.AddListener(() => EventHandler.Instance.OnChoiceSelected(choices.IndexOf(choice)));
        }
    }

    public void UpdateTraitsUI()
    {
        List<Trait> traits = Player.Instance.selectedTraits;

        foreach (Transform child in traitsContainer) // ���� UI ����
        {
            Destroy(child.gameObject);
        }

        foreach (Trait trait in traits)
        {
            GameObject traitObj = Instantiate(traitsPrefab, traitsContainer); // UI ����
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
        UpdatePlayerUI(Player.Instance);
        PlayerInfoBox.SetActive(true);       
    }

    public void ClosePlayerInfoBox()
    {
        PlayerInfoBox.SetActive(false);
    }

    private void Awake()
    {
    
    }

    void Start()
    {

    }


    // Update is called once per frame
    void Update()
    {

    }
}
*/