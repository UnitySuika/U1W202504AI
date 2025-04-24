using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static Enemy;

[CreateAssetMenu(menuName = "ScriptableObject/Enemy")]
public class EnemySource : ScriptableObject
{
  public string Id;

  public Parameter[] Parameters;

  public Func<Battle, Enemy, Queue<EnemyActionData>> GenerateAI()
  {
    if (Id == "ゴブリン")
    {
      return (battle, enemy) =>
      {
        Queue<EnemyActionData> actions = new Queue<EnemyActionData>();
        actions.Enqueue(new EnemyActionData(EnemyActionTypes.Attack, Array.Find(Parameters, param => param.Id == "ATK").Value));
        return actions;
      };
    }
    else if (Id == "スライム")
    {
      return (battle, enemy) =>
      {
        Queue<EnemyActionData> actions = new Queue<EnemyActionData>();
        actions.Enqueue(new EnemyActionData(EnemyActionTypes.Heal, Array.Find(Parameters, param => param.Id == "HEAL").Value));
        
        bool isContainDefenceUpSE = false;
        foreach (StatusEffect statusEffect in enemy.StatusEffects)
        {
          if (statusEffect.Type == StatusEffect.EffectTypes.DefenceUp)
          {
            isContainDefenceUpSE = true;
          }
        }
        if (!isContainDefenceUpSE)
        {
          actions.Enqueue(new EnemyActionData(EnemyActionTypes.Defend, Array.Find(Parameters, param => param.Id == "DEFEND").Value));
        }

        return actions;
      };
    }
    return null;
  }
}
