using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;

namespace Entities
{
    public class Enemy : MonoBehaviour, IUnit
    {
        private EntitiesData enemyData;
        
        public UnitType UnitType => UnitType.Enemy;

        public int level;
        private Dictionary<string, int> stats = new()
        {
            { "Atk", 0 },
            { "Def", 0 },
            { "HP", 0 },
            { "MP", 0 },
            { "CurrentHP", 0 },
            { "CurrentMP", 0 }
        };

        public int HP => GetStat("HP");
        public int MP => GetStat("MP");
        public int Atk => GetStat("Atk");
        public int Def => GetStat("Def");
        public int CurrentHP => GetStat("CurrnetHP");
        public int CurrentMP => GetStat("CurrnetMP");

        [SerializeField] private TMP_Text enemyHPText;
        [SerializeField] private Slider enemyHp;
        [SerializeField] private Image enemySprite; 
        
        
        public void Initialize(EntitiesData data)
        {
            enemyData = data;
            SetStat("CurrentHP", data.MaxHp); 
            SetStat("CurrentMP", data.MaxMp);
        }

        public void SetStat(string key, int value)
        {
            stats[key] = value;
        }

        public void TakeDamage(int dmg)
        {
            ChangeStat("CurrentHP", -dmg);
            if (CurrentHP <= 0) SetStat("CurrentHP", 0);
        }

        public void Heal(int amount)
        {
            ChangeStat("CurrentHP", amount);
            if (CurrentHP > HP) SetStat("CurrentHP", HP);
        }

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

        public void ChangeStat(string statName, int value)
        {
            if (stats.ContainsKey(statName))
            {
                stats[statName] += value;
                Debug.Log($"{statName}이(가) {value} 만큼 변경됨. 현재 값: {stats[statName]}");
            }
        }

        public int GetStat(string key)
        {
            return stats.TryGetValue(key, out int value) ? value : 0;
        }

    }
}
