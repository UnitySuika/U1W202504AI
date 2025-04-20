using System;
using System.Collections.Generic;
using UnityEngine;
using static Enemy;

[CreateAssetMenu(menuName = "ScriptableObject/Enemy")]
public class EnemySource : ScriptableObject
{
  public string Id;

  public Parameter[] Parameters;

  public Func<Battle, Queue<EnemyActionData>> GenerateAI()
  {
    if (Id == "ゴブリン")
    {
      return static battle =>
      {
        Queue<EnemyActionData> actions = new Queue<EnemyActionData>();
        actions.Enqueue(new EnemyActionData(EnemyActionTypes.Attack, 5));
        return actions;
      };
    }
    else if (Id == "スライム")
    {
      return static battle =>
      {
        Queue<EnemyActionData> actions = new Queue<EnemyActionData>();
        actions.Enqueue(new EnemyActionData(EnemyActionTypes.Heal, 5));
        actions.Enqueue(new EnemyActionData(EnemyActionTypes.Defend, 10));
        return actions;
      };
    }
    return null;
  }
}
