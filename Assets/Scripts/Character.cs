using System.Collections.Generic;
using UnityEngine;

public class Character
{
  public string Name { get; private set; }
  public int MaxHp { get; private set; }
  public int Hp { get; private set; }

  public class StatusEffect
  {
    public enum EffectTypes
    {
      DefenceUp
    }

    public EffectTypes Type;
    public int Value;
    public int RemainingTurn;

    public StatusEffect(EffectTypes type, int value, int turnNumber)
    {
      Type = type;
      Value = value;
      RemainingTurn = turnNumber;
    }
  }

  public List<StatusEffect> StatusEffects;

  public Character(string name, int maxHp)
  {
    Name = name;
    MaxHp = maxHp;
    Hp = MaxHp;
    StatusEffects = new List<StatusEffect>();
  }

  public void ReceiveDamage(int damageValue, BattleEventQueue beq)
  {
    foreach (StatusEffect statusEffect in StatusEffects)
    {
      if (statusEffect.Type == StatusEffect.EffectTypes.DefenceUp)
      {
        damageValue = Mathf.Max(0, damageValue - statusEffect.Value);
      }
    }
    Hp = Mathf.Max(0, Hp - damageValue);

    {
      BattleEvent be = new BattleEvent(BattleEvent.EventTypes.CharacterDamage);
      be.Value = Hp;
      beq.Enqueue(be);
    }
  }

  public void Heal(int healValue, BattleEventQueue beq)
  {
    Hp = Mathf.Min(MaxHp, Hp + healValue);

    {
      BattleEvent be = new BattleEvent(BattleEvent.EventTypes.CharacterHeal);
      be.Value = Hp;
      beq.Enqueue(be);
    }
  }

  public void AdvanceTurn()
  {
    List<StatusEffect> nextStatusEffects = new List<StatusEffect>();
    foreach (StatusEffect se in StatusEffects)
    {
      --se.RemainingTurn;
      if (se.RemainingTurn > 0)
      {
        nextStatusEffects.Add(se);
      }
    }
    StatusEffects = nextStatusEffects;
  }
}
