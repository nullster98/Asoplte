using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }

    [Header("Main Component")]
    [SerializeField] TMP_Text EventText;
    [SerializeField] Image EventSprite;
    [SerializeField] Transform ButtonContainer;
    [SerializeField] GameObject ButtonPrefab;
    [SerializeField] ScrollRect eventScrollView;

    [Header("Event Data")]
    public EventDatabase eventDatabase;
    private EventHandler eventHandler;

    /*IEnumerator WaitForPlayer()
    {
        //  Player.Instance가 null이면 생성될 때까지 대기
        while (Player.Instance == null)
        {
            Debug.LogWarning("Player 인스턴스가 아직 생성되지 않음. 대기 중...");
            yield return null; // 한 프레임 대기 후 다시 확인
        }

    }*/

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            if (eventDatabase == null)
            {
                Debug.LogError("EventDatabase가 연결되지 않았습니다!");
                return;
            }
            ResetEventDatabase();
            Debug.Log("이벤트 생성 실행");
            EventCreator.GenerateEvents(eventDatabase);

            eventHandler = new EventHandler(eventDatabase); //eventHandler 초기화
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        if (eventHandler == null)
        {
            Debug.LogError("EventHandler가 초기화되지 않았습니다!");
            return;
        }

        eventHandler.StartEvent("시작이벤트");
    }

    public void Update()
    {

    }



    public void UpdateEventUI(string eventDescription, List<EventChoice> choices, Sprite eventSprite)
    {
        EventText.text = eventDescription;
        EventSprite.sprite = eventSprite;

        // 2. 기존 버튼 삭제 (안전한 방식으로 제거)
        List<GameObject> buttonsToDestroy = new List<GameObject>();
        foreach (Transform child in ButtonContainer)
        {
            buttonsToDestroy.Add(child.gameObject);
        }
        foreach (GameObject button in buttonsToDestroy)
        {
            Destroy(button);
        }

        // 3. 새로운 버튼 생성 (안전한 방식으로 인덱스를 전달)
        for (int i = 0; i < choices.Count; i++)
        {
            EventChoice choice = choices[i];

            GameObject newButton = Instantiate(ButtonPrefab, ButtonContainer);
            TMP_Text buttonText = newButton.GetComponentInChildren<TMP_Text>();

            if (buttonText != null)
            {
                buttonText.text = choice.ChoiceName;
            }
            else
            {
                Debug.LogError("ButtonPrefab에 TMP_Text가 존재하지 않습니다!");
            }

            Button buttonComponent = newButton.GetComponent<Button>();

            if (buttonComponent != null)
            {
                int choiceIndex = i; // 인덱스를 안전하게 전달
                buttonComponent.onClick.AddListener(() => eventHandler.OnChoiceSelected(choiceIndex));
            }
            else
            {
                Debug.LogError("ButtonPrefab에 Button 컴포넌트가 존재하지 않습니다!");
            }
        }

        // 4. 스크롤 뷰의 위치를 최상단으로 이동
        if (eventScrollView != null)
        {
            eventScrollView.verticalNormalizedPosition = 1f;
        }
        else
        {
            Debug.LogWarning("eventScrollView가 설정되지 않았습니다!");
        }
    }

    public void OnChoiceSelected(int choiceIndex)
    {
        eventHandler.OnChoiceSelected(choiceIndex); // 플레이어가 선택한 이벤트 진행
    }

    private void ResetEventDatabase()
    {
        if (eventDatabase != null)
        {
            eventDatabase.ResetDatabase();
            Debug.Log("게임 시작 시 EventDatabase 초기화 완료!");
        }
        else
        {
            Debug.LogError("EventDatabase를 찾을 수 없습니다!");
        }
    }


}
