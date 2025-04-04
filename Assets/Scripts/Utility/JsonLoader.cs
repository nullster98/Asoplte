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
using UI;
using UnityEditor.iOS;
using UnityEditor.UIElements;
using UnityEngine.UI;

public static class JsonLoader
{
    #region 이벤트 JsonLoader

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
            line.rewardID = !string.IsNullOrWhiteSpace(line.rewardID) ? line.rewardID : null;
            line.entityID = !string.IsNullOrWhiteSpace(line.entityID) ? line.entityID : null;
            line.nextPhaseIndex = line.nextPhaseIndex != 0 ? line.nextPhaseIndex : -1;

            // ✅ enum (string) → null or string 유지
            line.rewardType = string.IsNullOrEmpty(line.rewardType) ? null : line.rewardType;

            filledLines.Add(line);
        }

        Debug.Log($"[JsonLoader] JSON에서 {filledLines.Count}개의 이벤트 라인을 불러왔습니다.");
        return filledLines;
    }

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
                FileName = line.FileName,
                GodID = line.GodID,
                UnlockCost = line.UnlockCost,
                EffectKey = line.Effect,
                IsUnlocked = (line.GodName == "Liberty"),
            };
            god.initializeEffect();
            god.LoadGodData();
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
            "StrongFaith", "PowerfulForce", "MentalResponse",
            "Blind", "Weakness", "Sleepless"
        };
        
        
        foreach (var line in lines)
        {
            TraitPnN parsedPnN = TraitPnN.Positive;
            if (!string.IsNullOrWhiteSpace(line.PnN))
            {
                Enum.TryParse(line.PnN.Trim(), true, out parsedPnN);
            }
            var trait = new TraitData
            {
                traitName = line.TraitName,
                fileName = line.FileName,
                PnN = parsedPnN,
                traitID = line.TraitID,
                cost = line.Cost,
                EffectKey = line.Effect,
                isUnlock = initialUnlocks.Contains(line.FileName)
            };
            trait.initializeEffect();
            trait.LoadTraitData();
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
        string lastFileName = "";
        string lastRaceID = "";
        string lastRaceEffectKey = "";

        foreach (var line in lines)
        {
            // 빈 값은 이전 값으로 채움
            if (string.IsNullOrWhiteSpace(line.RaceName)) line.RaceName = lastRaceName;
            else lastRaceName = line.RaceName;

            if (string.IsNullOrWhiteSpace(line.FileName)) line.FileName = lastFileName;
            else lastFileName = line.FileName;

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
                    fileName = line.FileName,
                    raceID = line.RaceID,
                    EffectKey = line.RaceEffect,
                    subRace = new List<SubRaceData>()
                };
                list.Add(currentRace);
            }

            var composedSubRaceID = $"{line.RaceID}{line.SubRaceID}";
            // SubRaceData 생성
            var sub = new SubRaceData
            {
                subRaceName = line.SubRaceName,
                fileName = line.SubFileName,
                subRaceID = composedSubRaceID,
                requireFaith = line.RequireFaith,
                isUnlocked = System.Array.Exists(initialUnlocks, id => id == composedSubRaceID),
                EffectKey = line.SubRaceEffect
            };

            currentRace?.subRace.Add(sub);
        }
        foreach (var race in list)
        {
            race.InitializeRaceEffect();
            race.LoadRaceData();

            foreach (var sub in race.subRace)
            {
                sub.initializeSubRaceEffect(race.raceEffect);
                sub.LoadSubRaceData();
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
                EffectKey = line.Effect
            };
            item.initializeEffect();
            item.LoadItemData();
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
        
        EntityType parsedEntity = EntityType.None;

        foreach (var line in lines)
        {
            Enum.TryParse(line.Type, true, out EntityType parsedType);
            
            var entity = new EntitiesData
            {
                EntityID = line.EntityID,
                Name = line.Name,
                EntityType = parsedType,
                MaxHp = line.MaxHP,
                MaxMp = line.MaxMP,
                Attack = line.Attack,
                Defense = line.Defense,
                SpawnableFloors = line.SpawnFloor,
                IsEventOnly = line.IsEventOnly,
                EffectKey = line.Effect
            };
            entity.initializeEffect();
            entity.LoadEnemySprite();
            entity.GetDescription();
            list.Add(entity);
        }

        return list;
    }
    

    #endregion
}