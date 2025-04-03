using System;
using System.Collections.Generic;
using System.Linq;
using Entities;
using Game;
using PlayerScript;
using UI;
using UnityEngine;

namespace Event
{
    public class EventHandler
    {
        private EventChoice SelectedChoice { get; set; }
        private EventData currentEvent;
        private int currentPhaseIndex;

        private BattleManager battleManager;
        private AcquisitionUI acquisitionUI;

        private List<DialogueBlock> currentDialogues;
        private int dialogueIndex;

        public EventHandler(BattleManager battlemanager, AcquisitionUI acquisitionUI)
        {
            this.battleManager = battlemanager;
            this.acquisitionUI = acquisitionUI;
        }

        public void StartEvent(string eventName)
        {
            List<FlatEventLine> allLines = DatabaseManager.Instance.eventLines;

            if (allLines == null || allLines.Count == 0)
            {
                Debug.LogError("[EventHandler] 이벤트 데이터가 로드되지 않았습니다.");
                return;
            }

            var linesForEvent = allLines.FindAll(e => e.eventName == eventName);
            if (linesForEvent == null || linesForEvent.Count == 0)
            {
                Debug.LogError($"[EventHandler] 이벤트 '{eventName}'를 찾을 수 없습니다.");
                return;
            }

            Debug.Log($"[EventHandler] 이벤트 시작: {eventName}");

            var groupedByPhase = linesForEvent
                .GroupBy(l => l.phaseName)
                .ToDictionary(g => g.Key, g => g.ToList());

            var sortedPhases = groupedByPhase.OrderBy(kvp => kvp.Key).ToList();
            List<EventPhase> eventPhases = new List<EventPhase>();

            foreach (var (phaseName, phaseLines) in sortedPhases)
            {
                string bgImageName = phaseLines.FirstOrDefault(l => !string.IsNullOrEmpty(l.backgroundImage))?.backgroundImage;
                if (string.IsNullOrEmpty(bgImageName) && eventPhases.Count > 0)
                    bgImageName = eventPhases.Last().backgroundImageName;

                EventPhase phase = new EventPhase
                {
                    phaseName = phaseName,
                    backgroundImageName = bgImageName,
                    backgroundImage = LoadImage(bgImageName),
                    dialouges = new List<DialogueBlock>(),
                    phaseOutcome = ParsePhaseOutcome(phaseLines.First())
                };

                DialogueBlock currentBlock = null;

                foreach (var line in phaseLines)
                {
                    if (string.IsNullOrEmpty(line.dialogueText) && string.IsNullOrEmpty(line.choiceName))
                        continue;

                    if (!string.IsNullOrEmpty(line.dialogueText))
                    {
                        currentBlock = new DialogueBlock
                        {
                            dialogueText = line.dialogueText,
                            outcome = ParseDialogueOutcome(line),
                            choices = new List<EventChoice>()
                        };
                        phase.dialouges.Add(currentBlock);
                    }

                    if (!string.IsNullOrEmpty(line.choiceName))
                    {
                        if (currentBlock == null && phase.dialouges.Count > 0)
                            currentBlock = phase.dialouges.Last();

                        if (currentBlock != null)
                        {
                            currentBlock.choices.Add(new EventChoice
                            {
                                choiceName = line.choiceName,
                                requiredTraits = line.requiredTraits,
                                nextEventName = line.nextEventName,
                                nextPhaseIndex = line.nextPhaseIndex,
                                outcome = ParseChoiceOutcome(line)
                            });
                        }
                    }
                }

                eventPhases.Add(phase);
            }

            string tagString = linesForEvent.FirstOrDefault()?.eventType;
            EventTag tag = Enum.TryParse(tagString, out EventTag parsed) ? parsed : EventTag.None;
            bool.TryParse(linesForEvent[0].isRecycle, out bool isRecycleFlag);

            currentEvent = new EventData
            {
                eventName = eventName,
                eventType = tag,
                phases = eventPhases,
                isRecycle = isRecycleFlag,
                isUsed = false
            };

            currentPhaseIndex = 0;
            ProcessPhase();
        }

