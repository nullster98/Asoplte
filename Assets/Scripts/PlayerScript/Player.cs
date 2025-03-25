using System.Collections.Generic;
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
                if (_instance) return _instance;
                // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                _instance = FindObjectOfType<Player>();
                if (_instance) return _instance;
                GameObject obj = new GameObject("Player");
                // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                _instance = obj.AddComponent<Player>();
                return _instance;
            }
        }
        private void Awake()
        {
            // 싱글톤 패턴 구현
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                Debug.Log("플레이어 데이터 생성");

            }
            else
            {
                Debug.Log("데이터가 이미 존재함");
                Destroy(gameObject);
            }

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
      
        private Equipment[] equippedItem = new Equipment[6];
        private EquipUI equipUI;
      
        [SerializeField] public Sprite playerImg; //플레이어 이미지

        public List<TraitData> selectedTraits = new List<TraitData>();
        private RaceData selectedRace { get; set; }
        public GodData selectedGod { get; set; }
        
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
        public Equipment GetEquippedItem(EquipmentType slot)
        {
            return equippedItem[(int)slot]; // 배열 인덱스로 직접 접근
        }

        public bool EquipItem(Equipment equipItem)
        {

            int slotIndex = (int)equipItem.EquipmentType;

            RemoveEquip(equipItem);

            //장비 장착
            equippedItem[slotIndex] = equipItem;
            Debug.Log($"{equipItem.ItemName}을 {equipItem.EquipmentType}칸에 장착했습니다.");

            ChangeStat("Atk", equipItem.AttackPoint);
            ChangeStat("Def", equipItem.DefensePoint);
            
            equipItem.Effects?.ForEach(effect => effect.ApplyEffect(this));

            equipUI?.UpdateEquipmentUI();

            return true;
        }

        public void RemoveEquip(Equipment equipitem)
        {
            int slotIndex = ( int)equipitem.EquipmentType;
            // 장비 제거
            if (equippedItem[slotIndex] != null)
            {
                Debug.Log($"기존 장비 {equippedItem[slotIndex].ItemName}을 해제합니다.");

                ChangeStat("Atk", -equippedItem[slotIndex].AttackPoint);
                ChangeStat("Def", -equippedItem[slotIndex].DefensePoint);

                equippedItem[slotIndex] = null;
            }
            
            equipitem.Effects?.ForEach(effect =>
            {
                if (effect is IRemovableEffect removable)
                {
                    removable.RemoveEffect(this);
                }
            });

            equipUI?.UpdateEquipmentUI();
        }

   /*public void RemoveItemEffects(Equipment equipItem)
    {
        foreach (var effect in equipItem.Effects)
        {
            effect.RemoveEffect(this); // 플레이어에게 적용된 효과 제거
        }
    }*/

        #endregion

        #region 신, 종족, 특성 저장및 적용 + 진행특성 적용
        
        public void SelectedGod(GodData god)
        {
            selectedGod = god;
            god.SpecialEffect?.ApplyEffect(this);
        }

        public void SelectedRace(RaceData race, SubRaceData subRace)
        {
            selectedRace = race;
            race.raceEffect?.ApplyEffect(this);
            subRace.subRaceEffect.ApplyEffect(this);
        }

        public void ApplyAllSelectedTraits()
        {
            foreach (var trait in selectedTraits)
            {
                trait.traiteffect?.ApplyEffect(this);
            }
        }

        public void ApplyTraits(TraitData trait)
        {
            selectedTraits.Add(trait);
            trait.traiteffect?.ApplyEffect(this);
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
