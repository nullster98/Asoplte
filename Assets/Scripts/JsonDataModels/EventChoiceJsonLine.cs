using System;
using System.Collections;
using System.Collections.Generic;
using Event;
using UnityEngine;
[Serializable]
public class EventChoiceJsonLine
{
    public string dialogueID;
    public string choiceID;
    public string choiceName;
    public string nextPhaseID;
    public string nextEventID;
    public string nextDialogueID;
    public string condition;
    public string outcomeScript;
}
