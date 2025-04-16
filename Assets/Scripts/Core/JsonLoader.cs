using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Entities;
using Event;
using God;
using Item;
using JsonDataModels;
using Race;
using Trait;
using UnityEditor.Experimental.GraphView;

public static class JsonLoader
{
    #region 이벤트 통합 로더

    public static List<EventData> LoadCompleteEventData()
    {
        // 1. JSON 파일 로드
        var mainJson = Resources.Load<TextAsset>("Json/eventmain_data");
        if (mainJson == null || string.IsNullOrEmpty(mainJson.text)) // <--- 여기!
        {
            Debug.LogError("❌ [JsonLoader] eventmain_data.json 로드 실패 또는 내용 없음");
            return new List<EventData>();
        }
        var phaseJson = Resources.Load<TextAsset>("Json/eventphase_data");
        if (phaseJson == null) Debug.LogError("2없데");
        var dialogueJson = Resources.Load<TextAsset>("Json/eventdialogue_data");
        if (dialogueJson == null) Debug.LogError("3없데");
        var choiceJson = Resources.Load<TextAsset>("Json/eventchoice_data");
        if (choiceJson == null) Debug.LogError("4없데");

        // 2. JsonLine 파싱
        var mainLineList = JsonHelper.FromJson<EventMainJsonLine>(mainJson.text);
        if (mainLineList == null)
        {
            Debug.LogError("❌ [JsonLoader] eventmain_data 파싱 실패 → Json 배열 구조 확인 필요");
            return new List<EventData>();
        }
        var phaseLineList = JsonHelper.FromJson<EventPhaseJsonLine>(phaseJson.text);
        var dialogueLineList = JsonHelper.FromJson<EventDialogueJsonLine>(dialogueJson.text);
        var choiceLineList = JsonHelper.FromJson<EventChoiceJsonLine>(choiceJson.text);

        // 3. 변환 + ID 상속 포함
        var mainList = mainLineList.Select(line => line.ToEventData()).ToList();
        var phaseList = EventJsonLineConverters.ConvertPhaseLines(phaseLineList);
        var dialogueList = EventJsonLineConverters.ConvertDialogueLines(dialogueLineList);
        var choiceList = EventJsonLineConverters.ConvertChoiceLines(choiceLineList);

        // 4. 연결
        foreach (var dialogue in dialogueList)
        {
            dialogue.choices = choiceList
                .Where(c => c.dialogueID == dialogue.dialogueID)
                .ToList();
        }

        foreach (var phase in phaseList)
        {
            phase.dialogues = dialogueList
                .Where(d => d.phaseID == phase.phaseID)
                .ToList();
        }

        foreach (var e in mainList)
        {
            e.phases = phaseList
                .Where(p => p.eventID == e.eventID)
                .ToList();
        }

        return mainList;
    }

    #endregion
    
     #region 이벤트 JsonLoader
    //
    // public static List<FlatEventLine> LoadFlatLinesFromJson()
    // {
    //     TextAsset jsonFile = Resources.Load<TextAsset>("Json/event_data");
    //     if (jsonFile == null)
    //     {
    //         Debug.LogError("[JsonLoader] JSON 파일을 찾을 수 없습니다.");
    //         return new List<FlatEventLine>();
    //     }
    //
    //     List<FlatEventLine> rawLines = JsonHelper.FromJson<FlatEventLine>(jsonFile.text);
    //     List<FlatEventLine> filledLines = new List<FlatEventLine>();
    //
    //     string currentEventName = "";
    //     string currentPhaseName = "";
    //
    //     foreach (var line in rawLines)
    //     {
    //         // eventName / phaseName 자동 채움
    //         if (!string.IsNullOrEmpty(line.eventName))
    //             currentEventName = line.eventName;
    //         else
    //             line.eventName = currentEventName;
    //
    //         if (!string.IsNullOrEmpty(line.phaseName))
    //             currentPhaseName = line.phaseName;
    //         else
    //             line.phaseName = currentPhaseName;
    //         
    //         // outcomeScript가 존재한다면 파싱해서 outcome으로 변환
    //         if (!string.IsNullOrWhiteSpace(line.outcomeScript))
    //         {
    //             var parsed = OutcomeParser.Parse(line.outcomeScript);
    //             line.outcome = OutcomeParser.ToEventOutcome(parsed);
    //         }
    //         else
    //         {
    //             line.outcome = new EventOutcome();
    //         }
    //
    //         filledLines.Add(line);
    //     }
    //
    //     Debug.Log($"[JsonLoader] JSON에서 {filledLines.Count}개의 이벤트 라인을 불러왔습니다.");
    //     return filledLines;
    // }
    //
    #endregion

