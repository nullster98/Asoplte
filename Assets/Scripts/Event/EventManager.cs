
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

    [Header("UI Components")]
    public TMP_Text eventText;
    public Image eventSprite;
    public ScrollRect eventScrollView;
    public Button eventAreaBtn;
    public Transform buttonContainer;
    public GameObject buttonPrefab;

    [Header("Managers")]
    public BattleManager battleManager;
    public AcquisitionUI acquisitionUI;

    [Header("Current State")]
    public GameObject currentSpawnedEnemy;
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
        eventAreaBtn.onClick.AddListener(OnAreaClicked);
        eventHandler.StartEvent("E1");
    }

    public void UpdateEventUI(string dialogue, List<EventChoice> choices = null)
        {
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            fullDialogueText = dialogue;
            currentChoices = choices;
            typingCoroutine = StartCoroutine(TypeText(dialogue, choices));
            ClearChoiceButtons();
        }

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

        public void ShowChoice(List<EventChoice> choices)
        {
            ClearChoiceButtons();
            for (int i = 0; i < choices.Count; i++)
            {
                var choice = choices[i];
                if (choice.condition != null && !EventConditionEvaluator.IsConditionMet(choice.condition, Player.Instance))
                    continue;

                var button = Instantiate(buttonPrefab, buttonContainer);
                button.GetComponentInChildren<TMP_Text>().text = choice.choiceName;
                int index = i;
                button.GetComponent<Button>().onClick.AddListener(() => eventHandler.OnChoiceSelected(index));
            }
        }

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

        public void NotifyUIOpened() => isWaitingForUI = true;
        public void NotifyUIClosed()
        {
            isWaitingForUI = false;
            if (pendingHandleEventEnd)
            {
                pendingHandleEventEnd = false;
                eventHandler.HandleEventEnd();
            }
        }

        public void RequestHandleEventEnd()
        {
            if (isWaitingForUI) pendingHandleEventEnd = true;
            else eventHandler.HandleEventEnd();
        }

        private void ClearChoiceButtons()
        {
            foreach (Transform child in buttonContainer)
                Destroy(child.gameObject);
        }
    }
}


