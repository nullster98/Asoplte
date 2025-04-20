using System.Collections.Generic;
using System.Linq;
using Game;
using God;
using Item;
using Race;
using Trait;
using UI;
using UnityEngine;
using UnityEngine.Serialization;

namespace PlayerScript
{
    public class Player : MonoBehaviour, IUnit
    {
        //싱글톤

        #region 싱글톤

        private static Player _instance;

        public static Player Instance
        {
            get
            {
                if (_instance != null) return _instance;

                // ⚠ Awake() 전에 접근 방지용 로그
                Debug.LogWarning("⚠ Player.Instance가 아직 초기화되지 않았습니다.");
                return null;
            }
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Debug.Log("⚠ 중복 Player 발견, 파괴");
                Destroy(gameObject); // 중복 방지
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("✅ Player Awake() 호출, DontDestroyOnLoad 적용됨");
        }

        #endregion

        #region 플레이어 정보

        //플레이어 기본 스텟
        private Dictionary<string, int> stats = new Dictionary<string, int>()
        {
            { "Atk", 0 }, // 공격력
            { "Def", 0 }, // 방어력
            { "HP", 0 }, // 최대 체력
            { "MP", 0 }, // 최대 마나
            { "CurrentHP", 0 }, // 현재 체력
            { "CurrentMP", 0 }, // 현재 마나
            { "MentalStat", 0 }, // 정신력
            { "CurrentMentalStat", 0 }, // 현재 정신력
            { "FaithStat", 0 }, // 신앙심
            { "Gold", 1000 } // 골드
        };

        public int HP => GetStat("HP");
        public int MP => GetStat("MP");
        public int Atk => GetStat("Atk");
        public int Def => GetStat("Def");
        public int CurrentHP => GetStat("CurrentHP");
        public int CurrentMP => GetStat("CurrentMP");
        public int MentalStat => GetStat("MentalStat");
        public int CurrentMentalStat => GetStat("CurrentMentalStat");
        public int FaithStat => GetStat("FaithStat");
        public int Gold => GetStat("Gold");
        private float FaithPoint { get; set; } = 500f; //신앙포인트
        public int MaxCost { get; set; } // 코스트
        public UnitType UnitType => UnitType.Player;

        private ItemData[] equippedItem = new ItemData[6];
        private EquipUI equipUI;

        [SerializeField] public Sprite playerImg; //플레이어 이미지

        //public List<TraitData> selectedTraits = new List<TraitData>();
        public HashSet<string> selectedTraitIDs = new();
        public RaceData selectedRace { get; set; }
        public SubRaceData selectedSubRace { get; set; }
        public GodData selectedGod { get; set; }
        public string godID;
        public string _lastAppliedGodID;
        public string majorRaceID;
        public string subRaceID;
        public string fullRaceID => $"{majorRaceID}{subRaceID}";

        #endregion

        public void TakeDamage(int dmg)
        {
            ChangeStat("CurrentHP", -dmg);
        }

        public void Heal(int amount)
        {
            ChangeStat("CurrentHP", amount);
        }

        public void Die()
        {
            //EndingUI.
        }

        void Start()
        {
            StartStat();
            RecoverAll();
            MaxCost = 15;
        }

        #region 신앙포인트관련

        public string GetFaithString()
        {
            return FaithPoint.ToString("F0");
        }

        public float GetFaithPoint()
        {
            return FaithPoint;
        }


        public void SetFaithPoint(float value)
        {
            FaithPoint = value;
        }

        public bool SpendFaith(float amount)
        {
            if (FaithPoint >= amount)
            {
                FaithPoint -= amount;
                return true;
            }

            return false;
        }

        public void AddFaithPoint(float amount)
        {
            FaithPoint += amount;
        }

        #endregion

        #region 스탯 관련

        void StartStat() //초기플레이어 스텟
        {
            SetStat("Atk", 10);
            SetStat("Def", 10);
            SetStat("HP", 100);
            SetStat("MP", 100);
            SetStat("CurrentHP", GetStat("HP"));
            SetStat("CurrentMP", GetStat("MP"));
            SetStat("MentalStat", 100);
            SetStat("FaithStat", 100);
        }

        private void SetStat(string key, int value)
        {
            stats[key] = value;
        }


        public void ChangeStat(string statName, int value)
        {
            if (stats.ContainsKey(statName))
            {
                int before = stats[statName];
                stats[statName] += value;
                if (statName == "CurrentHP")
                {
                    int max = GetStat("HP");
                    stats[statName] = Mathf.Clamp(stats[statName], 0, max);
                }
                else if (statName == "CurrentMP")
                {
                    int max = GetStat("MP");
                    stats[statName] = Mathf.Clamp(stats[statName], 0, max);
                }
                int after = stats[statName];
                Debug.Log($"[ChangeStat] {statName}: {before} → {after} (변화량: {value})");
            }
            else
            {
                Debug.LogWarning($"{statName} 스탯이 존재하지 않습니다!");
            }
        }

        public int GetStat(string statName)
        {
            return stats.TryGetValue(statName, out var stat) ? stat : 0;
        }

