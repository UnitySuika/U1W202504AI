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
        actions.Enqueue(new EnemyActionData(EnemyActionTypes.Attack, Array.Find(enemy.Parameters, param => param.Id == "ATK").Value));
        return actions;
      };
    }
    else if (Id == "スライム")
    {
      return (battle, enemy) =>
      {
        Queue<EnemyActionData> actions = new Queue<EnemyActionData>();
        actions.Enqueue(new EnemyActionData(EnemyActionTypes.Heal, Array.Find(enemy.Parameters, param => param.Id == "HEAL").Value));
        
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
          actions.Enqueue(new EnemyActionData(EnemyActionTypes.Defend, Array.Find(enemy.Parameters, param => param.Id == "DEFEND").Value));
        }

        return actions;
      };
    }
    else if (Id == "汎用AI兵")
    {
      return (battle, enemy) => 
      {
        Queue<EnemyActionData> actions = new Queue<EnemyActionData>();

        if (enemy.Hp <= enemy.MaxHp / 2)
        {
          actions.Enqueue(new EnemyActionData(EnemyActionTypes.Defend, Array.Find(enemy.Parameters, param => param.Id == "DEFEND").Value));
        }
        else
        {
          actions.Enqueue(new EnemyActionData(EnemyActionTypes.Attack, Array.Find(enemy.Parameters, param => param.Id == "ATK").Value));
        }

        return actions;
      };
    }
    else
    {
      return (battle, enemy) => 
      {
        Queue<EnemyActionData> actions = new Queue<EnemyActionData>();
        actions.Enqueue(new EnemyActionData(EnemyActionTypes.Heal, Array.Find(enemy.Parameters, param => param.Id == "HEAL").Value));

        return actions;
      };
    }
  }
}
