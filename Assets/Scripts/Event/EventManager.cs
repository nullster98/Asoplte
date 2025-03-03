using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System.Diagnostics.Tracing;
using JetBrains.Annotations;
using UnityEngine.EventSystems;
using System.Linq;
using System;
using UnityEditor;


[System.Flags]
public enum EventTag //�̺�Ʈ �±׵�
{
    None = 0,
    Start = 1 << 0, // 1
    Positive = 1 << 1, // 2
    Negative = 1 << 2, // 4
    Battle = 1 << 3, // 8
    Chaos = 1 << 4, // 16
    Encounter = 1 << 5, // 32
    Rest = 1 << 6, // 64
    Boss = 1 << 7, // 128

    Sequential = 1 << 8 // �̺�Ʈ �ļ�

}

[System.Serializable]
public class EventChoice
{
    public string ChoiceName; //������ �̸�
    public string RequiredTraits; //�ʿ� Ư��
    [SerializeField] private int nextEventID = -1; // �̺�Ʈ ID ����
    
    [NonSerialized] public EventData NextEventRef; // �̵��� �̺�Ʈ ��������

    public int NextEventID
    {
        get => nextEventID;
        set => nextEventID = value;
    }
}

[System.Serializable]
public class EventData //�̺�Ʈ �⺻ ����
{
    public EventTag Tags; //�̺�Ʈ �±�
    public bool isUsed = false;
     
    public string EventName;
    public Sprite DisplayImg;
    [TextArea]
    public string EventText;

    public List<EventChoice> Buttons;

    
}



public class EventManager : MonoBehaviour
{
    [Header("Upper Component")]
    [SerializeField] Image Hp; //HP��
    [SerializeField] Image MP; //Mp��
    [SerializeField] Slider ProgressSlider; //�����̴�
    [SerializeField] int currentProgress = 0; //���൵
    [SerializeField] TMP_Text CurrentGold;
    [SerializeField] TMP_Text CurrentFaithPoint;
    [SerializeField] TMP_Text CharacterName;
    [SerializeField] Image CharacterImg;
    [SerializeField] GameObject PlayerInfoBox;
    [SerializeField] Transform TraitContainer;
    [SerializeField] GameObject TraitPrefeb; //Ư���̹��� ������
    [SerializeField] GameObject TraitTextBox;
    [SerializeField] TMP_Text TraitText; //Ư�� ��
    private EventData CurrentEvent;

    [Header("Main Component")]
    [SerializeField] TMP_Text EventText;
    [SerializeField] Image EventSprite;
    [SerializeField] Transform ButtonContainer;
    [SerializeField] GameObject ButtonPrefab;
    [SerializeField] ScrollRect eventScrollView;
    //[SerializeField] int currentEventIndex = 0;//���൵üũ

    [Header("�̺�Ʈ")]
    [SerializeField] List<EventData> Events;
    public List<EventData> GetEvents() => Events;

    IEnumerator WaitForPlayer()
    {
        //  Player.Instance�� null�̸� ������ ������ ���
        while (Player.Instance == null)
        {
            Debug.LogWarning("Player �ν��Ͻ��� ���� �������� ����. ��� ��...");
            yield return null; // �� ������ ��� �� �ٽ� Ȯ��
        }

    }


    public void Start()
    {
        StartEvent();
    }

    public void Update()
    {
        Damage();
    }

    private void StartEvent()
    {
        StartCoroutine(WaitForPlayer());

        if (Events != null && Events.Count > 0 && Events[0] != null)
        {
            DisplayEvent(Events[0]);
        }
        else
        {
            Debug.LogWarning("Events ����Ʈ�� ��� �ְų� ù ��° �̺�Ʈ�� null�Դϴ�.");
        }

        //  Player.Instance�� �����ϴ� ��� UI ������Ʈ
        if (Player.Instance != null)
        {
            UpdatePlayerUI();
        }
        else
        {
            Debug.LogError("Player.Instance�� �������� ���� - �⺻���� ������ �� ����.");

        }
    }

    // �̺�Ʈ�� �ؽ�Ʈ, �̹���, ��ư���� ȭ�鿡 ǥ���ϴ� �Լ�
    public void DisplayEvent(EventData evt)
    {
        if (evt == null)
        {
            Debug.LogWarning("�̺�Ʈ�� null�Դϴ�.");
            return;
        }

        EventText.text = evt.EventText;
        if (EventSprite != null && evt.DisplayImg != null)
            EventSprite.sprite = evt.DisplayImg;

        GenerateEventButtons(evt);
    }

