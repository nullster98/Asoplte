using System.Collections.Generic;
using Game;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Event
{
    public class EventManager : MonoBehaviour
    {
        public static EventManager Instance { get; private set; }

        public int Floor { get; private set; }

        
        [Header("Main Component")]
        [FormerlySerializedAs("EventText")] [SerializeField] private TMP_Text eventText;
        [FormerlySerializedAs("EventSprite")] [SerializeField] private Image eventSprite;
        [FormerlySerializedAs("ButtonContainer")] [SerializeField] private Transform buttonContainer;
        [FormerlySerializedAs("ButtonPrefab")] [SerializeField] private GameObject buttonPrefab;
        [SerializeField] private ScrollRect eventScrollView;

        [Header("Event Data")]
        private EventHandler eventHandler;
        public BattleManager battleManger;
        public AcquisitionUI acquisitionUI;

        public EventManager(int floor)
        {
            Floor = floor;
        }

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

                if (DatabaseManager.Instance.eventDatabase == null)
                {
                    Debug.LogError("EventDatabase가 연결되지 않았습니다!");
                    return;
                }
                ResetEventDatabase();
                Debug.Log("이벤트 생성 실행");
                EventCreator.GenerateEvents();

                eventHandler = new EventHandler(battleManger, acquisitionUI); //eventHandler 초기화
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
        
        public void UpdateEventUI(string eventDescription, List<EventChoice> choices, Sprite eventUISprite)
        {
            eventText.text = eventDescription;
            this.eventSprite.sprite = eventUISprite;

            // 2. 기존 버튼 삭제 (안전한 방식으로 제거)
            List<GameObject> buttonsToDestroy = new List<GameObject>();
            foreach (Transform child in buttonContainer)
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

                GameObject newButton = Instantiate(buttonPrefab, buttonContainer);
                TMP_Text buttonText = newButton.GetComponentInChildren<TMP_Text>();

                if (buttonText != null)
                {
                    buttonText.text = choice.choiceName;
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
            if (DatabaseManager.Instance.eventDatabase != null)
            {
                DatabaseManager.Instance.eventDatabase.ResetDatabase();
                Debug.Log("게임 시작 시 EventDatabase 초기화 완료!");
            }
            else
            {
                Debug.LogError("EventDatabase를 찾을 수 없습니다!");
            }
        }

    }
}
