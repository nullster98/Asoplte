using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Entities;
using Event;
using UI;

public static class JsonLoader
{
    public static List<FlatEventLine> LoadFlatLinesFromJson()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("Json/event_data");
        if (jsonFile == null)
        {
            Debug.LogError("[JsonLoader] JSON 파일을 찾을 수 없습니다.");
            return new List<FlatEventLine>();
        }

        List<FlatEventLine> rawLines = JsonHelper.FromJson<FlatEventLine>(jsonFile.text);
        List<FlatEventLine> filledLines = new List<FlatEventLine>();

        string currentEventName = "";
        string currentPhaseName = "";

        foreach (var line in rawLines)
        {
            // ✅ eventName / phaseName 자동 채움
            if (!string.IsNullOrEmpty(line.eventName))
                currentEventName = line.eventName;
            else
                line.eventName = currentEventName;

            if (!string.IsNullOrEmpty(line.phaseName))
                currentPhaseName = line.phaseName;
            else
                line.phaseName = currentPhaseName;
            

            // ✅ int 기본값 처리
            line.rewardID = line.rewardID != 0 ? line.rewardID : 0;
            line.entityID = line.entityID != 0 ? line.entityID : 0;
            line.nextPhaseIndex = line.nextPhaseIndex != 0 ? line.nextPhaseIndex : -1;

            // ✅ enum (string) → null or string 유지
            line.rewardType = string.IsNullOrEmpty(line.rewardType) ? null : line.rewardType;

            filledLines.Add(line);
        }

        Debug.Log($"[JsonLoader] JSON에서 {filledLines.Count}개의 이벤트 라인을 불러왔습니다.");
        return filledLines;
    }
}