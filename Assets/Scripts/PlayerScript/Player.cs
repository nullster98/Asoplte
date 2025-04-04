using System.Collections.Generic;
using System.Linq;
using Game;
using God;
using Item;
using Race;
using Trait;
using UI;
using UnityEngine;

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
            { "Atk", 0 },          // 공격력
            { "Def", 0 },          // 방어력
            { "HP", 0 },          // 최대 체력
            { "MP", 0 },          // 최대 마나
            { "CurrentHP", 0 },   // 현재 체력
            { "CurrentMP", 0 },   // 현재 마나
            { "MentalStat", 0 },  // 정신력
            { "CurrentMentalStat", 0 }, // 현재 정신력
            { "FaithStat", 0 },   // 신앙심
            { "Gold", 1000 }        // 골드

        };

        public int HP =>GetStat("HP");
        public int MP => GetStat("MP");
        public int Atk => GetStat("Atk");
        public int Def => GetStat("Def");
        public int CurrentHP => GetStat("CurrentHP");
        public int CurrentMP => GetStat("CurrentMP");
        public int MentalStat => GetStat("MentalStat");
        public int CurrentMentalStat => GetStat("CurrentMentalStat");
        public int FaithStat => GetStat("FaithStat");
        public int Gold => GetStat("Gold");
        private float FaithPoint { get; set; } = 500f;//신앙포인트
        public int MaxCost { get; set; } // 코스트
        public UnitType UnitType => UnitType.Player;
      
        private ItemData[] equippedItem = new ItemData[6];
        private EquipUI equipUI;
      
        [SerializeField] public Sprite playerImg; //플레이어 이미지

        public List<TraitData> selectedTraits = new List<TraitData>();
        private RaceData selectedRace { get; set; }
        private SubRaceData selectedSubRace { get; set; }
        public GodData selectedGod { get; set; }
        public string selectedGodID;
        public string selectedSubRaceID;
        public List<string> selectedTraitID = new();
        
        #endregion
        public void TakeDamage(int dmg)
        {
            ChangeStat("CurrentHP", -dmg);
        }

        public void Heal(int amount)
        {
            ChangeStat("CurrentHP", amount);
        }
        
        void Start()
        {
            StartStat();
            RecoverAll();
            MaxCost = 15;
        }

        #region 신앙포인트관련
        public string GetFatihString()
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

        public void AddFatihPoint(float amount)
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
                stats[statName] += value;
                Debug.Log($"{statName}이(가) {value} 만큼 변경됨. 현재 값: {stats[statName]}");
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
            int slotIndex = ( int)equipitem.equipSlot;
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
            selectedGodID = god.GodID;
            foreach (var effect in god.SpecialEffect)
            {
                effect?.ApplyEffect(this);
            }
        }

        public void RecoverGodFromID()
        {
            selectedGod = DatabaseManager.Instance.godList
                .FirstOrDefault(g => g.GodID == selectedGodID);
            
            if (selectedGod != null)
            {
                foreach (var effect in selectedGod.SpecialEffect)
                {
                    effect?.ApplyEffect(this);
                }
            }
        }

        public void SelectedRace(RaceData race, SubRaceData subRace)
        {
            selectedRace = race;
            selectedSubRace = subRace;
            foreach (var effect in subRace.subRaceEffect)
            {
                effect.ApplyEffect(this);
            }
        }

        public void RecoverRaceFromID(string subRaceID)
        {
            foreach (var race in DatabaseManager.Instance.raceList)
            {
                foreach (var sub in race.subRace)
                {
                    if (sub.subRaceID == subRaceID)
                    {
                        SelectedRace(race, sub);
                        Debug.Log($"[🔁 복원 완료] {subRaceID} → {race.raceName} / {sub.subRaceName}");
                        return;
                    }
                }
            }

            Debug.LogWarning($"[❌ 복원 실패] subRaceID: {subRaceID} 를 찾을 수 없습니다.");
        }

        public void ApplyAllSelectedTraits()
        {
            foreach (var trait in selectedTraits)
            {
                ApplyTraitEffect(trait);
            }
        }

        public void ApplyTraitEffect(TraitData trait)
        {
            if (!selectedTraits.Contains(trait))
            {
                selectedTraits.Add(trait);

                if (!selectedTraitID.Contains(trait.traitID))
                {
                    selectedTraitID.Add(trait.traitID);
                }
            }

            if (trait.traitEffect == null || trait.traitEffect.Count == 0)
            {
                trait.initializeEffect(); // 누락된 경우 대비
            }

            foreach (var effect in trait.traitEffect)
            {
                effect?.ApplyEffect(this);
            }
        }
        
        public void RecoverTraitsFromIDs()
        {
            selectedTraits = selectedTraitID
                .Select(id => DatabaseManager.Instance.traitList
                    .FirstOrDefault(t => t.traitID == id))
                .Where(t => t != null)
                .ToList();

            ApplyAllSelectedTraits(); // 효과 재적용
        }

        public void RecoverAll()
        {
            if (DatabaseManager.Instance.godList == null || DatabaseManager.Instance.godList.Count == 0)
            {
                Debug.LogWarning("❌ GodList가 비어 있음. RecoverAll을 나중에 다시 호출해야 함.");
                return;
            }            
            
            RecoverGodFromID();
            RecoverTraitsFromIDs();
            RecoverRaceFromID(selectedSubRaceID);
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
