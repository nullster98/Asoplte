using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitType //유닛 종류
{
  Player,
  Enemy,
  NPC
}
public interface IUnit //유닛 공통 인터페이스
{
  int HP { get; }
  int MP { get;  }
  int Atk { get; }
  int Def { get;  }
  UnitType UnitType { get; }

  void TakeDamage(int amount);
  void Heal(int amount);
  void ChangeStat(string key, int value);
  int GetStat(string key);
  void ApplyEffect(IEffect effect);
  void ApplyEffectList(IEnumerable<IEffect> effects);
}