        #endregion

        #region 장비관련함수

        public void SetEquipUI(EquipUI uI)
        {
            equipUI = uI;
        }

        //  특정 슬롯의 장착된 장비를 가져오는 함수
        public ItemData GetEquippedItem(EquipmentType slot)
        {
            if (slot == EquipmentType.None)
            {
                Debug.LogWarning("[Player] None 슬롯은 장비가 존재하지 않습니다.");
                return null;
            }

            return equippedItem[(int)slot];
        }

        public bool EquipItem(ItemData equipItem)
        {
            int slotIndex = (int)equipItem.equipSlot;

            RemoveEquip(equipItem);

            //장비 장착
            equippedItem[slotIndex] = equipItem;
            Debug.Log($"{equipItem.itemName}을 {equipItem.equipSlot}칸에 장착했습니다.");

            ApplyItemEffects(equipItem);

            equipUI?.UpdateEquipmentUI();

            return true;
        }

        public void RemoveEquip(ItemData equipitem)
        {
            int slotIndex = (int)equipitem.equipSlot;
            // 장비 제거
            if (equippedItem[slotIndex] != null)
            {
                Debug.Log($"기존 장비 {equippedItem[slotIndex].itemName}을 해제합니다.");

                RemoveItemEffects(equipitem);

                equippedItem[slotIndex] = null;
            }

            equipUI?.UpdateEquipmentUI();
        }

        private void ApplyItemEffects(ItemData item)
        {
            if (item?.effects == null) return;

            foreach (var effect in item.effects)
            {
                effect.ApplyEffect(this);
            }
        }

        private void RemoveItemEffects(ItemData item)
        {
            if (item?.effects == null) return;

            foreach (var effect in item.effects)
            {
                if (effect is IRemovableEffect removable)
                {
                    removable.RemoveEffect(this);
                }
            }
        }

        #endregion

        #region 신, 종족, 특성 저장및 적용 + 진행특성 적용

        public void SelectedGod(GodData god)
        {
            selectedGod = god;
            godID = god.GodID;
            foreach (var effect in god.SpecialEffect)
            {
                effect?.ApplyEffect(this);
            }
        }

        public void RecoverGodFromID()
        {
            var found = DatabaseManager.Instance.godList
                .FirstOrDefault(g => g.GodID == godID);

            if (found == null)
            {
                Debug.LogWarning($"[RecoverGodFromID] godID '{godID}'에 해당하는 신을 찾을 수 없습니다.");
                return;
            }

            selectedGod = found;

            if (_lastAppliedGodID == selectedGod.GodID)
            {
                Debug.Log($"[RecoverGodFromID] 이미 {selectedGod.GodID} 효과가 적용되어 있음 → 재적용 생략");
                return;
            }

            foreach (var effect in selectedGod.SpecialEffect)
            {
                effect?.ApplyEffect(this);
            }

            _lastAppliedGodID = selectedGod.GodID;
        }

        public void SelectedRace(RaceData race, SubRaceData subRace)
        {
            selectedRace = race;
            selectedSubRace = subRace;
            subRaceID = subRace.subRaceID;
            foreach (var effect in subRace.subRaceEffect)
            {
                effect.ApplyEffect(this);
            }
        }

        public void RecoverRaceFromID(string subRaceID)
        {
            var (race, sub) = DatabaseManager.Instance.GetSubRaceByID(subRaceID);
            if (sub != null && race != null)
            {
                SelectedRace(race, sub);
                Debug.Log($"[🔁 복원 완료] {subRaceID} → {race.raceName} / {sub.subRaceName}");
            }
            else
            {
                Debug.LogWarning($"[❌ 복원 실패] subRaceID: {subRaceID} 를 찾을 수 없습니다.");
            }
        }

        public bool HasRace(string requiredID)
        {
            return requiredID == fullRaceID
                   || requiredID == majorRaceID
                   || requiredID == subRaceID;
        }

        public void ApplyAllSelectedTraits()
        {
            foreach (string trait in selectedTraitIDs)
            {
                TraitData traitData = DatabaseManager.Instance.GetTraitData(trait);
                ApplyTraitEffect(traitData);
            }
        }

        public void ApplyTraitEffect(TraitData traitData)
        {
            if (traitData == null) return;

            // 효과 적용 전 초기화 (필요시)
            if (traitData.traitEffect == null || traitData.traitEffect.Count == 0)
            {
                traitData.initializeEffect();
            }

            // 효과 적용
            if (traitData.traitEffect != null)
            {
                foreach (var effect in traitData.traitEffect)
                {
                    effect?.ApplyEffect(this);
                }

                Debug.Log($"[Player] 특성 {traitData.traitID} ({traitData.traitName}) 효과 적용 완료.");
            }
            else
            {
                Debug.LogWarning($"[Player] 특성 {traitData.traitID} ({traitData.traitName}) null");
            }
        }


