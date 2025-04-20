using System;
using System.Collections.Generic;
using UnityEngine;

public class Enemy
{
  [Serializable]
  public class Parameter
  {
    public string Id;
    public int Value;
  }

  public EnemySource Source { get; private set; }

  public Parameter[] Parameters { get; private set; }

  public Func<Battle, Queue<EnemyActionData>> AI { get; private set; }

  public int MaxHp { get; private set; }

  public int Hp { get; private set; }

  public enum EnemyActionTypes
  {
    Attack,
    Defend,
    Heal,
  }

  public class EnemyActionData
  {
    public EnemyActionTypes ActionType;
    public int Value;

    public EnemyActionData(EnemyActionTypes actionType, int value)
    {
      ActionType = actionType;
      Value = value;
    }
  }

  public Queue<EnemyActionData> NextActions { get; private set; }

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

  public Enemy(EnemySource source)
  {
    Source = source;
    Parameters = source.Parameters;
    AI = source.GenerateAI();
    MaxHp = Array.Find(Parameters, param => param.Id == "max_hp").Value;
    Hp = MaxHp;
    StatusEffects = new List<StatusEffect>();
  }

  public void SetNextActionData(Battle battle)
  {
    NextActions = AI(battle);
  }

  public bool ReceiveDamage(int damageValue)
  {
    foreach (StatusEffect statusEffect in StatusEffects)
    {
      if (statusEffect.Type == StatusEffect.EffectTypes.DefenceUp)
      {
        damageValue = Mathf.Max(0, damageValue - statusEffect.Value);
      }
    }
    Hp = Mathf.Max(0, Hp - damageValue);
    return Hp == 0;
  }

  public void Heal(int healValue)
  {
    Hp = Mathf.Min(MaxHp, Hp + healValue);
  }

  public bool CanPlayAction()
  {
    return NextActions.Count > 0;
  }

  public EnemyActionData PlayAction(Battle battle)
  {
    if (NextActions.Count == 0) return null;

    EnemyActionData action = NextActions.Dequeue();
    if (action.ActionType == EnemyActionTypes.Attack)
    {
      battle.MainCharacter.ReceiveDamage(action.Value);
    }
    else if (action.ActionType == EnemyActionTypes.Defend)
    {
      StatusEffects.Add(new StatusEffect(StatusEffect.EffectTypes.DefenceUp, action.Value, 1));
    }
    else if (action.ActionType == EnemyActionTypes.Heal)
    {
      Heal(action.Value);
    }
    
    return action;
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