    #region 신 JsonLoader

    public static List<GodData> LoadGodData()
    {
        TextAsset json = Resources.Load<TextAsset>("Json/god_data");
        if (json == null)
        {
            Debug.LogError("GodData.json 로드 실패");
            return new List<GodData>();
        }

        List<GodJsonLine> lines = JsonHelper.FromJson<GodJsonLine>(json.text);
        List<GodData> list = new();

        foreach (var line in lines)
        {
            var god = new GodData
            {
                GodName = line.GodName,
                GodID = line.GodID,
                UnlockCost = line.UnlockCost,
                EffectKey = line.Effect,
                IsUnlocked = (line.GodName == "Liberty"),
                codexPath = line.CodexPath,
                imagePath = line.ImagePath,
                bgPath = line.bgPath,
                summary = line.Summary,
                unlockHint = line.UnlockHint,
            };
            god.initializeEffect();
            list.Add(god);
        }

        return list;
    }

    #endregion

    #region 특성 JsonLoader
    public static List<TraitData> LoadTraitData()
    {
        TextAsset json = Resources.Load<TextAsset>("Json/trait_data");
        if (json == null)
        {
            Debug.LogError("TraitData.json 로드 실패");
            return new List<TraitData>();
        }

        List<TraitJsonLine> lines = JsonHelper.FromJson<TraitJsonLine>(json.text);
        List<TraitData> list = new();

        string[] initialUnlocks =
        {
            "TP01", "TP02", "TP03",
            "TN01", "TN02", "TN03"
        };
        
        
        foreach (var line in lines)
        {
            TraitPnN parsedPnN = TraitPnN.Positive;
            if (!string.IsNullOrWhiteSpace(line.PnN))
            {
                Enum.TryParse(line.PnN.Trim(), true, out parsedPnN);
            }

            Rarity parsedRarity = Rarity.Common;
            if (!string.IsNullOrWhiteSpace(line.Rarity))
            {
                Enum.TryParse(line.Rarity.Trim(), true, out parsedRarity);
            }
            var trait = new TraitData
            {
                traitName = line.TraitName,
                PnN = parsedPnN,
                rarity = parsedRarity,
                traitID = line.TraitID,
                cost = line.Cost,
                EffectKey = line.Effect,
                isUnlock = initialUnlocks.Contains(line.TraitID),
                codexPath = line.CodexPath,
                imagePath = line.ImagePath,
                summary = line.Summary,
                unlockHint = line.UnlockHint,
            };
            trait.initializeEffect();
            list.Add(trait);
        }

        return list;
    }
    #endregion

    #region 종족 JsonLoader

    public static List<RaceData> LoadRaceData()
    {
        TextAsset json = Resources.Load<TextAsset>("Json/race_data");
        if (json == null)
        {
            Debug.LogError("RaceData.json 로드 실패");
            return new List<RaceData>();
        }

        List<RaceJsonLine> lines = JsonHelper.FromJson<RaceJsonLine>(json.text);
        List<RaceData> list = new();

        string[] initialUnlocks =
        {
            "R1S1", "R2S1"
        };

        RaceData currentRace = null;
        
        // 이전 값 저장용 캐시
        string lastRaceName = "";
        string lastRaceID = "";
        string lastRaceEffectKey = "";

        foreach (var line in lines)
        {
            // 빈 값은 이전 값으로 채움
            if (string.IsNullOrWhiteSpace(line.RaceName)) line.RaceName = lastRaceName;
            else lastRaceName = line.RaceName;

            if (string.IsNullOrWhiteSpace(line.RaceID)) line.RaceID = lastRaceID;
            else lastRaceID = line.RaceID;

            if (string.IsNullOrWhiteSpace(line.RaceEffect)) line.RaceEffect = lastRaceEffectKey;
            else lastRaceEffectKey = line.RaceEffect;

            // 새로운 종족이면 새 RaceData 생성
            if (!string.IsNullOrWhiteSpace(line.RaceName) && (currentRace == null || currentRace.raceName != line.RaceName))
            {
                currentRace = new RaceData
                {
                    raceName = line.RaceName,
                    raceID = line.RaceID,
                    EffectKey = line.RaceEffect,
                    subRace = new List<SubRaceData>(),
                    codexPath = line.R_CodexPath,
                    imagePath = line.R_ImagePath,
                    summary = line.R_Summary,
                    unlockHint = line.R_UnlockHint,
                    
                };
                list.Add(currentRace);
            }

            var composedSubRaceID = $"{line.RaceID}{line.SubRaceID}";
            // SubRaceData 생성
            var sub = new SubRaceData
            {
                subRaceName = line.SubRaceName,
                fileName = line.SubFileName,
                parentRaceID = line.RaceID,
                subRaceID = line.SubRaceID,
                requireFaith = line.RequireFaith,
                isUnlocked = System.Array.Exists(initialUnlocks, id => id == composedSubRaceID),
                EffectKey = line.SubRaceEffect,
                codexPath = line.SR_CodexPath,
                imagePath = line.SR_ImagePath,
                summary = line.SR_Summary,
                unlockHint = line.SR_UnlockHint,
            };

            currentRace?.subRace.Add(sub);
        }
        foreach (var race in list)
        {
            race.InitializeRaceEffect();

            foreach (var sub in race.subRace)
            {
                sub.initializeSubRaceEffect(race.raceEffect);
            }
        }

        return list;
    }