        public void RecoverTraitsFromIDs()
        {
            Debug.Log($"[Player] ID 기반 특성 복구 시작... ID 개수: {selectedTraitIDs.Count}");
            foreach (string traitID in selectedTraitIDs)
            {
                TraitData traitData = DatabaseManager.Instance.GetTraitData(traitID);
                if (traitData != null)
                {
                    Debug.Log($"[Player] 특성 {traitID} 효과 복구 중...");
                    ApplyTraitEffect(traitData); // 내부 함수 호출
                }
                else
                {
                    Debug.LogWarning($"[Player] 특성 ID {traitID}에 해당하는 데이터를 찾을 수 없어 복구/적용 불가");
                }
            }

            Debug.Log("[Player] ID 기반 특성 복구 완료.");
        }

        public bool HasTrait(string requiredID)
        {
            if (string.IsNullOrEmpty(requiredID)) return false; // ID가 비어있으면 false 반환
            return selectedTraitIDs.Contains(requiredID); // HashSet의 Contains 사용
        }

        public void AddTrait(string traitID)
        {
            if (string.IsNullOrWhiteSpace(traitID))
            {
                Debug.LogWarning("[Player] 추가하려는 traitID가 비어있다냥!");
                return;
            }

            // 데이터베이스에서 TraitData 가져오기 먼저!
            var traitData = DatabaseManager.Instance.GetTraitData(traitID);
            if (traitData == null)
            {
                Debug.LogWarning($"[Player] traitID {traitID}에 해당하는 TraitData가 존재하지 않아 추가 불가!");
                return; // ❌ traitID 유효하지 않으면 등록도 하지 않음
            }

            // 이미 가지고 있는지 확인하고, 없으면 ID 추가
            if (selectedTraitIDs.Add(traitID))
            {
                Debug.Log($"[Player] 특성 ID {traitID} 추가됨.");
                ApplyTraitEffect(traitData);
            }
            else
            {
                // 이미 가지고 있는 경우 (필요시 로깅)
                // Debug.Log($"[Player] 이미 특성 {traitID}를 가지고 있다냥.");
            }
        }

        public void RemoveTrait(string traitID)
        {
            if (string.IsNullOrEmpty(traitID))
            {
                Debug.LogWarning("[Player] 제거하려는 traitID가 비어있음");
                return;
            }

            // 가지고 있는지 확인하고 제거
            if (selectedTraitIDs.Remove(traitID)) // Remove 성공 시 true 반환
            {
                Debug.Log($"[Player] 특성 ID {traitID} 제거됨.");

                // 데이터베이스에서 TraitData 가져오기 (제거 효과 적용 위해)
                var traitData = DatabaseManager.Instance.GetTraitData(traitID);
                if (traitData != null)
                {
                    // 효과 적용 전 초기화 (필요시)
                    if (traitData.traitEffect == null || traitData.traitEffect.Count == 0)
                    {
                        traitData.initializeEffect();
                    }

                    // 제거 가능한 효과 적용
                    if (traitData.traitEffect != null)
                    {
                        foreach (var effect in traitData.traitEffect)
                        {
                            if (effect is IRemovableEffect removable) // IRemovableEffect 인터페이스 확인
                            {
                                removable.RemoveEffect(this);
                            }
                        }

                        Debug.Log($"[Player] 특성 {traitID} ({traitData.traitName}) 제거 효과 적용 완료.");
                    }
                    else
                    {
                        Debug.LogWarning($"[Player] 특성 {traitID} ({traitData.traitName})의 효과 리스트가 null");
                    }
                }
                else
                {
                    Debug.LogWarning($"[Player] traitID {traitID}에 해당하는 특성을 찾을 수 없어 제거 효과 적용 불가");
                }
            }
            else
            {
                // 제거할 ID가 없었음 (필요시 로깅)
                // Debug.Log($"[Player] 제거하려는 특성 ID {traitID}를 가지고 있지 않다냥.");
            }
        }

        public void RecoverAll()
        {
            if (DatabaseManager.Instance.raceList == null || DatabaseManager.Instance.raceList.Count == 0)
            {
                Debug.LogWarning("❌ RaceList가 비어 있음. RecoverAll을 나중에 다시 호출해야 함.");
                return;
            }

            if (DatabaseManager.Instance.traitList == null || DatabaseManager.Instance.traitList.Count == 0)
            {
                Debug.LogWarning("❌ TraitList가 비어 있음. RecoverAll을 나중에 다시 호출해야 함.");
                return;
            }

            if (DatabaseManager.Instance.godList == null || DatabaseManager.Instance.godList.Count == 0)
            {
                Debug.LogWarning("❌ GodList가 비어 있음. RecoverAll을 나중에 다시 호출해야 함.");
                return;
            }
            RecoverRaceFromID(subRaceID);
            RecoverTraitsFromIDs();
            RecoverGodFromID();
        }

        #endregion

        public void ApplyEffect(IEffect effect)
        {
            effect.ApplyEffect(this);
        }

        public void ApplyEffectList(IEnumerable<IEffect> effects)
        {
            foreach (var effect in effects)
            {
                effect.ApplyEffect(this);
            }
        }
    }
}