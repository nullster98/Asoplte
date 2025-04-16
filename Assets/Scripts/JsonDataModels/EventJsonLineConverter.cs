using System;
using System.Collections.Generic;
using System.Linq;
using Event;
using UnityEngine;

public static class EventJsonLineConverters
{
    public static EventData ToEventData(this EventMainJsonLine line)
    {
        return new EventData
        {
            eventID = line.eventID,
            eventName = line.eventName,
            eventType = Enum.TryParse<EventTag>(line.eventType, out var tag) ? tag : EventTag.None,
            allowedFloors = line.allowedFloors?.ToList() ?? new List<int>(),
            isRecycle = line.isRecycle,
            isUsed = false, // 기본값 초기화
            phases = new List<EventPhase>()
        };
    }

    public static EventPhase ToEventPhase(this EventPhaseJsonLine line)
    {
        return new EventPhase
        {
            eventID = line.eventID,
            phaseID = line.phaseID,
            phaseName = line.phaseName,
            imagePath = line.imagePath,
            phaseImage = null, // 이후 Resource.Load<Sprite>(imagePath)로 처리
            phaseOutcome = OutcomeParser.ToEventOutcome(OutcomeParser.Parse(line.outcomeScript)),
            dialogues = new List<DialogueBlock>()
        };
    }

    public static DialogueBlock ToDialogueBlock(this EventDialogueJsonLine line)
    {
        return new DialogueBlock
        {
            dialogueID = line.dialogueID,
            dialoguePath = line.dialoguePath,
            dialogueText = null, // 이후 Resource.Load<TextAsset>(dialoguePath)로 처리
            phaseID = line.phaseID,
            condition = new EventCondition
            {
                dslExpression = line.condition,
            },
            outcome = OutcomeParser.ToEventOutcome(OutcomeParser.Parse(line.outcomeScript)),
            choices = new List<EventChoice>()
        };
    }

    public static EventChoice ToEventChoice(this EventChoiceJsonLine line)
    {
        return new EventChoice
        {
            choiceID = line.choiceID,
            dialogueID = line.dialogueID,
            choiceName = line.choiceName,
            nextEventID = line.nextEventID,
            nextPhaseID = line.nextPhaseID,
            nextDialogueID = line.nextDialogueID,
            condition = new EventCondition
            {
                dslExpression = line.condition,
            },
            outcome = OutcomeParser.ToEventOutcome(OutcomeParser.Parse(line.outcomeScript))
        };
    }
    
    public static List<EventPhase> ConvertPhaseLines(List<EventPhaseJsonLine> lines)
    {
        string lastEventID = "", lastImagePath = "";
        foreach (var line in lines)
        {
            if (!string.IsNullOrEmpty(line.eventID)) lastEventID = line.eventID;
            else line.eventID = lastEventID;

            if (!string.IsNullOrEmpty(line.imagePath)) lastImagePath = line.imagePath;
            else line.imagePath = lastImagePath;
        }

        return lines.Select(line => line.ToEventPhase()).ToList();
    }

    public static List<DialogueBlock> ConvertDialogueLines(List<EventDialogueJsonLine> lines)
    {
        string lastEventID = "", lastPhaseID = "";
        foreach (var line in lines)
        {
            if (!string.IsNullOrEmpty(line.eventID)) lastEventID = line.eventID;
            else line.eventID = lastEventID;

            if (!string.IsNullOrEmpty(line.phaseID)) lastPhaseID = line.phaseID;
            else line.phaseID = lastPhaseID;
        }

        return lines.Select(line => line.ToDialogueBlock()).ToList();
    }

    public static List<EventChoice> ConvertChoiceLines(List<EventChoiceJsonLine> lines)
    {
        string lastDialogueID = "";
        foreach (var line in lines)
        {
            if (!string.IsNullOrEmpty(line.dialogueID)) lastDialogueID = line.dialogueID;
            else line.dialogueID = lastDialogueID;
        }

        return lines.Select(line => line.ToEventChoice()).ToList();
    }
}