    #endregion

    #region 아이템 JsonLoader

    public static List<ItemData> LoadItemData()
    {
        TextAsset json = Resources.Load<TextAsset>("Json/item_data");
        if (json == null)
        {
            Debug.LogError("ItemData.json 로드 실패");
            return new List<ItemData>();
        }
        
        List<ItemJsonLine> lines = JsonHelper.FromJson<ItemJsonLine>(json.text);
        List<ItemData> list = new();

        foreach (var line in lines)
        {
            ItemType parsedType = ItemType.None;
            if (!string.IsNullOrWhiteSpace(line.ItemType))
            {
                Enum.TryParse(line.ItemType.Trim(), true, out parsedType);
            }
            
            Rarity parsedRarity = Rarity.Common;
            if (!string.IsNullOrWhiteSpace(line.Rarity))
            {
                Enum.TryParse(line.Rarity.Trim(), true, out parsedRarity);
            }
            
            EquipmentType parsedEquipment = EquipmentType.None;
            if (!string.IsNullOrWhiteSpace(line.EquipType))
            {
                Enum.TryParse(line.EquipType.Trim(), true, out parsedEquipment);
            }
            
            var item = new ItemData
            {
                itemName = line.ItemName,
                itemType = parsedType,
                rarity = parsedRarity,
                itemID = line.ItemID,
                isEquipable = line.IsEquipable,
                equipSlot = parsedEquipment,
                purchasePrice = line.PurchasePrice,
                salePrice = line.SalePrice,
                EffectKey = line.Effect,
                codexPath = line.CodexPath,
                imagePath = line.ImagePath,
                summary = line.Summary,
            };
            item.initializeEffect();
            list.Add(item);
        }
        return list;
    }
    

    #endregion

    #region Entity JsonLoader

    public static List<EntitiesData> LoadEntitiesData()
    {
        TextAsset json = Resources.Load<TextAsset>("Json/entity_data");
        if (json == null)
        {
            Debug.LogError("Entity.json 로드 실패");
            return new List<EntitiesData>();
        }
        List<EntityJsonLine> lines = JsonHelper.FromJson<EntityJsonLine>(json.text);
        List<EntitiesData> list = new();
        

        foreach (var line in lines)
        {
            Enum.TryParse(line.Type, true, out EntityType parsedType);

            List<int> parsedFloors = string.IsNullOrWhiteSpace(line.SpawnFloor)
                ? new List<int>()
                : line.SpawnFloor.Split('|').Select(s =>
                {
                    if (int.TryParse(s.Trim(), out int result))
                        return result;
                    else
                        return -1;
                }).Where(i => i >= 0).ToList();
            List<string> preferredTraits = string.IsNullOrWhiteSpace(line.PreferredTraits)
                ? new List<string>()
                : line.PreferredTraits.Split('|').Select(s => s.Trim()).ToList();

            List<string> dislikedTraits = string.IsNullOrWhiteSpace(line.DislikedTraits)
                ? new List<string>()
                : line.DislikedTraits.Split('|').Select(s => s.Trim()).ToList();


            var entity = new EntitiesData
            {
                EntityID = line.EntityID,
                Name = line.Name,
                EntityType = parsedType,
                MaxHp = line.MaxHP,
                MaxMp = line.MaxMP,
                Attack = line.Attack,
                Defense = line.Defense,
                SpawnableFloors = parsedFloors,
                IsEventOnly = line.IsEventOnly,
                EffectKey = line.Effect,
                codexPath = line.CodexPath,
                imagePath = line.ImagePath,
                summary = line.Summary,
                
                PreferredTraits = preferredTraits,
                DislikedTraits = dislikedTraits,
                FixedGodID = line.FixedGodID,
                Personality = line.Personality,
            };
            entity.initializeEffect();
            list.Add(entity);
        }

        return list;
    }
    

    #endregion
}