using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHandler
{

    public EventChoice SelectedChoice { get; private set; }
    private EventData currentEvent;
    private int currentPhaseIndex = 0;

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
            Debug.LogError("�̺�Ʈ �����ͺ��̽��� �������� �ʽ��ϴ�!");
            return;
        }

        currentEvent = DatabaseManager.Instance.eventDatabase.GetEventByName(eventName);

        if (currentEvent == null)
        {
            Debug.LogError($"�̺�Ʈ '{eventName}'�� ã�� �� �����ϴ�!");
            return;
        }

        Debug.Log($"�̺�Ʈ ����: {eventName}");

        currentPhaseIndex = 0;
        ProcessPhase();
    }

    private void ProcessPhase()
    {
        if (currentEvent == null || currentEvent.Phases == null || currentEvent.Phases.Count == 0)
        {
            Debug.LogWarning("�̺�Ʈ�� ��ȿ�� ����� �����ϴ�! �̺�Ʈ ����.");
            HandleEventEnd();
            return;
        }

        if (currentPhaseIndex >= currentEvent.Phases.Count)
        {
            Debug.Log("�̺�Ʈ ����");
            HandleEventEnd();
            return;
        }

        EventPhase phase = currentEvent.Phases[currentPhaseIndex];

        Debug.Log($"�̺�Ʈ ���� ��: {phase.PhaseName}");

        List<EventChoice> availableChoices = new List<EventChoice>();

        foreach (var choice in phase.Choices)
        {
            if (choice.CanPlayerSelect(Player.Instance.selectedTraits))
            {
                availableChoices.Add(choice);
            }
        }

        Debug.Log("UI ������Ʈ ȣ��");
        EventManager.Instance.UpdateEventUI(phase.EventDescription, availableChoices, phase.EventImage);

        currentPhaseIndex++;
    }

    public EventData GetRandomEvent()
    {
        if (DatabaseManager.Instance.eventDatabase == null || DatabaseManager.Instance.eventDatabase.events.Count == 0)
        {
            Debug.LogWarning("�̺�Ʈ �����ͺ��̽��� ��� �ֽ��ϴ�!");
            return null;
        }

        int randomIndex = Random.Range(0, DatabaseManager.Instance.eventDatabase.events.Count);
        return DatabaseManager.Instance.eventDatabase.events[randomIndex]; // �ߺ� ���� ������� ������ �̺�Ʈ ����
    }

    public void OnChoiceSelected(int choiceIndex)
    {
        EventPhase currentPhase = currentEvent.Phases[currentPhaseIndex - 1];
        SelectedChoice = currentPhase.Choices[choiceIndex];

        if (SelectedChoice.AcquisitionTrigger)
        {
            Debug.Log("ȹ�� UI Ȱ��ȭ!");
            acquisitionUI.OpenAcquisitionUI(SelectedChoice.AcqType.Value, SelectedChoice.AcqID.Value);
            return;
        }

        if (SelectedChoice.BattleTrigger)
        {
            Debug.Log("���� ����!");
            // ���� ����
            battleManager.StartBattle(SelectedChoice);
            return;
        }

        if (SelectedChoice.NextPhaseIndex != -1) //  Ư�� ������� �̵��� ���
        {
            MoveToNextPhase(SelectedChoice.NextPhaseIndex); // ���� `StartEvent()` ��� `MoveToNextPhase()` ���
            return;
        }

        if (SelectedChoice.IsEventEnd())
        {
            HandleEventEnd();
        }
        else
        {
            StartEvent(SelectedChoice.NextEventName);
        }
    }

    private void HandleEventEnd()
    {
        Debug.Log("�̺�Ʈ�� ����Ǿ����ϴ�. ���� �̺�Ʈ ����!");

        EventData randomEvent = GetRandomEvent();
        if (randomEvent != null)
        {
            StartEvent(randomEvent.EventName);
        }
        else
        {
            Debug.Log("��� ������ ���� �̺�Ʈ�� �����ϴ�!");
        }
    }

    public void MoveToNextPhase(int nextPhaseIndex)
    {
        if (currentEvent == null)
        {
            Debug.LogError("���� ���� ���� �̺�Ʈ�� �����ϴ�!");
            return;
        }

        if (nextPhaseIndex < 0 || nextPhaseIndex >= currentEvent.Phases.Count)
        {
            Debug.LogError($"�߸��� ������ �ε���: {nextPhaseIndex} (�� ������ ����: {currentEvent.Phases.Count})");
            return;
        }

        Debug.Log($"������ �̵�: {currentPhaseIndex} �� {nextPhaseIndex}");
        currentPhaseIndex = nextPhaseIndex; // ������ �ε����� ���� ����
        ProcessPhase(); // ���� ������ ����
    }
}
