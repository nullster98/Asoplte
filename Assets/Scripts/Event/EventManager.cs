
using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Event
{
    public class EventManager : MonoBehaviour
    {
        public static EventManager Instance { get; private set; }

        [Header("Main Component")]
        public TMP_Text eventText;
        public Image eventSprite;
        public Transform buttonContainer;
        public GameObject buttonPrefab;
        public ScrollRect eventScrollView;
        public Button eventAreaBtn;

        [Header("Managers")]
        public EventHandler eventHandler;
        public BattleManager battleManger;
        public AcquisitionUI acquisitionUI;

        [Header("Data")]
        public int floor;
        public GameObject currentSpawnedEnemy;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;

                if (DatabaseManager.Instance.eventLines == null || DatabaseManager.Instance.eventLines.Count == 0)
                {
                    Debug.LogError("[EventManager] eventLines가 비어 있습니다. JSON 로드에 실패했을 수 있습니다.");
                    return;
                }

                if (battleManger == null || acquisitionUI == null)
                {
                    Debug.LogError("[EventManager] 필수 매니저(battleManager 또는 acquisitionUI)가 연결되지 않았습니다.");
                    return;
                }

                eventHandler = new EventHandler(battleManger, acquisitionUI);
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

            eventHandler.StartEvent("StartEvent");
        }

        public void UpdateEventUI(List<DialogueBlock> dialogueBlocks)
        {
            eventText.text = "";
            this.eventSprite.sprite = null;

            foreach (Transform child in buttonContainer)
            {
                Destroy(child.gameObject);
            }

            StartCoroutine(PlayDialogueSequence(dialogueBlocks));
        }

        public EventPhase GetCurrentPhase()
        {
            if (eventHandler != null)
                return eventHandler.GetCurrentPhase();
            return null;
        }

        public void UpdateEventUI(string dialogue)
        {
            eventText.text = dialogue;
        }

        public void ShowChoice(List<EventChoice> choices)
        {
            ClearChoiceButtons();
            for (int i = 0; i < choices.Count; i++)
            {
                var choice = choices[i];

                GameObject buttonObj = Instantiate(buttonPrefab, buttonContainer);
                TMP_Text btnText = buttonObj.GetComponentInChildren<TMP_Text>();
                btnText.text = choice.choiceName;

                int choiceIndex = i;
                buttonObj.GetComponent<Button>().onClick.AddListener(() => eventHandler.OnChoiceSelected(choiceIndex));
            }

            if (eventScrollView != null)
                eventScrollView.verticalNormalizedPosition = 1f;
        }

        private void ClearChoiceButtons()
        {
            foreach (Transform child in buttonContainer)
            {
                Destroy(child.gameObject);
            }
        }

        public void PrepareBattleButton(EventOutcome outcome)
        {
            eventAreaBtn.onClick.RemoveAllListeners();
            eventAreaBtn.onClick.AddListener(() =>
            {
                ExecuteBattleFromOutcome(outcome);
            });
        }

        private IEnumerator PlayDialogueSequence(List<DialogueBlock> dialogueBlocks)
        {
            foreach (var block in dialogueBlocks)
            {
                eventText.text = block.dialogueText;

                var phase = GetCurrentPhase();
                if (phase != null && phase.backgroundImage != null)
                {
                    eventSprite.sprite = phase.backgroundImage;
                }

                // ✅ 선택지가 먼저 나오도록 우선 순위 조정
                if (block.choices != null && block.choices.Count > 0)
                {
                    CreateChoiceButtons(block.choices);
                    yield break;
                }

                if (block.outcome != null)
                {
                    if (block.outcome.giveReward)
                    {
                        acquisitionUI.SetupAcquisitionUI(block.outcome.rewardType.Value, block.outcome.rewardID.Value);
                        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
                    }
                    else if (block.outcome.startBattle)
                    {
                        // startBattle == true일 경우 ScrollView 클릭 전투 준비
                        PrepareBattleButton(block.outcome);
                        yield break;
                    }
                }

                yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
            }

            eventHandler.HandleEventEnd();
        }

        private void CreateChoiceButtons(List<EventChoice> choices)
        {
            for (int i = 0; i < choices.Count; i++)
            {
                EventChoice choice = choices[i];
                GameObject newButton = Instantiate(buttonPrefab, buttonContainer);

                TMP_Text buttonText = newButton.GetComponentInChildren<TMP_Text>();
                if (buttonText != null)
                    buttonText.text = choice.choiceName;

                Button btn = newButton.GetComponent<Button>();
                if (btn != null)
                {
                    int index = i;
                    btn.onClick.AddListener(() => eventHandler.OnChoiceSelected(index));
                }
            }
        }

        public void OnChoiceSelected(int choiceIndex)
        {
            eventHandler.OnChoiceSelected(choiceIndex);
        }

        private void ExecuteBattleFromOutcome(EventOutcome outcome)
        {
            if (outcome.entityID == null)
            {
                Debug.LogError("ExecuteBattle: entityID가 null입니다.");
                return;
            }

            var enemyData = DatabaseManager.Instance.entitiesDatabase.GetEnemyByID(outcome.entityID.Value);
            if (enemyData == null)
            {
                Debug.LogError($"ID {outcome.entityID.Value}의 적 데이터를 찾을 수 없습니다.");
                return;
            }

            GameObject enemyObj = EntitySpawner.Spawn(enemyData, floor);
            battleManger.StartBattle(enemyObj);
            eventAreaBtn.onClick.RemoveAllListeners();
        }
    }
}
