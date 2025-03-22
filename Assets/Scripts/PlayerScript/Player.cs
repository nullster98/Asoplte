using System.Collections.Generic;
using God;
using Item;
using UI;
using UnityEngine;

namespace PlayerScript
{
    public class Player : MonoBehaviour
    {
        //싱글톤
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

        private Equipment[] equippedItem = new Equipment[6];
        private EquipUI equipUI;


        public int MaxCost { get; set; } // 코스트
        [SerializeField] public Sprite playerImg; //플레이어 이미지

        public List<Trait.Trait> selectedTraits = new List<Trait.Trait>();

        private float FaithPoint { get; set; } = 500f;//신앙포인트
        //private RaceData selectedRace;

        public GodData SelectedGod { get; set; }

        /*public RaceData SelectedRace
    {
        get { return selectedRace; }
        set { selectedRace = value; }
    }*/

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

        public void SetEquipUI(EquipUI uI)
        {
            equipUI = uI;
        }


        // 특성들을 한꺼번에 적용
        public void ApplySelectedTraits(List<Trait.Trait> traits)
        {
            foreach (var trait in traits)
            {
                ApplyTraitEffect(trait);
                selectedTraits.Add(trait); // 선택된 특성 리스트에 추가
            }
        }

        // 특성의 효과를 적용하는 메서드
        private void ApplyTraitEffect(Trait.Trait trait)
        {
            switch (trait.traitName)
            {
                case "불타는 마력":
                    // ChangeStat("Atk", GetStat("MP") * 0.1f);
                    break;

                case "생명전환":
                    int excessHp = GetStat("CurrentHP") - GetStat("HP");
                    if (excessHp > 0)
                    {
                        ChangeStat("MP", excessHp);
                        ChangeStat("CurrentHP", -excessHp);
                    }
                    break;

                case "어두운 시야":
                    // 적의 체력이 보이지 않도록 설정 (UI 상태 업데이트 필요)
                    break;

                case "마나의 역류":
                    // 마나를 사용하는 스킬이 체력을 소모하게 함 (스킬 발동 로직 반영 필요)
                    break;
            }
        }

        // 선택된 특성의 효과를 제거하는 메서드도 필요할 경우 구현 가능


        void Start()
        {
            StartStat();
            MaxCost = 15;
        }

        #region FatihPoint Get,Set
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

        private void ApplyGodBonuses(GodData selectedGod) //신앙 선택에 따른 추가효과
        {
            foreach(var effect in selectedGod.GodStats)
            {
                ChangeStat(effect.Key, effect.Value);
            }

            selectedGod.SpecialEffect?.ApplyEffect(this);
        }

        private void ApplyRaceBonuses() //종족 선택에 따른 추가효과
        {
        
        }

        void StartStat() //초기플레이어 스텟
        {
            ChangeStat("Atk", 10);
            ChangeStat("Def", 10);
            ChangeStat("HP", 100);
            ChangeStat("MP", 100);
            ChangeStat("CurrentHP", GetStat("HP"));
            ChangeStat("CurrentMP", GetStat("MP"));
            ChangeStat("MentalStat", 100);
            ChangeStat("FaithStat", 100);       
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

        #region 장비관련함수
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

            equipUI?.UpdateEquipmentUI();
        }

        public void ApplyItemEffects(Equipment equipItem)
        {
            foreach (var effect in equipItem.Effects)
            {
                effect.ApplyEffect(this); // 플레이어에게 효과 적용
            }
        }

        /*public void RemoveItemEffects(Equipment equipItem)
    {
        foreach (var effect in equipItem.Effects)
        {
            effect.RemoveEffect(this); // 플레이어에게 적용된 효과 제거
        }
    }*/

        #endregion
    }
}
