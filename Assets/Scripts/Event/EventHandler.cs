using System.Collections.Generic;
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

        public EventHandler(BattleManager battlemanager, AcquisitionUI acquisitionUI)
        {
            this.battleManager = battlemanager;
            this.acquisitionUI = acquisitionUI;
        }

        public void StartEvent(string eventName)
        {
            if (DatabaseManager.Instance.eventDatabase == null)
            {
                Debug.LogError("이벤트 데이터베이스가 존재하지 않습니다!");
                return;
            }

            currentEvent = DatabaseManager.Instance.eventDatabase.GetEventByName(eventName);

            if (currentEvent == null)
            {
                Debug.LogError($"이벤트 '{eventName}'을 찾을 수 없습니다!");
                return;
            }

            Debug.Log($"이벤트 시작: {eventName}");

            currentPhaseIndex = 0;
            ProcessPhase();
        }

        private void ProcessPhase()
        {
            if (currentEvent == null || currentEvent.phases == null || currentEvent.phases.Count == 0)
            {
                Debug.LogWarning("이벤트에 유효한 페이즈가 없습니다! 이벤트 종료.");
                HandleEventEnd();
                return;
            }

            if (currentPhaseIndex >= currentEvent.phases.Count)
            {
                Debug.Log("이벤트 종료");
                HandleEventEnd();
                return;
            }

            EventPhase phase = currentEvent.phases[currentPhaseIndex];

            Debug.Log($"이벤트 진행 중: {phase.phaseName}");

            if (phase.phaseOutcome != null && phase.phaseOutcome.spawnEntity)
            {
                float chance = phase.phaseOutcome.spawnChance;
                if (Random.value <= chance)
                {
                    var enemyData = DatabaseManager.Instance.entitiesDatabase.GetEnemyByID(phase.phaseOutcome.entityID.Value);
                    if (enemyData != null)
                    {
                        EntitySpawner.Spawn(enemyData);
                    }
                    else
                    {
                        Debug.LogError($"ID {phase.phaseOutcome.entityID}에 해당하는 적 데이터가 없습니다!");
                    }
                }
                else
                {
                    Debug.Log("확률 판정에 따라 적이 등장하지 않았습니다.");
                }
            }
            
            List<EventChoice> availableChoices = new List<EventChoice>();

            foreach (var choice in phase.choices)
            {
                if (choice.CanPlayerSelect(Player.Instance.selectedTraits))
                {
                    availableChoices.Add(choice);
                }
            }

            Debug.Log("UI 업데이트 호출");
            EventManager.Instance.UpdateEventUI(phase.EventDescription, availableChoices, phase.eventImage);

            currentPhaseIndex++;
        }

        public EventData GetRandomEvent()
        {
            if (DatabaseManager.Instance.eventDatabase == null || DatabaseManager.Instance.eventDatabase.events.Count == 0)
            {
                Debug.LogWarning("이벤트 데이터베이스가 비어 있습니다!");
                return null;
            }

            int randomIndex = Random.Range(0, DatabaseManager.Instance.eventDatabase.events.Count);
            return DatabaseManager.Instance.eventDatabase.events[randomIndex]; // 중복 여부 상관없이 무작위 이벤트 선택
        }

        public void OnChoiceSelected(int choiceIndex)
        {
            EventPhase currentPhase = currentEvent.phases[currentPhaseIndex - 1];
            SelectedChoice = currentPhase.choices[choiceIndex];

            if (SelectedChoice.outcome.giveReward)
            {
                Debug.Log("획득 UI 활성화!");
                if (SelectedChoice.outcome.rewardType != null)
                    if (SelectedChoice.outcome.rewardID != null)
                        acquisitionUI.OpenAcquisitionUI(SelectedChoice.outcome.rewardType.Value, 
                            SelectedChoice.outcome.rewardID.Value);
                return;
            }

            if (SelectedChoice.outcome.startBattle)
            {
                Debug.Log("전투 시작!");
                // 전투 시작
                battleManager.StartBattle(SelectedChoice.outcome.entityID);
                return;
            }

            if (SelectedChoice.nextPhaseIndex != -1) //  특정 페이즈로 이동할 경우
            {
                MoveToNextPhase(SelectedChoice.nextPhaseIndex); // 기존 `StartEvent()` 대신 `MoveToNextPhase()` 사용
                return;
            }

            if (SelectedChoice.IsEventEnd())
            {
                HandleEventEnd();
            }
            else
            {
                StartEvent(SelectedChoice.nextEventName);
            }
        }

        private void HandleEventEnd()
        {
            Debug.Log("이벤트가 종료되었습니다. 랜덤 이벤트 실행!");

            EventData randomEvent = GetRandomEvent();
            if (randomEvent != null)
            {
                StartEvent(randomEvent.eventName);
            }
            else
            {
                Debug.Log("사용 가능한 랜덤 이벤트가 없습니다!");
            }
        }

        private void MoveToNextPhase(int nextPhaseIndex)
        {
            if (currentEvent == null)
            {
                Debug.LogError("현재 진행 중인 이벤트가 없습니다!");
                return;
            }

            if (nextPhaseIndex < 0 || nextPhaseIndex >= currentEvent.phases.Count)
            {
                Debug.LogError($"잘못된 페이즈 인덱스: {nextPhaseIndex} (총 페이즈 개수: {currentEvent.phases.Count})");
                return;
            }

            Debug.Log($"페이즈 이동: {currentPhaseIndex} → {nextPhaseIndex}");
            currentPhaseIndex = nextPhaseIndex; // 페이즈 인덱스를 직접 설정
            ProcessPhase(); // 다음 페이즈 실행
        }
    }
}
