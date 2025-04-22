using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEvent
{
  public enum EventTypes
  {
    CardPlayEffect,
    EnemyPlayAction,
    CharacterDamage,
    CharacterHeal,
    EnemyDamage,
    EnemyHeal,
    EnemyGetStatusEffect,
    DecreaseEnergy,
    IncreaseEnergy,
    EndCardPlayEffect,
    EndEnemyPlayAction,
  }

  public EventTypes Type;

  public List<Enemy> TargetEnemies;

  public int Value;

  public Enemy.EnemyActionData TargetAction;

  public List<Enemy.StatusEffect> TargetStatusEffects;

  public BattleEvent(EventTypes type)
  {
    Type = type;
  }
}

public class BattleEventQueue
{

  public Queue<BattleEvent> Events { get; private  set; }

  public BattleEventQueue()
  {
    Events = new Queue<BattleEvent>();
  }

  public void Enqueue(BattleEvent battleEvent)
  {
    Events.Enqueue(battleEvent);
  }
  
  public BattleEvent Dequeue()
  {
    return Events.Dequeue();
  }
}
