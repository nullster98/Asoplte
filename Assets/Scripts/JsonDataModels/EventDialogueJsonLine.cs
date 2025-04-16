using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class EventDialogueJsonLine
{
    public string eventID;
    public string phaseID;
    public string dialogueID;
    public string dialoguePath;
    public string condition;
    public string outcomeScript;
    public string nextEventID;
    public string nextPhaseID;
}