    public void GenerateEventButtons(EventData currentEvent) //������ ��ư ����
    {
        // ���� ��ư ���� (�ߺ� ����)
        foreach (Transform child in ButtonContainer)
        {
            Destroy(child.gameObject);
        }

        // �̺�Ʈ ��ư�� ���� ��� �α� ���
        if (currentEvent.Buttons == null || currentEvent.Buttons.Count == 0)
        {
            Debug.LogWarning("�̺�Ʈ�� �������� �����ϴ�.");
            return;
        }

        // ���ο� ��ư ����
        foreach (var choice in currentEvent.Buttons)
        {
            if (!string.IsNullOrEmpty(choice.RequiredTraits))
            {
                bool hasRequiredTrait = Player.Instance.selectedTraits.Any(trait => trait.TraitName == choice.RequiredTraits);
                if (!hasRequiredTrait)
                {
                    Debug.Log($"�÷��̾ '{choice.RequiredTraits}' Ư���� ������ ���� �����Ƿ� '{choice.ChoiceName}' �������� �������� �ʽ��ϴ�.");
                    continue;
                }
            }

            GameObject newButton = Instantiate(ButtonPrefab, ButtonContainer);
            TMP_Text buttonText = newButton.GetComponentInChildren<TMP_Text>();

            if (buttonText != null)
            {
                buttonText.text = choice.ChoiceName; // ��ư�� �ؽ�Ʈ�� ChoiceName���� ����
            }
            else
            {
                Debug.LogError("��ư�� TMP_Text ������Ʈ�� �����ϴ�.");
            }

            // ��ư Ŭ�� �� ������ ��� �߰� ����
            Button buttonComponent = newButton.GetComponent<Button>();
            if (buttonComponent != null)
            {
                buttonComponent.onClick.RemoveAllListeners();
                buttonComponent.onClick.AddListener(() => OnChoiceSelected(choice));
            }
        }
    }

    private void OnChoiceSelected(EventChoice choice)
    {
        Debug.Log($"������ �ɼ�: {choice.ChoiceName}");

        if (choice.NextEventID >= 0 && choice.NextEventID < Events.Count)
        {
            //  ���� �̺�Ʈ�� ������ ��� �� ���� ����
            CurrentEvent = Events[choice.NextEventID];
            DisplayEvent(CurrentEvent);
        }
        else
        {
            //  ���� �̺�Ʈ�� ���� ��� �� ���� �̺�Ʈ ����
            SelectRandomEvent();
        }

        if(eventScrollView != null)
        {
            eventScrollView.verticalNormalizedPosition = 1f;
        }

    }

    private void SelectRandomEvent()
    {
        //  �������� ���� ������ �̺�Ʈ ����Ʈ ����
        List<EventData> availableEvents = Events
            .Where(e => !e.isUsed && (e.Tags & (EventTag.Positive | EventTag.Negative | EventTag.Battle | EventTag.Chaos | EventTag.Encounter)) != 0) //  ������ �ʾ����� �ش� �±װ� �ִ� ��쿡�� ����
            .ToList();

        if (availableEvents.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, availableEvents.Count);
            CurrentEvent = availableEvents[randomIndex];
            CurrentEvent.isUsed = true; //  ���� �̺�Ʈ�� ǥ��
            DisplayEvent(CurrentEvent);
        }
        else
        {
            Debug.Log("�� �̻� ����� �� �ִ� ���� �̺�Ʈ�� �����ϴ�.");
        }
    }

    /*
    public void ProgressCheck()
    {
        ProgressSlider.value = currentProgress;
    }*/

   

    private void UpdatePlayerUI()
    {
        CurrentGold.text = Player.Instance.GetStat("Gold").ToString();
        CurrentFaithPoint.text = Player.Instance.GetStat("FaithPoint").ToString(); // FaithStat�� �ƴ϶� FaithPoint�� ǥ���ϴ� �� ����
        CharacterName.text = Player.Instance.Race.ToString();
        CharacterImg.sprite = Player.Instance.PlayerImg;

    }

    private void UpdateHPUI()
    {
        if (Player.Instance != null)
            Hp.fillAmount = Player.Instance.GetStat("CurrentHP") / Player.Instance.GetStat("HP");
    }

    private void Damage()
    {
        if (Player.Instance == null) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Player.Instance.ChangeStat("CurrentHP", -10);
            UpdateHPUI();
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            float newHP = Mathf.Min(Player.Instance.GetStat("CurrentHP") + 10, Player.Instance.GetStat("HP"));
            Player.Instance.ChangeStat("CurrentHP", newHP - Player.Instance.GetStat("CurrentHP"));
            UpdateHPUI();
        }
    }


    
}
