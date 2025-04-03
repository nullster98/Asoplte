using System.Collections.Generic;

[System.Serializable]
public class FlatEventLine
{
    public string eventName;
    public string isRecycle;
    public string eventType;
    public string phaseName;
    public string backgroundImage;

    public string dialogueText;
    public string choiceName;
    public string requiredTraits;
    public string nextEventName;
    public int nextPhaseIndex;

    // Outcome 구분 타입 (예: Choice, Dialogue 등)
    public string outcomeType;

    // 전투 관련
    public bool startBattle;
    public bool spawnEntity;
    public int entityID;

    // 보상 관련
    public bool giveReward;
    public string rewardType;
    public int rewardID;

    // 상태 변화 - 스탯
    public string stat_1_name;
    public int stat_1_amount;
    public string stat_2_name;
    public int stat_2_amount;
    public string stat_3_name;
    public int stat_3_amount;

    // 상태 변화 - 트레잇 추가
    public string addTrait_1;
    public string addTrait_2;
    public string addTrait_3;

    // 상태 변화 - 트레잇 제거
    public string removeTrait_1;
    public string removeTrait_2;
    public string removeTrait_3;
}
