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
public enum EventTag //이벤트 태그들
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

    Sequential = 1 << 8 // 이벤트 후속

}

[System.Serializable]
public class EventChoice
{
    public string ChoiceName; //선택지 이름
    public string RequiredTraits; //필요 특성
    [SerializeField] private int nextEventID = -1; // 이벤트 ID 저장
    
    [NonSerialized] public EventData NextEventRef; // 이동할 이벤트 참조변수

    public int NextEventID
    {
        get => nextEventID;
        set => nextEventID = value;
    }
}

[System.Serializable]
public class EventData //이벤트 기본 뼈대
{
    public EventTag Tags; //이벤트 태그
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
    [SerializeField] Image Hp; //HP바
    [SerializeField] Image MP; //Mp바
    [SerializeField] Slider ProgressSlider; //슬라이더
    [SerializeField] int currentProgress = 0; //진행도
    [SerializeField] TMP_Text CurrentGold;
    [SerializeField] TMP_Text CurrentFaithPoint;
    [SerializeField] TMP_Text CharacterName;
    [SerializeField] Image CharacterImg;
    [SerializeField] GameObject PlayerInfoBox;
    [SerializeField] Transform TraitContainer;
    [SerializeField] GameObject TraitPrefeb; //특성이미지 프리팹
    [SerializeField] GameObject TraitTextBox;
    [SerializeField] TMP_Text TraitText; //특성 설
    private EventData CurrentEvent;

    [Header("Main Component")]
    [SerializeField] TMP_Text EventText;
    [SerializeField] Image EventSprite;
    [SerializeField] Transform ButtonContainer;
    [SerializeField] GameObject ButtonPrefab;
    [SerializeField] ScrollRect eventScrollView;
    //[SerializeField] int currentEventIndex = 0;//진행도체크

    [Header("이벤트")]
    [SerializeField] List<EventData> Events;
    public List<EventData> GetEvents() => Events;

    IEnumerator WaitForPlayer()
    {
        //  Player.Instance가 null이면 생성될 때까지 대기
        while (Player.Instance == null)
        {
            Debug.LogWarning("Player 인스턴스가 아직 생성되지 않음. 대기 중...");
            yield return null; // 한 프레임 대기 후 다시 확인
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
            Debug.LogWarning("Events 리스트가 비어 있거나 첫 번째 이벤트가 null입니다.");
        }

        //  Player.Instance가 존재하는 경우 UI 업데이트
        if (Player.Instance != null)
        {
            UpdatePlayerUI();
        }
        else
        {
            Debug.LogError("Player.Instance가 존재하지 않음 - 기본값을 적용할 수 없음.");

        }
    }

    // 이벤트의 텍스트, 이미지, 버튼들을 화면에 표시하는 함수
    public void DisplayEvent(EventData evt)
    {
        if (evt == null)
        {
            Debug.LogWarning("이벤트가 null입니다.");
            return;
        }

        EventText.text = evt.EventText;
        if (EventSprite != null && evt.DisplayImg != null)
            EventSprite.sprite = evt.DisplayImg;

        GenerateEventButtons(evt);
    }

    public void GenerateEventButtons(EventData currentEvent) //선택지 버튼 생성
    {
        // 기존 버튼 삭제 (중복 방지)
        foreach (Transform child in ButtonContainer)
        {
            Destroy(child.gameObject);
        }

        // 이벤트 버튼이 없는 경우 로그 출력
        if (currentEvent.Buttons == null || currentEvent.Buttons.Count == 0)
        {
            Debug.LogWarning("이벤트에 선택지가 없습니다.");
            return;
        }

        // 새로운 버튼 생성
        foreach (var choice in currentEvent.Buttons)
        {
            if (!string.IsNullOrEmpty(choice.RequiredTraits))
            {
                bool hasRequiredTrait = Player.Instance.selectedTraits.Any(trait => trait.TraitName == choice.RequiredTraits);
                if (!hasRequiredTrait)
                {
                    Debug.Log($"플레이어가 '{choice.RequiredTraits}' 특성을 가지고 있지 않으므로 '{choice.ChoiceName}' 선택지를 생성하지 않습니다.");
                    continue;
                }
            }

            GameObject newButton = Instantiate(ButtonPrefab, ButtonContainer);
            TMP_Text buttonText = newButton.GetComponentInChildren<TMP_Text>();

            if (buttonText != null)
            {
                buttonText.text = choice.ChoiceName; // 버튼의 텍스트를 ChoiceName으로 설정
            }
            else
            {
                Debug.LogError("버튼에 TMP_Text 컴포넌트가 없습니다.");
            }

            // 버튼 클릭 시 동작할 기능 추가 가능
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
        Debug.Log($"선택한 옵션: {choice.ChoiceName}");

        if (choice.NextEventID >= 0 && choice.NextEventID < Events.Count)
        {
            //  다음 이벤트가 설정된 경우 → 직접 연결
            CurrentEvent = Events[choice.NextEventID];
            DisplayEvent(CurrentEvent);
        }
        else
        {
            //  다음 이벤트가 없는 경우 → 랜덤 이벤트 실행
            SelectRandomEvent();
        }

        if(eventScrollView != null)
        {
            eventScrollView.verticalNormalizedPosition = 1f;
        }

    }

    private void SelectRandomEvent()
    {
        //  랜덤으로 선택 가능한 이벤트 리스트 생성
        List<EventData> availableEvents = Events
            .Where(e => !e.isUsed && (e.Tags & (EventTag.Positive | EventTag.Negative | EventTag.Battle | EventTag.Chaos | EventTag.Encounter)) != 0) //  사용되지 않았으며 해당 태그가 있는 경우에만 선택
            .ToList();

        if (availableEvents.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, availableEvents.Count);
            CurrentEvent = availableEvents[randomIndex];
            CurrentEvent.isUsed = true; //  사용된 이벤트로 표시
            DisplayEvent(CurrentEvent);
        }
        else
        {
            Debug.Log("더 이상 사용할 수 있는 랜덤 이벤트가 없습니다.");
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
        CurrentFaithPoint.text = Player.Instance.GetStat("FaithPoint").ToString(); // FaithStat이 아니라 FaithPoint를 표시하는 게 맞음
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
