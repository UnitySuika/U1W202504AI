using System;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Enemy")]
public class EnemySource : ScriptableObject
{
  public string Id;

  public Enemy.Parameter[] Parameters;

  public Action<Battle> GenerateAI(Battle battle)
  {
    if (Id == "ゴブリン")
    {
      return battle =>
      {

      };
    }
    return null;
  }
}
