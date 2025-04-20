using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Entities;
using Game;
using PlayerScript;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Event
{
    public class EventManager : MonoBehaviour
    {
        public static EventManager Instance { get; private set; }

        [Header("UI Components")] public TMP_Text eventText;
        public Image eventSprite;
        public ScrollRect eventScrollView;
        public Slider currentProgressSlider;
        public int currentProgress = 1;
        public Button eventAreaBtn;
        public Transform buttonContainer;
        public GameObject buttonPrefab;

        [Header("Managers")] public BattleManager battleManager;
        public AcquisitionUI acquisitionUI;
        public EffectResultUI effectResultUI;

        [Header("Current State")] public GameObject currentSpawnedEnemy;
        public int floor;

        private List<EventChoice> currentChoices;
        private Coroutine typingCoroutine;
        private bool isTextPrinting = false;
        private string fullDialogueText = "";
        private bool dialogueEnded = false;

        private bool isWaitingForUI = false;
        private bool pendingHandleEventEnd = false;

        public EventHandler eventHandler;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                eventHandler = new EventHandler(battleManager, acquisitionUI);
            }
            else Destroy(gameObject);
        }

        private void Start()
        {
            eventAreaBtn.onClick.AddListener(OnAreaClicked); // 화면 클릭 이벤트 등록
            eventHandler.StartEvent("E1"); // 첫 이벤트 시작
            currentProgressSlider.value = currentProgress; // 진행도 초기화
        }

        // 대사 및 선택지를 UI에 표시
        public void UpdateEventUI(string dialogue, List<EventChoice> choices = null)
        {
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            fullDialogueText = dialogue;
            currentChoices = choices;
            typingCoroutine = StartCoroutine(TypeText(dialogue, choices));
            ClearChoiceButtons();
        }

        // 타이핑 효과
        private IEnumerator TypeText(string dialogue, List<EventChoice> choices = null)
        {
            isTextPrinting = true;
            dialogueEnded = false;
            eventText.text = "";

            foreach (char c in dialogue)
            {
                eventText.text += c;
                yield return new WaitForSeconds(0.03f);
            }

            isTextPrinting = false;
            dialogueEnded = true;
            yield return null;

            if (choices != null && choices.Count > 0)
                ShowChoice(choices);
        }

        // 선택지 생성 (조건확인)
        public void ShowChoice(List<EventChoice> choices)
        {
            ClearChoiceButtons();
            for (int i = 0; i < choices.Count; i++)
            {
                var choice = choices[i];
                if (choice.condition != null &&
                    !EventConditionEvaluator.IsConditionMet(choice.condition, Player.Instance))
                    continue;

                var button = Instantiate(buttonPrefab, buttonContainer);
                button.GetComponentInChildren<TMP_Text>().text = choice.choiceName;
                int index = i;
                button.GetComponent<Button>().onClick.AddListener(() => eventHandler.OnChoiceSelected(index));
            }
        }

        // 화면 클릭 처리 (텍스트 스킵 하거나 다음으로 넘어가기(선택지가 없을경우에만))
        private void OnAreaClicked()
        {
            if (isTextPrinting)
            {
                if (typingCoroutine != null) StopCoroutine(typingCoroutine);
                eventText.text = fullDialogueText;
                isTextPrinting = false;
                dialogueEnded = true;
                if (currentChoices != null && currentChoices.Count > 0) ShowChoice(currentChoices);
                return;
            }

            if (dialogueEnded && buttonContainer.childCount == 0)
            {
                var dialogue = eventHandler.GetCurrentDialogue();
                if (dialogue?.outcome != null)
                {
                    if (dialogue.outcome.battleTrigger)
                    {
                        NotifyUIOpened();
                        battleManager.StartBattle(currentSpawnedEnemy);
                        return;
                    }

                    if (dialogue.outcome.rewardTrigger && dialogue.outcome.rewardType.HasValue)
                    {
                        NotifyUIOpened();
                        acquisitionUI.SetupAcquisitionUI(dialogue.outcome.rewardType.Value, dialogue.outcome.rewardID);
                        return;
                    }
                }

                eventHandler.ProcessNextDialogue();
            }
        }

        public void NotifyUIOpened() => isWaitingForUI = true; // 외부 UI 열림 알림

        public void NotifyUIClosed() // 외부 UI 닫힘 알림
        {
            isWaitingForUI = false;
            if (pendingHandleEventEnd)
            {
                pendingHandleEventEnd = false;
                eventHandler.HandleEventEnd();
            }
        }

        public void RequestHandleEventEnd() //이벤트 종료 요청
        {
            if (isWaitingForUI) pendingHandleEventEnd = true;
            else eventHandler.HandleEventEnd();
        }

        private void ClearChoiceButtons() //선택지 버튼 제거
        {
            foreach (Transform child in buttonContainer)
                Destroy(child.gameObject);
        }
    }
}