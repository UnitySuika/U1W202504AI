using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
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

  public Func<Battle, Enemy, Queue<EnemyActionData>> AI { get; private set; }

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
    NextActions = AI(battle, this);
  }

  public bool ReceiveDamage(int damageValue, BattleEventQueue beq)
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
      BattleEvent be = new BattleEvent(BattleEvent.EventTypes.EnemyDamage);
      be.Value = Hp;
      be.TargetEnemies = new List<Enemy>() { this };
      beq.Enqueue(be);
    }

    return Hp == 0;
  }

  public void Heal(int healValue, BattleEventQueue beq)
  {
    Hp = Mathf.Min(MaxHp, Hp + healValue);

    {
      BattleEvent be = new BattleEvent(BattleEvent.EventTypes.EnemyHeal);
      be.TargetEnemies = new List<Enemy>() { this };
      beq.Enqueue(be);
    }
  }

  public bool CanPlayAction()
  {
    return NextActions.Count > 0;
  }

  public EnemyActionData PlayAction(Battle battle, BattleEventQueue beq)
  {
    if (NextActions.Count == 0) return null;

    EnemyActionData action = NextActions.Dequeue();

    {
      BattleEvent be = new BattleEvent(BattleEvent.EventTypes.EnemyPlayAction);
      be.TargetEnemies = new List<Enemy>() { this };
      beq.Enqueue(be);
    }

    if (action.ActionType == EnemyActionTypes.Attack)
    {
      battle.MainCharacter.ReceiveDamage(action.Value, beq);
    }
    else if (action.ActionType == EnemyActionTypes.Defend)
    {
      StatusEffect se = new StatusEffect(StatusEffect.EffectTypes.DefenceUp, action.Value, 2);
      StatusEffects.Add(se);

      {
        BattleEvent be = new BattleEvent(BattleEvent.EventTypes.EnemyGetStatusEffect);
        be.TargetEnemies = new List<Enemy>() { this };
        be.TargetStatusEffects = new List<StatusEffect>() { se };
        beq.Enqueue(be);
      }
    }
    else if (action.ActionType == EnemyActionTypes.Heal)
    {
      Heal(action.Value, beq);
    }

    {
      BattleEvent be = new BattleEvent(BattleEvent.EventTypes.EndEnemyPlayAction);
      be.TargetEnemies = new List<Enemy>() { this };
      beq.Enqueue(be);
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