        private void ProcessPhase()
        {
            if (currentEvent == null || currentEvent.phases == null || currentEvent.phases.Count == 0)
            {
                Debug.LogWarning("[EventHandler] 유효한 페이즈가 없습니다. 이벤트 종료.");
                HandleEventEnd();
                return;
            }

            if (currentPhaseIndex >= currentEvent.phases.Count)
            {
                Debug.Log("[EventHandler] 이벤트 종료");
                HandleEventEnd();
                return;
            }

            EventPhase phase = currentEvent.phases[currentPhaseIndex];
            Debug.Log($"[EventHandler] 페이즈: {phase.phaseName}");

            // ✅ 전투 태그 + 적 스폰
            if (currentEvent.eventType == EventTag.Battle && phase.phaseOutcome?.spawnEntity == true)
            {
                var enemyData = DatabaseManager.Instance.entitiesDatabase.GetEnemyByID(phase.phaseOutcome.entityID ?? -1);
                if (enemyData != null)
                {
                    var enemy = EntitySpawner.Spawn(enemyData, EventManager.Instance.floor);
                    EventManager.Instance.currentSpawnedEnemy = enemy;

                    if (phase.phaseOutcome.startBattle)
                        EventManager.Instance.PrepareBattleButton(phase.phaseOutcome);
                }
            }

            if (phase.backgroundImage != null)
                EventManager.Instance.eventSprite.sprite = phase.backgroundImage;

            currentDialogues = phase.dialouges;
            dialogueIndex = 0;

            if (currentDialogues == null || currentDialogues.Count == 0)
            {
                MoveToNextPhaseOrEnd();
                return;
            }

            EventManager.Instance.UpdateEventUI(currentDialogues);
            ShowNextDialogue();
        }

        public void ShowNextDialogue()
        {
            if (dialogueIndex >= currentDialogues.Count)
            {
                if (currentDialogues.Last().choices?.Count > 0)
                    EventManager.Instance.ShowChoice(currentDialogues.Last().choices);
                else
                    MoveToNextPhaseOrEnd();

                return;
            }

            var dialogue = currentDialogues[dialogueIndex];
            EventManager.Instance.UpdateEventUI(dialogue.dialogueText);
            dialogueIndex++;
        }

        public void OnChoiceSelected(int choiceIndex)
        {
            EventPhase currentPhase = currentEvent.phases[currentPhaseIndex];
            DialogueBlock lastBlockWithChoices = currentPhase.dialouges
                .LastOrDefault(db => db.choices != null && db.choices.Count > 0);

            if (lastBlockWithChoices == null)
            {
                Debug.LogError("[EventHandler] 선택지를 가진 DialogueBlock이 없음.");
                return;
            }

            SelectedChoice = lastBlockWithChoices.choices[choiceIndex];

            if (SelectedChoice.outcome != null)
            {
                if (SelectedChoice.outcome.giveReward && SelectedChoice.outcome.rewardType != null)
                {
                    acquisitionUI.SetupAcquisitionUI(SelectedChoice.outcome.rewardType.Value, SelectedChoice.outcome.rewardID.Value);
                    return;
                }

                if (SelectedChoice.outcome.startBattle)
                {
                    battleManager.StartBattle(EventManager.Instance.currentSpawnedEnemy);
                    return;
                }
            }

            if (!string.IsNullOrEmpty(SelectedChoice.nextEventName))
            {
                StartEvent(SelectedChoice.nextEventName);
                return;
            }

            if (SelectedChoice.nextPhaseIndex != -1)
            {
                MoveToNextPhase(SelectedChoice.nextPhaseIndex);
                return;
            }

            if (SelectedChoice.IsEventEnd())
            {
                HandleEventEnd();
            }
        }

        private void MoveToNextPhase(int nextPhaseIndex)
        {
            if (nextPhaseIndex < 0 || nextPhaseIndex >= currentEvent.phases.Count)
            {
                Debug.LogError($"[EventHandler] 잘못된 페이즈 인덱스: {nextPhaseIndex}");
                return;
            }

            currentPhaseIndex = nextPhaseIndex;
            ProcessPhase();
        }

        private void MoveToNextPhaseOrEnd()
        {
            if (currentPhaseIndex + 1 < currentEvent.phases.Count)
            {
                currentPhaseIndex++;
                ProcessPhase();
            }
            else
            {
                HandleEventEnd();
            }
        }

