using System;
using System.Collections.Generic;
using UnityEngine;
using static Enemy;

[CreateAssetMenu(menuName = "ScriptableObject/Enemy")]
public class EnemySource : ScriptableObject
{
  public string Id;

  public Parameter[] Parameters;

  public Func<Battle, List<EnemyActionData>> GenerateAI()
  {
    if (Id == "ゴブリン")
    {
      return static battle =>
      {
        return new List<EnemyActionData>()
        {
          new EnemyActionData(EnemyActions.Attack, 5),
        };
      };
    }
    else if (Id == "スライム")
    {
      return static battle =>
      {
        return new List<EnemyActionData>()
        {
          new EnemyActionData(EnemyActions.Heal, 5),
          new EnemyActionData(EnemyActions.Defend, 10),
        };
      };
    }
    return null;
  }
}
