using System;
using System.Collections.Generic;
using System.Linq;
using Entities;
using Game;
using PlayerScript;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Event
{
    public class EventHandler
    {
        // 상태
        public EventData currentEvent;
        private EventPhase currentPhase;
        private int currentPhaseIndex;
        public int dialogueIndex;
        public List<DialogueBlock> currentDialogues;
        private readonly HashSet<string> usedEvents = new();

        // 의존성
        private readonly BattleManager battleManager;
        private readonly AcquisitionUI acquisitionUI;

        public EventHandler(BattleManager battleManager, AcquisitionUI acquisitionUI)
        {
            this.battleManager = battleManager;
            this.acquisitionUI = acquisitionUI;
        }

        // 이벤트 시작 및 설정
        public void StartEvent(string eventID, int phaseIndex = 0, int dialogueIndex = 0)
        {
            SetCurrentEventAndPhase(eventID, phaseIndex, dialogueIndex);
            ProcessPhase();
        }

        private void SetCurrentEventAndPhase(string eventID, int phaseIndex, int dialogueIndex)
        {
            currentEvent = DatabaseManager.Instance.eventLines.Find(e => e.eventID == eventID);

            if (currentEvent == null)
            {
                Debug.LogError($"Could not find event with ID {eventID}");
                return;
            }

            if (phaseIndex < 0 || phaseIndex >= currentEvent.phases.Count)
            {
                Debug.LogError($"Could not find phase index {phaseIndex}");
                return;
            }

            currentPhaseIndex = phaseIndex;
            currentPhase = currentEvent.phases[phaseIndex];
            currentDialogues = currentPhase.dialogues;

            this.dialogueIndex = (dialogueIndex >= 0 && dialogueIndex < currentDialogues.Count) ? dialogueIndex : 0;
        }

        // 📌 Phase/Dialogue 실행
        private void ProcessPhase()
        {
            if (currentPhase == null)
            {
                Debug.LogError("No phase selected");
                HandleEventEnd();
                return;
            }

            if (currentPhase.phaseOutcome != null)
                PhaseOutcome(currentPhase.phaseOutcome);

            EventManager.Instance.eventSprite.sprite = currentPhase.phaseImage;
            ProcessDialogue();
        }

        private void ProcessDialogue()
        {
            while (dialogueIndex < currentDialogues.Count)
            {
                var dialogue = currentDialogues[dialogueIndex++];

                if (dialogue.condition != null &&
                    !EventConditionEvaluator.IsConditionMet(dialogue.condition, Player.Instance))
                    continue;

                if (dialogue.outcome != null && dialogue.outcome.HasEffect())
                    DialogueOutcome(dialogue.outcome);

                EventManager.Instance.UpdateEventUI(dialogue.dialogueText, dialogue.choices);
                return;
            }

            // 모든 대사 끝난 경우 처리
            var last = currentDialogues.Last();

            if (!string.IsNullOrEmpty(last.nextEventID))
            {
                StartEvent(last.nextEventID);
                return;
            }

            if (!string.IsNullOrEmpty(last.nextPhaseID))
            {
                MoveToNextPhase(last.nextPhaseID);
                return;
            }

            if (last.choices?.Count > 0)
                EventManager.Instance.ShowChoice(last.choices);
            else
                TryFallbackPhaseOrEnd();
        }

        public void ProcessNextDialogue() => ProcessDialogue();
        public DialogueBlock GetCurrentDialogue() => (dialogueIndex - 1 >= 0 && dialogueIndex - 1 < currentDialogues.Count) ? currentDialogues[dialogueIndex - 1] : null;

        // 📌 선택지 처리
        public void OnChoiceSelected(int choiceIndex)
        {
            Debug.Log($"[EventHandler] 선택지 클릭됨 - 인덱스: {choiceIndex}, 현재 대사 인덱스: {dialogueIndex}");
            var lastDialogue = currentDialogues[Mathf.Clamp(dialogueIndex - 1, 0, currentDialogues.Count - 1)];
            if (lastDialogue.choices == null || choiceIndex >= lastDialogue.choices.Count)
            {
                Debug.LogWarning("[EventHandler] 선택지 인덱스가 유효하지 않음");
                return;
            }

            var choice = lastDialogue.choices[choiceIndex];

            if (choice.outcome != null && choice.outcome.HasEffect())
            {
                ChoiceOutcome(choice.outcome);
                return;
            }

            HandleChoiceFlow(choice);
        }

        private void HandleChoiceFlow(EventChoice choice)
        {
            if (!string.IsNullOrEmpty(choice.nextDialogueID))
            {
                int targetIndex = currentDialogues.FindIndex(d => d.dialogueID == choice.nextDialogueID);
                if (targetIndex != -1)
                {
                    dialogueIndex = targetIndex;
                    ProcessDialogue();
                    return;
                }
                else Debug.LogWarning($"[EventHandler] nextDialogueID '{choice.nextDialogueID}' 없음");
            }

            if (!string.IsNullOrEmpty(choice.nextPhaseID))
            {
                MoveToNextPhase(choice.nextPhaseID);
                return;
            }

            if (!string.IsNullOrEmpty(choice.nextEventID))
            {
                StartEvent(choice.nextEventID);
                return;
            }

            HandleEventEnd();
        }

        // 📌 Outcome 처리
        private void DialogueOutcome(EventOutcome outcome)
        {
            if (outcome.rewardTrigger && outcome.rewardType.HasValue)
            {
                acquisitionUI.SetupAcquisitionUI(outcome.rewardType.Value, outcome.rewardID);
                return;
            }

            if (outcome.stateTrigger)
            {
                outcome.modifyStat?.ForEach(mod => Player.Instance.ChangeStat(mod.stat, mod.amount));
                outcome.addTrait?.ForEach(Player.Instance.AddTrait);
                outcome.removeTrait?.ForEach(Player.Instance.RemoveTrait);
            }
        }

        private void ChoiceOutcome(EventOutcome outcome)
        {
            if (outcome.battleTrigger)
            {
                var enemy = EventManager.Instance.currentSpawnedEnemy;
                if (enemy == null)
                {
                    Debug.LogError("[ChoiceOutcome] currentSpawnedEnemy가 null임");
                    return;
                }
                EventManager.Instance.battleManager.StartBattle(enemy);
                return;
            }

            if (outcome.openShop)
            {
                var entity = EventManager.Instance.currentSpawnedEnemy?.GetComponent<EntityObject>();
                entity?.shopPanel?.SetActive(true);
                return;
            }

            if (outcome.rewardTrigger && outcome.rewardType.HasValue)
            {
                acquisitionUI.SetupAcquisitionUI(outcome.rewardType.Value, outcome.rewardID);
                return;
            }

            if (outcome.stateTrigger)
            {
                outcome.modifyStat?.ForEach(mod => Player.Instance.ChangeStat(mod.stat, mod.amount));
                outcome.addTrait?.ForEach(Player.Instance.AddTrait);
                outcome.removeTrait?.ForEach(Player.Instance.RemoveTrait);
                return;
            }

            // if (Random.value <= outcome.spawnChance)
            // {
            //     var enemy = EntitySpawner.Spawn(
            //         DatabaseManager.Instance.GetEntitiesData(outcome.entityID),
            //         EventManager.Instance.floor);
            //     EventManager.Instance.currentSpawnedEnemy = enemy;
            // }
        }

        private void PhaseOutcome(EventOutcome outcome)
        {
            if (!outcome.spawnEntity || string.IsNullOrEmpty(outcome.entityID)) return;

            if (Random.value <= outcome.spawnChance)
            {
                var enemy = EntitySpawner.Spawn(
                    DatabaseManager.Instance.GetEntitiesData(outcome.entityID),
                    EventManager.Instance.floor);
                EventManager.Instance.currentSpawnedEnemy = enemy;
            }
        }

        // 📌 이벤트 흐름 종료 및 전환
        public void HandleEventEnd()
        {
            if (!usedEvents.Contains(currentEvent.eventID))
                usedEvents.Add(currentEvent.eventID);

            Debug.Log("[EventHandler] 이벤트가 종료됨. 랜덤 이벤트로 이동");

            if (EventManager.Instance.currentSpawnedEnemy != null)
            {
                GameObject.Destroy(EventManager.Instance.currentSpawnedEnemy);
                EventManager.Instance.currentSpawnedEnemy = null;
            }

            var usableEvents = DatabaseManager.Instance.eventLines
                .Where(e => (e.isRecycle || !usedEvents.Contains(e.eventID)) && e.eventType != EventTag.None)
                .ToList();

            if (usableEvents.Count == 0)
            {
                Debug.LogWarning("사용 가능한 이벤트가 없습니다");
                return;
            }

            var next = usableEvents[Random.Range(0, usableEvents.Count)];
            StartEvent(next.eventID);
        }

        private void MoveToNextPhase(string nextPhaseID)
        {
            var next = currentEvent.phases.Find(p => p.phaseID == nextPhaseID);
            if (next != null)
            {
                currentPhaseIndex = currentEvent.phases.IndexOf(next);
                currentPhase = next;
                currentDialogues = next.dialogues;
                dialogueIndex = 0;
                ProcessPhase();
            }
            else
            {
                EventManager.Instance.RequestHandleEventEnd();
            }
        }

        private void TryFallbackPhaseOrEnd()
        {
            if (currentPhaseIndex + 1 < currentEvent.phases.Count)
            {
                currentPhaseIndex++;
                ProcessPhase();
            }
            else
            {
                EventManager.Instance.RequestHandleEventEnd();
            }
        }
    }
}