        public void HandleEventEnd()
        {
            Debug.Log("[EventHandler] 이벤트가 종료되었습니다. 재사용 여부를 기반으로 랜덤 이벤트 실행!");

            var groupedEvents = DatabaseManager.Instance.eventLines
                .GroupBy(l => l.eventName);

            var usableEvents = groupedEvents
                .Where(group =>
                {
                    var line = group.First();
                    bool.TryParse(line.isRecycle, out bool isRecycle);
                    bool isUsed = EventUsedTracker.Instance.IsUsed(line.eventName);
                    return isRecycle || !isUsed;
                })
                .Select(group => group.Key)
                .ToList();

            if (usableEvents.Count == 0)
            {
                Debug.LogWarning("사용 가능한 이벤트가 없습니다.");
                return;
            }

            string randomEventName = usableEvents[UnityEngine.Random.Range(0, usableEvents.Count)];

            // 사용 처리
            EventUsedTracker.Instance.MarkAsUsed(randomEventName);
            StartEvent(randomEventName);
        }
        
        public class EventUsedTracker
        {
            private static HashSet<string> usedEvents = new();

            public static EventUsedTracker Instance { get; } = new EventUsedTracker();

            public bool IsUsed(string eventName) => usedEvents.Contains(eventName);

            public void MarkAsUsed(string eventName) => usedEvents.Add(eventName);

            public void Reset() => usedEvents.Clear();
        }

        private Sprite LoadImage(string imageName)
        {
            if (string.IsNullOrEmpty(imageName)) return null;
            return Resources.Load<Sprite>($"Event/Images/{imageName}");
        }

        private EventOutcome ParsePhaseOutcome(FlatEventLine line)
        {
            if (!line.spawnEntity && line.entityID == 0)
                return null;

            return new EventOutcome
            {
                spawnEntity = line.spawnEntity,
                entityID = line.entityID != 0 ? line.entityID : (int?)null,
                startBattle = line.startBattle
            };
        }

        private EventOutcome ParseDialogueOutcome(FlatEventLine line)
        {
            var outcome = new EventOutcome
            {
                startBattle = line.startBattle,
                giveReward = line.giveReward,
                rewardID = line.rewardID != 0 ? line.rewardID : (int?)null,
                rewardType = string.IsNullOrEmpty(line.rewardType) ? null : EnumParser.Parse<AcquisitionType>(line.rewardType),
                modifyStat = new List<StatModifier>(),
                addTrait = new List<string>(),
                removeTrait = new List<string>()
            };

            void TryAddStat(string name, int amount)
            {
                if (!string.IsNullOrEmpty(name))
                    outcome.modifyStat.Add(new StatModifier { stat = name, amount = amount });
            }

            TryAddStat(line.stat_1_name, line.stat_1_amount);
            TryAddStat(line.stat_2_name, line.stat_2_amount);
            TryAddStat(line.stat_3_name, line.stat_3_amount);

            if (!string.IsNullOrEmpty(line.addTrait_1)) outcome.addTrait.Add(line.addTrait_1);
            if (!string.IsNullOrEmpty(line.addTrait_2)) outcome.addTrait.Add(line.addTrait_2);
            if (!string.IsNullOrEmpty(line.addTrait_3)) outcome.addTrait.Add(line.addTrait_3);
            if (!string.IsNullOrEmpty(line.removeTrait_1)) outcome.removeTrait.Add(line.removeTrait_1);
            if (!string.IsNullOrEmpty(line.removeTrait_2)) outcome.removeTrait.Add(line.removeTrait_2);
            if (!string.IsNullOrEmpty(line.removeTrait_3)) outcome.removeTrait.Add(line.removeTrait_3);

            outcome.affectPlayerState = outcome.modifyStat.Count > 0 || outcome.addTrait.Count > 0 || outcome.removeTrait.Count > 0;

            return outcome;
        }

        private EventOutcome ParseChoiceOutcome(FlatEventLine line)
        {
            return ParseDialogueOutcome(line);
        }

        public EventPhase GetCurrentPhase()
        {
            if (currentEvent != null && currentPhaseIndex < currentEvent.phases.Count)
                return currentEvent.phases[currentPhaseIndex];
            return null;
        }
    }
}
